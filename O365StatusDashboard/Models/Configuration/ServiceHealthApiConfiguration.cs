namespace O365StatusDashboard.Models.Configuration
{
    public class ServiceHealthApiConfiguration
    {
        public string TenantHost { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int CacheDurationInSeconds { get; set; } = 60;
        
        /// <summary>
        /// Comma-separated black list of services that should not be shown.
        /// Use the short name of these services to blacklist them.
        /// </summary>
        public string WorkloadBlacklist { get; set; }
    }
}