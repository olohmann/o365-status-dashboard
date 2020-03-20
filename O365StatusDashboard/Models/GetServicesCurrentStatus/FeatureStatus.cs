using System.Text.Json.Serialization;

namespace O365StatusDashboard.Models.GetServicesCurrentStatus
{
    public class FeatureStatus
    {
        [JsonPropertyName("FeatureDisplayName")]
        public string FeatureDisplayName { get; set; }

        [JsonPropertyName("FeatureName")]
        public string FeatureName { get; set; }

        [JsonPropertyName("FeatureServiceStatus")]
        public string FeatureServiceStatus { get; set; }

        [JsonPropertyName("FeatureServiceStatusDisplayName")]
        public string FeatureServiceStatusDisplayName { get; set; }
    }
}