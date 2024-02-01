using Newtonsoft.Json;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public class Endpoints
    {
        /// <inheritdoc/>
        [JsonProperty("httpPort")]
        public int HttpPort { get; set; }

        /// <inheritdoc/>
        [JsonProperty("httpUpdatesPort")]
        public int HttpUpdatesPort { get; set; }
    }
}