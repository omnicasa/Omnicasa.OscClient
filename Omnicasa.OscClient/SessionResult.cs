using Newtonsoft.Json;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public class SessionResult
    {
        /// <inheritdoc/>
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        /// <inheritdoc/>
        [JsonProperty("timeout")]
        public int Timeout { get; set; }
    }

    /// <inheritdoc/>
    public class FileUriResult
    {
        /// <inheritdoc/>
        [JsonProperty("fileUri")]
        public string FileUri { get; set; }
    }
}