using System.Globalization;
using System.IO;
using System.Dynamic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Bitstream.Net;

public class BitstreamMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BitstreamMiddleware> _logger;
    private readonly Dictionary<string, object> _options;
    private int _logLevel = 20;

    public BitstreamMiddleware(RequestDelegate next, ILogger<BitstreamMiddleware> logger, Dictionary<string, object> options)
    {
        _next = next;
        _logger = logger;
        _options = options;

        setDefaultOptionValues(options);
    }

    public void setDefaultOptionValues(Dictionary<string, object> options)
    {
        if (options.ContainsKey("LogLevel"))
        {
            _logLevel = (int)options["LogLevel"];
        }
    }

    public async Task Invoke(HttpContext context)
    {
        if (_logLevel >= 50)
        {
            _logger.LogInformation("[Bitstream]: Handling request: {Path}", context.Request.Path);
        }

        // context.Request.EnableBuffering();

        using var payloadBodyReader = new StreamReader(context.Request.Body);
        string payloadBody = await payloadBodyReader.ReadToEndAsync();

        dynamic headers = new ExpandoObject();
        foreach (var header in context.Request.Headers)
        {
            ((IDictionary<string, object>)headers)[header.Key] = header.Value.ToString();
        }

        string requestUrl = context.Request.GetDisplayUrl().ToString();
        if (_logLevel >= 50)
        {
            _logger.LogInformation("[Bitstream]: Request URL: {RequestUrl}", requestUrl);
        }

        dynamic responseBodyString;
        Stream originalResponseBody = context.Response.Body;

        try
        {
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            responseBodyStream.Position = 0;
            using var responseBodyReader = new StreamReader(responseBodyStream);
            var responseBody = await responseBodyReader.ReadToEndAsync();
            responseBodyStream.Position = 0;

            await responseBodyStream.CopyToAsync(originalResponseBody);

            responseBodyString = responseBody;
        }
        finally
        {
            context.Response.Body = originalResponseBody;
        }

        var payloadRequest = new BitstreamPayloadRequest
        {
            method = context.Request.Method,
            url = requestUrl,
            path = context.Request.Path,
            body = payloadBody,
            headers = headers,
            ip = context.Connection?.RemoteIpAddress?.ToString(),
            size = (long?)(payloadBody.Length + headers.ToString().Length)
        };

        if (context.Response.Headers.ContainsKey("Content-Type") && context.Response.Headers["Content-Type"].ToString().Contains("application/json"))
        {
            responseBodyString = JsonConvert.DeserializeObject(responseBodyString);
        }

        var payloadResponse = new BitstreamPayloadResponse
        {
            status = context.Response.StatusCode,
            body = responseBodyString,
            headers = context.Response.Headers,
            size = (long?)(responseBodyString?.ToString().Length + context?.Response?.Headers?.ToString()?.Length ?? 0),
        };

        var payload = new BitstreamPayload
        {
            service_id = _options["ServiceId"].ToString(),
            request = payloadRequest,
            response = payloadResponse
        };

        if (context?.Request?.Query?.Count > 0)
        {
            var queryStrings = new Dictionary<string, string>();
            foreach (var query in context.Request.Query)
            {
                queryStrings[query.Key] = query.Value.ToString();
            }
            payload.request.queryStrings = queryStrings;
        }

        if (_logLevel >= 50)
        {
            _logger.LogInformation("[Bitstream]: Payload: {Payload}", JsonConvert.SerializeObject(payload, Formatting.Indented));
        }

        try
        {
            using (var httpClient = new HttpClient())
            {
                var payloadJson = JsonConvert.SerializeObject(payload);
                var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://dumka33b3rits53fxeovgscbtm0gvvck.lambda-url.eu-west-2.on.aws/", content);

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (_logLevel >= 20)
                    {
                        _logger.LogError("[Bitstream]: Error sending request to Bitstream Event API: {ResponseContent}", responseContent);

                    }
                }
                else
                {
                    if (_logLevel >= 50)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("[Bitstream]: Successfully sent request to Bitstream Event API: {ResponseContent}", responseContent);
                    }
                }

                response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception ex)
        {
            if (_logLevel >= 20)
            {
                _logger.LogError("[Bitstream]: Error sending request to Bitstream Event API: {Exception}", ex);
            }
        }

        if (_logLevel >= 50)
        {
            _logger.LogInformation("[Bitstream]: Finished handling request.");
        }
    }
}
