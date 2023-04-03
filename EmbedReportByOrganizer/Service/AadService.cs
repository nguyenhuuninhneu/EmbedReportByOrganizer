using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace EmbedReportByOrganizer.Service
{
    public class AadService
    {
        private static readonly string m_authorityUrl = ConfigurationManager.AppSettings["authorityUrl"];
        private static readonly string AuthenticationType = ConfigurationManager.AppSettings["AuthenticationType"];
        private static readonly string[] m_scope = ConfigurationManager.AppSettings["scopeBase"].Split(';');

        /// <summary>
        /// Get Access token
        /// </summary>
        /// <returns>Access token</returns>
        public static async Task<string> GetAccessToken()
        {
            AuthenticationResult authenticationResult = null;
            string AccessToken = "";
            try
            {
                var tenant = SDConfig.GetTenantCache();
                var user = SDConfig.GetConfigurationCache();
                if (AuthenticationType.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase))
                {
                    
                    IPublicClientApplication clientApp = PublicClientApplicationBuilder
                                                                        .Create(tenant.clientId)
                                                                        .WithAuthority(m_authorityUrl)
                                                                        .Build();
                    var userAccounts = await clientApp.GetAccountsAsync();

                    try
                    {
                        if (userAccounts != null && userAccounts.Any())
                        {
                            authenticationResult = await clientApp.AcquireTokenSilent(m_scope, userAccounts.FirstOrDefault()).ExecuteAsync();
                        }
                        else
                        {
                            SecureString secureStringPassword = new SecureString();
                            foreach (var key in user.password)
                            {
                                secureStringPassword.AppendChar(key);
                            }
                            authenticationResult = await clientApp.AcquireTokenByUsernamePassword(m_scope, user.userName, secureStringPassword).ExecuteAsync();
                        }
                    }
                    catch (MsalUiRequiredException exMsal)
                    {
                        SecureString secureStringPassword = new SecureString();
                        foreach (var key in user.password)
                        {
                            secureStringPassword.AppendChar(key);
                        }
                        authenticationResult = await clientApp.AcquireTokenByUsernamePassword(m_scope, user.userName, secureStringPassword).ExecuteAsync();
                    }
                }

                // Service Principal auth is recommended by Microsoft to achieve App Owns Data Power BI embedding
                else if (AuthenticationType.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase))
                {
                    // For app only authentication, we need the specific tenant id in the authority url
                    var tenantSpecificURL = m_authorityUrl.Replace("organizations", tenant.tenantId);

                    IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder
                                                                                    .Create(tenant.clientId)
                                                                                    .WithClientSecret(tenant.clientSecret)
                                                                                    .WithAuthority(tenantSpecificURL)
                                                                                    .Build();

                    authenticationResult = await clientApp.AcquireTokenForClient(m_scope).ExecuteAsync();

                }
            }
            catch (Exception ex)
            {

            }


            return authenticationResult?.AccessToken;
        }
    }
}