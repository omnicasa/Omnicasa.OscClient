using Newtonsoft.Json;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public class ExecuteProgress
    {
        /// <inheritdoc/>
        [JsonProperty("completion")]
        public float Completion { get; set; }
    }
}