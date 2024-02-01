using Newtonsoft.Json;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public class ExecuteRequest
    {
        /// <inheritdoc/>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <inheritdoc/>
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <inheritdoc/>
    public class ExecuteRequest<T> : ExecuteRequest
    {
        /// <inheritdoc/>
        [JsonProperty("parameters")]
        public T Parameters { get; set; }
    }
}