using Microsoft.AspNetCore.Http;

namespace Bitstream.Net;

public class BitstreamPayload
{
    public string? service_id { get; set; }
    public BitstreamPayloadRequest? request { get; set; }
    public BitstreamPayloadResponse? response { get; set; }
}

public class BitstreamPayloadRequest
{
    public string? method { get; set; }
    public string? url { get; set; }
    public string? path { get; set; }
    public string? body { get; set; }
    public IDictionary<string?, object?>? headers { get; set; }
    public string? ip { get; set; }
    public IDictionary<string, string>? queryStrings { get; set; }
    public long? size { get; set; }
}

public class BitstreamPayloadResponse
{
    public int? status { get; set; }
    public object? body { get; set; }
    public IHeaderDictionary? headers { get; set; }
    public long? size { get; set; }
    public long? time { get; set; }
}