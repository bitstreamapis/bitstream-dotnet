using System.Text.Json.Serialization;

namespace Bitstream.Net.Serializtion;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]

[JsonSerializable(typeof(BitstreamPayload))]
[JsonSerializable(typeof(BitstreamPayloadRequest))]
[JsonSerializable(typeof(BitstreamPayloadResponse))]
internal sealed partial class JsonSerializationContext : JsonSerializerContext;