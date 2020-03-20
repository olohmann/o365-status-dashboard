using System;
using System.Text.Json.Serialization;

namespace O365StatusDashboard.Models.GetServicesCurrentStatus
{
    public class GetServicesCurrentStatusResponse
    {
        [JsonPropertyName("@odata.context")]
        public Uri OdataContext { get; set; }

        [JsonPropertyName("value")]
        public ServiceStatus[] Value { get; set; }
    }
}