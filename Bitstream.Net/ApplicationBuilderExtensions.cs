using Microsoft.AspNetCore.Builder;

namespace Bitstream.Net;

public static class BitstreamMiddlewareExtensions
{
  public static IApplicationBuilder UseBitstream(this IApplicationBuilder builder, Dictionary<string, object> options)
  {
    return builder.UseMiddleware<BitstreamMiddleware>(options);
  }
}
