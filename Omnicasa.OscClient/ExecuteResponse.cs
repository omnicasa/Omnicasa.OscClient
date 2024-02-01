using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public class ExecuteResponse<T> : ExecuteResponse
    {
        /// <inheritdoc/>
        [JsonProperty("results")]
        public T Results { get; set; }
    }

    /// <inheritdoc/>
    public class ExecuteResponse
    {
        /// <inheritdoc/>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <inheritdoc/>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <inheritdoc/>
        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverterCameCase))]
        public ExecutionState State { get; set; }

        /// <inheritdoc/>
        [JsonProperty("progress")]
        public ExecuteProgress Progress { get; set; }

        /// <inheritdoc/>
        [JsonProperty("error")]
        public ExecuteError Error { get; set; }
    }
}