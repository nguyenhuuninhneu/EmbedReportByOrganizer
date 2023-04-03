using EmbedReportByOrganizer;
using EmbedReportByOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EmbedReportByOrganizer
{
    public static class SDConfig
    {
        public static void RefreshTenantCache()
        {
            TenantModel tenantModel = new TenantModel();
            tenantModel = GetTenantCache();
            if (tenantModel == null)
            {

                tenantModel = CreateDefaultTenant();
            }
            Common.WriteJson(tenantModel, "Tenant");
        }
        public static void SetTenantCache(TenantModel tenantModel)
        {
            Common.WriteJson(tenantModel, "Tenant");
        }
        public static TenantModel GetTenantCache()
        {
            return Common.ReadJson<TenantModel>("Tenant");
        }
        public static TenantModel CreateDefaultTenant()
        {
            var obj = new TenantModel()
            {
                tenantId = Constants.TenantId,
                clientId = Constants.ClientID,
                clientSecret = Constants.ClientSecret,

            };
            return obj;
        }
        public static void RefreshConfigurationCache()
        {
            ConfigurationAccountModel configurationAccountModel = new ConfigurationAccountModel();
            configurationAccountModel = GetConfigurationCache();
            if (configurationAccountModel == null)
            {

                configurationAccountModel = CreateDefaultConfiguration();
            }
            Common.WriteJson(configurationAccountModel, "Configuration");
        }
        public static void SetConfigurationCache(ConfigurationAccountModel configurationAccountModel)
        {
            Common.WriteJson(configurationAccountModel, "Configuration");
        }
        public static ConfigurationAccountModel GetConfigurationCache()
        {
            return Common.ReadJson<ConfigurationAccountModel>("Configuration");
        }
        public static ConfigurationAccountModel CreateDefaultConfiguration()
        {
            var obj = new ConfigurationAccountModel()
            {
                userName = "nhn@staod.onmicrosoft.com",
                password = "Khongbietgi@123",
            };
            return obj;
        }
        public static void RefreshInsightsCache()
        {
            List<InsightsModel> list = new List<InsightsModel>();
            list = GetInsightsCache();
            if (list == null || list.Count == 0)
            {
                list = new List<InsightsModel>();
                list.Add(CreateDefaultInsights());
            }
            Common.WriteJson(list, "Insights");
        }
        public static void SetInsightsCache(List<InsightsModel> list)
        {
            Common.WriteJson(list, "Insights");
        }
        public static List<InsightsModel> GetInsightsCache()
        {
            return Common.ReadJson<List<InsightsModel>>("Insights");
        }

        public static InsightsModel CreateDefaultInsights()
        {
            var obj = new InsightsModel()
            {
                id = Guid.NewGuid().ToString(),
                name = "Insights for Sensor",
                workspaceId = Constants.WorkspaceID,
                reportId = Constants.ReportID,
            };
            return obj;
        }

    }
}
