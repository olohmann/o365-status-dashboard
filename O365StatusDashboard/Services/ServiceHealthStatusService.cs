using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using O365StatusDashboard.Models;
using O365StatusDashboard.Models.Configuration;
using O365StatusDashboard.Models.GetServicesCurrentStatus;

namespace O365StatusDashboard.Services
{
    /// <summary>
    /// API Client for Office 365 Service Communications
    /// See https://docs.microsoft.com/en-us/office/office-365-management-api/office-365-service-communications-api-reference
    /// </summary>
    public class ServiceHealthStatusService
    {
        private const string CurrentStatusCacheKey = "CurrentStatus";
        
        private readonly IOptions<ServiceHealthApiConfiguration> _serviceHealthApiConfiguration;
        private readonly AuthenticationProvider _authenticationProvider;
        private readonly IMemoryCache _cache;

        public ServiceHealthStatusService(IOptions<ServiceHealthApiConfiguration> serviceHealthApiConfiguration, IMemoryCache cache)
        {
            _serviceHealthApiConfiguration = serviceHealthApiConfiguration;
            _authenticationProvider = new AuthenticationProvider(
                serviceHealthApiConfiguration.Value.TenantId,
                serviceHealthApiConfiguration.Value.ClientId,
                serviceHealthApiConfiguration.Value.ClientSecret);

            _cache = cache;
        }

        public Task<object> GetServices()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceStatus[]> GetCurrentStatusBlacklisted()
        {
            var workloadBlacklist = _serviceHealthApiConfiguration.Value.WorkloadBlacklist;
            var lst = new List<string>();
            if (!string.IsNullOrWhiteSpace(workloadBlacklist))
            {
                lst = workloadBlacklist.Split(',').Select(_ => _.Trim().ToLower()).ToList();
            }

            var serviceStatusList = await GetCurrentStatus();
            serviceStatusList = serviceStatusList.Where(_ => !lst.Contains(_.Workload.ToLower())).ToArray();

            return serviceStatusList;
        }

        public async Task<ServiceStatus[]> GetCurrentStatus()
        {
            var hasCachedValue = _cache.TryGetValue(CurrentStatusCacheKey, out GetServicesCurrentStatusResponse cachedCurrentStatus);
            if (hasCachedValue)
            {
                return cachedCurrentStatus.Value;
            }
                
            using var httpClient = new HttpClient();
            var req = await PrepareHttpRequestMessage("CurrentStatus");
            var res = await httpClient.SendAsync(req);

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception();
            }

            var currentStatus = await res.Content.ReadAsStringAsync();
            var statusResponse = JsonSerializer.Deserialize<GetServicesCurrentStatusResponse>(currentStatus);
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(_serviceHealthApiConfiguration.Value.CacheDurationInSeconds));

            _cache.Set(CurrentStatusCacheKey, statusResponse, cacheEntryOptions);
            
            return statusResponse.Value;
        }

        public async Task<ServiceStatus> GetCurrentStatus(string workload)
        {
            var serviceStatus = await GetCurrentStatus();
            return serviceStatus.FirstOrDefault(_ => _.Workload == workload);
        }

        public Task<object> GetHistoricalStatus()
        {
            throw new NotImplementedException();
        }

        private async Task<HttpRequestMessage> PrepareHttpRequestMessage(string requestType)
        {
             var uri = $"{GetBaseUri()}/{requestType}";
             
             var req= await _authenticationProvider.CreateHttpRequestMessage();
             req.Method = HttpMethod.Get;
             req.RequestUri = new Uri(uri);
             return req;
        }
        
        private string GetBaseUri()
        {
            return $"https://manage.office.com/api/v1.0/{_serviceHealthApiConfiguration.Value.TenantHost}/ServiceComms";
        }
    }
}