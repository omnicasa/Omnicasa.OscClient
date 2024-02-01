using Newtonsoft.Json;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public class StateResponse
    {
        /// <inheritdoc/>
        [JsonProperty("fingerprint")]
        public string Fingerprint { get; set; }

        /// <inheritdoc/>
        [JsonProperty("state")]
        public State State { get; set; }
    }
}