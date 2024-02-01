using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public class InfoResponse
    {
        /// <inheritdoc/>
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        /// <inheritdoc/>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <inheritdoc/>
        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        /// <inheritdoc/>
        [JsonProperty("firmwareVersion")]
        public string FirmwareVersion { get; set; }

        /// <inheritdoc/>
        [JsonProperty("supportUrl")]
        public string SupportUrl { get; set; }

        /// <inheritdoc/>
        [JsonProperty("apiLevel")]
        public List<int> ApiLevel { get; set; }

        /// <inheritdoc/>
        [JsonProperty("endpoints")]
        public Endpoints Endpoints { get; set; }

        /// <inheritdoc/>
        [JsonProperty("gps")]
        public bool Gps { get; set; }

        /// <inheritdoc/>
        [JsonProperty("gyro")]
        public bool Gyro { get; set; }

        /// <inheritdoc/>
        [JsonProperty("uptime")]
        public int UpTime { get; set; }

        /// <inheritdoc/>
        [JsonProperty("api")]
        public IList<string> Api { get; set; }
    }
}