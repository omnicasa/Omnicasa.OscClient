using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public class SessionParameter
    {
        /// <inheritdoc/>
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }

    /// <inheritdoc/>
    public class OptionParameter
    {
        /// <inheritdoc/>
        [JsonProperty("options")]
        public IDictionary<string, object> Options { get; set; }
    }

    /// <inheritdoc/>
    public class ClientVersionParameter : SessionParameter
    {
        /// <inheritdoc/>
        [JsonProperty("options")]
        public IDictionary<string, object> Options { get; set; }
    }
}