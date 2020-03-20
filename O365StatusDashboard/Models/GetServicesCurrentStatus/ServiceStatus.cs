using System;
using System.Text.Json.Serialization;

namespace O365StatusDashboard.Models.GetServicesCurrentStatus
{
    public class ServiceStatus
    {
        [JsonPropertyName("FeatureStatus")]
        public FeatureStatus[] FeatureStatus { get; set; }

        [JsonPropertyName("Id")]
        public string Id { get; set; }

        [JsonPropertyName("IncidentIds")]
        public string[] IncidentIds { get; set; }

        [JsonPropertyName("Status")]
        public string Status { get; set; }

        [JsonPropertyName("StatusDisplayName")]
        public string StatusDisplayName { get; set; }

        [JsonPropertyName("StatusTime")]
        public DateTimeOffset StatusTime { get; set; }

        [JsonPropertyName("Workload")]
        public string Workload { get; set; }

        [JsonPropertyName("WorkloadDisplayName")]
        public string WorkloadDisplayName { get; set; }
    }
}