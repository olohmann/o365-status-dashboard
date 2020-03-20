namespace O365StatusDashboard.Models.GetServicesCurrentStatus
{
    public enum ServiceStatusEnum
    {
        Investigating,
        ServiceDegradation,
        ServiceInterruption,
        RestoringService,
        ExtendedRecovery,
        ServiceRestored,
        PostIncidentReportPublished,
        VerifyingService,
        ServiceOperational,
        FalsePositive
    }
}