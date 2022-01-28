
using System.Text.Json.Serialization;

namespace imprmir
{
    class BinaryObjectResponse
    {
        [JsonPropertyName("impresora")]
        public string Impresora { get; set; }

        [JsonPropertyName("imagen")]
        public string Imagen { get; set; }
    }
}