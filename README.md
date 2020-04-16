# O365 Status Dashboard

This is a simple O365 Status Dashboard that leverages the [O365 Service Health API](https://docs.microsoft.com/en-us/office/office-365-management-api/office-365-service-communications-api-reference) to provide non-admin users a quick status overview about their O365 infrastructure. 

## Overview

The status dashboard is a tiny ASP.NET Core MVC application that retrieves tenant-specific O365 status information. It is containerized and can run on Windows, Mac, and Linux.

[Live Demo](https://o365dashboard-e1ac.azurewebsites.net/) and screenshot:
![](./docs/screenshot.png "O365 Status Dashboard Screenshot")

## Setup

### Create AAD App Registration for Service Health API
The application needs a pre-configured service account to get access to the [O365 Service Health API](https://docs.microsoft.com/en-us/office/office-365-management-api/office-365-service-communications-api-reference).

1. Go to the [Azure Portal](https://portal.azure.com) and create a new Azure AD application ([quick link](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps)).
2. In the 'Register an application' dialog, choose a meaningful display name for the application, e.g. "Contoso O365 Status Dashboard".
3. For 'Supported account types' choose 'Accounts in this organizational directory only (Microsoft only - Single tenant)'.
4. The 'Redirect URI' can be left empty.
5. Click 'Register'.

Configure the AAD App:

1. Select 'API permissions'
![](./docs/aad_o365_mgmt_app_01.png)
2. Remove any pre-configured permissions (usually 'Microsoft.Graph/User.Read').
3. Select 'Add a permission'
![](./docs/aad_o365_mgmt_app_02.png)
4. Choose 'Office 365 Management APIs'
![](./docs/aad_o365_mgmt_app_03.png)
5. Select 'Application permissions', search for 'health' and select the 'ServiceHealth.Read' permission.
![](./docs/aad_o365_mgmt_app_04.png)
6. Confirm with 'Add permissions'
7. Grant admin consent for the just created permission. You are done, when you see the green check mark.
![](./docs/aad_o365_mgmt_app_05.png)
8. Create a new secret and keep the created secret in a temporary scratch space (e.g. Notepad).
![](./docs/aad_o365_mgmt_app_06.png)

Collect the following values for the registered AAD App and keep them in a temporary scratch space - you will need them later:
* Tenant Host, e.g. contoso.onmicrosoft.com or contoso.com
* Tenant ID, e.g. 00000000-0000-0000-0000-000000000000
* Client ID also known as Application ID, e.g. 00000000-0000-0000-0000-000000000000
* Client Secret (you just created)

### Running the Dashboard Web App

You can either build the application from scratch, use the provided [Dockerfile](./O365StatusDashboard/Dockerfile) or run a pre-build docker image
from [Docker Hub](https://hub.docker.com/repository/docker/olohmann/o365-status-dashboard).

Here is a sample that uses the pre-built docker image:

1. Create a docker `env.list` file using the configuration from the App Registration:

```bash
ServiceHealthApiConfiguration__TenantHost=contoso.onmicrosoft.com
ServiceHealthApiConfiguration__TenantId=00000000-0000-0000-0000-000000000000
ServiceHealthApiConfiguration__ClientId=00000000-0000-0000-0000-000000000000
ServiceHealthApiConfiguration__ClientSecret=00000000-0000-0000-0000-000000000000
ServiceHealthApiConfiguration__CacheDurationInSeconds=60
ServiceHealthApiConfiguration__WorkloadBlacklist=Lync,SwayEnterprise
CompanyConfiguration__CompanyName=Contoso
CompanyConfiguration__SupportEmail=support@contoso.com
CompanyConfiguration__SupportPhone=+1 000-000-000

# If you run in Azure, you can also inject and application insights instance.
ApplicationInsights__InstrumentationKey=00000000-0000-0000-0000-000000000000
```

2. Run the docker image

```bash
docker run --env-file env.list -p8080:8080 olohmann/o365-status-dashboard:latest
```

### Configuration Reference

The following table defines the environment variable name, an example value and a brief description of the configuration option.

| Variable | Sample Value  | Description  |  
|---|---|---|
|ServiceHealthApiConfiguration__TenantHost| contoso.onmicrosoft.com | The AAD app's tenant host name. |
|ServiceHealthApiConfiguration__TenantId| 00000000-0000-0000-0000-000000000000   |  The AAD app's tenant ID. |
|ServiceHealthApiConfiguration__ClientId| 00000000-0000-0000-0000-000000000000  | The AAD app's client ID. |
|ServiceHealthApiConfiguration__ClientSecret|  00000000-0000-0000-0000-000000000000 | The AAD app's client secret. |
|ServiceHealthApiConfiguration__CacheDurationInSeconds| 60  | Caching duration of service status health in seconds. |
|ServiceHealthApiConfiguration__WorkloadBlacklist| Lync,SwayEnterprise  | The list of workload IDs that should NOT be visible on the dashboard. A case-insensitive, comma-separated list of workloads.|
|CompanyConfiguration__CompanyName| Contoso | The company's name. |
|CompanyConfiguration__SupportEmail| support@contoso.com  | The E-Mail address of the support team. |
|CompanyConfiguration__SupportPhone| +1 000-000-000  | A string that denotes the support phone number.  |

### Deploying the Dashboard Wep App in Azure

There is a fully functional terraform deployment in the folder [iac](./iac/). Use the [tfvars template](./iac/config_sample.tfvars) to customize the deployment.

### Configuring Access Control for the Dashboard when hosting in Azure

You most likely would like to control the access to the application when you deploy it in Azure so only employees from your 
company can see the status dashboard. To do so, you can leverage
the **Azure AD login** with App Services as described in the [documentation](https://docs.microsoft.com/en-us/azure/app-service/configure-authentication-provider-aad). 

If you need to fine-tune the access to the dashboard, you can configure the **Enterprise Application access** via Azure AD. This allows
you to configure access to the dashboard only to specific users or groups and enable a self-service access request. See the 
[documentation](https://docs.microsoft.com/en-us/azure/active-directory/manage-apps/assign-user-or-group-access-portal#configure-an-application-to-require-user-assignment) for details.
