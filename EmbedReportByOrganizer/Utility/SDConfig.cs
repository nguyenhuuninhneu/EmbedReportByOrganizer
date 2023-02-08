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
        public static void RefreshSettingCache()
        {
            List<SettingModel> list = new List<SettingModel>();
            list = GetSettingCache();
            if (list == null || list.Count == 0)
            {
                list = new List<SettingModel>();
                list.Add(CreateDefaultSetting());
            }
            Common.WriteJson(list, "Setting");
        }
        public static void SetSettingCache(List<SettingModel> list)
        {
            Common.WriteJson(list, "Setting");
        }
        public static List<SettingModel> GetSettingCache()
        {
            return Common.ReadJson<List<SettingModel>>("Setting");
        }
        public static SettingModel CreateDefaultSetting()
        {
            var obj = new SettingModel()
            {
                appcode = Constants.AppCode,
                tenantId = Constants.TenantId,
                clientId = Constants.ClientID,
                clientSecret = Constants.ClientSecret,
                reportId = Constants.ReportID,
                
            };
            return obj;
        }

        public static void RefreshUserPowerAppCache()
        {
            List<UserPowerAppModel> list = new List<UserPowerAppModel>();
            list = GetUserPowerAppCache();
            if (list == null || list.Count == 0)
            {
                list = new List<UserPowerAppModel>();
                list.Add(CreateDefaultUserPowerApp(Constants.EmailDefault));
                list.Add(CreateDefaultUserPowerApp("intune_user20@staod.onmicrosoft.com"));
            }
            Common.WriteJson(list, "UserPowerApp");
        }
        public static void SetUserPowerAppCache(List<UserPowerAppModel> list)
        {
            Common.WriteJson(list, "UserPowerApp");
        }
        public static List<UserPowerAppModel> GetUserPowerAppCache()
        {
            return Common.ReadJson<List<UserPowerAppModel>>("UserPowerApp");
        }
        public static UserPowerAppModel CreateDefaultUserPowerApp(string email)
        {
            var obj = new UserPowerAppModel()
            {
                email = email,
                access_token = "",
                refresh_token = "",

            };
            return obj;
        }
    }
}
