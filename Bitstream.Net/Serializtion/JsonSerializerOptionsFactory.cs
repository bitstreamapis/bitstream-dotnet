using System.Text.Json;

namespace Bitstream.Net.Serializtion;

/// <summary>
/// Represents a factory for <see cref="JsonSerializerOptions"/>
/// </summary>
/// <remarks>
/// Originally copied over from <see
/// href="https://github.com/akamsteeg/AtleX.HaveIBeenPwned/blob/757ff49e9314f1c3480d8011ec49bbc7b839c564/src/AtleX.HaveIBeenPwned/Serialization/Json/JsonSerializerOptionsFactory.cs"/>
/// and adapted to this project
/// </remarks>
internal static class JsonSerializerOptionsFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="JsonSerializerOptions"/>
    /// </summary>
    /// <returns>
    /// The created <see cref="JsonSerializerOptions"/>
    /// </returns>
    public static JsonSerializerOptions Create()
    {
        var result = new JsonSerializerOptions();

        result.WriteIndented = true;

#if NET8_0_OR_GREATER
        result.TypeInfoResolverChain.Add(new JsonSerializationContext());

        result.MakeReadOnly(); // Guard against modifications
#elif NET6_0_OR_GREATER
        result.AddContext<JsonSerializationContext>();
#endif

        return result;
    }
}