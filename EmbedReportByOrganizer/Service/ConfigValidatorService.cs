using EmbedReportByOrganizer.Models;
using System;
using System.Configuration;
using System.Linq;

namespace EmbedReportByOrganizer.Service
{
    public class ConfigValidatorService
    {
        /// <summary>
        /// Check if web.config embed parameters have valid values.
        /// </summary>
        /// <returns>Null if web.config parameters are valid, otherwise returns specific error string.</returns>
        public string GetWebConfigErrors(TenantModel tenantModel, InsightsModel insightsModel, ConfigurationAccountModel configurationAccountModel)
        {
            string message = null;
            Guid result;
            string applicationType = ConfigurationManager.AppSettings["authenticationType"];
            // Application Id must have a value.
            if (string.IsNullOrWhiteSpace(tenantModel.clientId))
            {
                message = "ApplicationId is empty. please register your application as Native app in https://dev.powerbi.com/apps and fill client Id in web.config.";
            }
            // Application Id must be a Guid object.
            else if (!Guid.TryParse(tenantModel.clientId, out result))
            {
                message = "ApplicationId must be a Guid object. please register your application as Native app in https://dev.powerbi.com/apps and fill application Id in web.config.";
            }
            // Workspace Id must have a value.
            else if (GetParamGuid(insightsModel.workspaceId) == Guid.Empty)
            {
                message = "WorkspaceId is empty or not a valid Guid. Please fill its Id correctly in web.config";
            }
            // Report Id must have a value.
            else if (GetParamGuid(insightsModel.reportId) == Guid.Empty)
            {
                message = "ReportId is empty or not a valid Guid. Please fill its Id correctly in web.config";
            }
            else if (applicationType.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase))
            {
                // Username must have a value.
                if (string.IsNullOrWhiteSpace(configurationAccountModel.userName))
                {
                    message = "Username is empty. Please fill Power BI username in web.config";
                }

                // Password must have a value.
                if (string.IsNullOrWhiteSpace(configurationAccountModel.password))
                {
                    message = "Password is empty. Please fill password of Power BI username in web.config";
                }
            }
            else if (applicationType.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(tenantModel.clientSecret))
                {
                    message = "ApplicationSecret is empty. please register your application as Web app and fill appSecret in web.config.";
                }
                // Must fill tenant Id
                else if (string.IsNullOrWhiteSpace(tenantModel.tenantId))
                {
                    message = "Invalid Tenant. Please fill Tenant ID in Tenant under web.config";
                }
            }
            else
            {
                message = "Invalid authentication type";
            }

            return message;
        }

        private Guid GetParamGuid(string param)
        {
            Guid paramGuid = Guid.Empty;
            Guid.TryParse(param, out paramGuid);
            return paramGuid;
        }
    }
}