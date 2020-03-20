using System.Linq;

namespace O365StatusDashboard.Models.GetServicesCurrentStatus
{
    public static class ServiceStatusCssHelper
    {
        private readonly static string[] OkStatusSet =
        {
            nameof(ServiceStatusEnum.ServiceOperational)
        };

        private readonly static string[] WarningStatusSet = 
        {
            nameof(ServiceStatusEnum.FalsePositive),
            nameof(ServiceStatusEnum.ServiceRestored),
            nameof(ServiceStatusEnum.PostIncidentReportPublished)
        };
        
        private readonly static string[] DangerStatusSet = 
        {
            nameof(ServiceStatusEnum.Investigating), 
            nameof(ServiceStatusEnum.ExtendedRecovery),
            nameof(ServiceStatusEnum.RestoringService),
            nameof(ServiceStatusEnum.ServiceDegradation),
            nameof(ServiceStatusEnum.ServiceInterruption),
            nameof(ServiceStatusEnum.VerifyingService)
        };


        public static string ServiceStatusToCssClass(string cssPrefix, string serviceStatus)
        {
            if (string.IsNullOrWhiteSpace(serviceStatus))
            {
                return $"{cssPrefix}-warning";
            }

            if (OkStatusSet.Contains(serviceStatus))
            {
                return $"{cssPrefix}-success";
            }

            if (WarningStatusSet.Contains(serviceStatus))
            {
                return $"{cssPrefix}-warning";
            }

            if (DangerStatusSet.Contains(serviceStatus))
            {
                return $"{cssPrefix}-danger";
            }

            return $"{cssPrefix}-warning"; 
        }
    }
}