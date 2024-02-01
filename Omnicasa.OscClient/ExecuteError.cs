using Newtonsoft.Json;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public class ExecuteError
    {
        /// <inheritdoc/>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <inheritdoc/>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}