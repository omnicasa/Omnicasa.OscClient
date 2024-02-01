using Newtonsoft.Json;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public class State
    {
        /// <inheritdoc/>
        [JsonProperty("_batteryState")]
        public string BatteryState { get; set; }

        /// <inheritdoc/>
        [JsonProperty("_captureStatus")]
        public string CaptureStatus { get; set; }

        /// <inheritdoc/>
        [JsonProperty("_latestFileUri")]
        public string LatestFileUri { get; set; }

        /// <inheritdoc/>
        [JsonProperty("_recordedTime")]
        public int RecordedTime { get; set; }

        /// <inheritdoc/>
        [JsonProperty("_recordableTime")]
        public int RecordableTime { get; set; }

        /// <inheritdoc/>
        [JsonProperty("batteryLevel")]
        public double BatteryLevel { get; set; }

        /// <inheritdoc/>
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        /// <inheritdoc/>
        [JsonProperty("storageChanged")]
        public bool StorageChanged { get; set; }
    }
}