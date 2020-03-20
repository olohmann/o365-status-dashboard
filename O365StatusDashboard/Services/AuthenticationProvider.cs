using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using O365StatusDashboard.Models;

namespace O365StatusDashboard.Services
{
    public class AuthenticationProvider 
    {
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public AuthenticationProvider(string tenantId, string clientId, string clientSecret)
        {
            _tenantId = tenantId;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<HttpRequestMessage> CreateHttpRequestMessage()
        {
            using var httpClient = new HttpClient();
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"), 
                new KeyValuePair<string, string>("scope", "https://manage.office.com/.default"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret) 
            });

            var result = await httpClient.PostAsync(GetAuthority(_tenantId), formContent);

            if (!result.IsSuccessStatusCode)
            {
                throw new AuthenticationException("Cannot authenticate with O365 management API. Did you configure the tenant and client correctly?");    
            }

            var authenticationResponseStr = await result.Content.ReadAsStringAsync();
            var authenticationResponse = JsonSerializer.Deserialize<AuthenticationResponse>(authenticationResponseStr);
            var accessToken = authenticationResponse.AccessToken;
            
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");

            return httpRequestMessage;
        }

        private string GetAuthority(string tenantId)
        {
            return $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
        }
    }
}