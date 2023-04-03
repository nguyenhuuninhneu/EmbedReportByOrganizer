using EmbedReportByOrganizer.Models;
using EmbedReportByOrganizer.Service;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace EmbedReportByOrganizer.Controllers
{
    public class HomeController : Controller
    {
        private string m_errorMessage;
        ConfigValidatorService configValidatorService = new ConfigValidatorService();
        public HomeController()
        {
            SDConfig.RefreshTenantCache();
            SDConfig.RefreshConfigurationCache();
            SDConfig.RefreshInsightsCache();
            var tenant = SDConfig.GetTenantCache();
            var user = SDConfig.GetConfigurationCache();
            var insights = SDConfig.GetInsightsCache().FirstOrDefault();
            m_errorMessage = configValidatorService.GetWebConfigErrors(tenant, insights, user);
          
        }

        public ActionResult Tenant()
        {
            ViewBag.Message = "Tenant Power BI parameter.";
            var tenantModel = SDConfig.GetTenantCache();
            return View(tenantModel);
        }
        [System.Web.Http.HttpPost]
        public JsonResult SaveTenant(TenantModel tenantModel)
        {
            var status = true;
            var message = "Success";
            try
            {
                var obj = SDConfig.GetTenantCache();
                if (obj == null)
                {
                    obj = new TenantModel();
                }
                obj.tenantId = tenantModel.tenantId;
                obj.clientId = tenantModel.clientId;
                obj.clientSecret = tenantModel.clientSecret;
                SDConfig.SetTenantCache(obj);
            }
            catch (Exception ex)
            {
                status = false;
                message = "error";
            }
            return Json(new { status = status, message = message });


        }
        public ActionResult InsightsManagement()
        {
            ViewBag.Message = "Insights.";
            var list = SDConfig.GetInsightsCache();
            return View(list);
        }
        public ActionResult Insights(string id)
        {
            ViewBag.Message = "Create Insights";
            InsightsModel insightsModel = new InsightsModel();
            if (!string.IsNullOrEmpty(id))
            {
                var listInsights = SDConfig.GetInsightsCache();

                if (listInsights != null && listInsights.Count > 0)
                {
                    insightsModel = listInsights.FirstOrDefault(p => p.id == id);
                    if (insightsModel == null)
                        insightsModel = new InsightsModel();
                }
            }
            return View(insightsModel);
        }
        [System.Web.Http.HttpPost]
        public JsonResult SaveInsights(InsightsModel insightsModel)
        {
            var status = true;
            var message = "Success";
            try
            {
                var listInsights = SDConfig.GetInsightsCache();
                
                if (insightsModel != null && !string.IsNullOrEmpty(insightsModel.id) && listInsights != null && listInsights.Count > 0)
                {
                    foreach (var item in listInsights)
                    {
                        if (item.id == insightsModel.id)
                        {
                            item.name = insightsModel.name;
                            item.workspaceId = insightsModel.workspaceId;
                            item.reportId = insightsModel.reportId;
                        }
                    }
                    SDConfig.SetInsightsCache(listInsights);
                }
                else
                {
                    if (listInsights == null)
                    {
                        listInsights = new List<InsightsModel>();
                    }
                    insightsModel.id = Guid.NewGuid().ToString();
                    listInsights.Add(insightsModel);
                    SDConfig.SetInsightsCache(listInsights);
                }

            }
            catch (Exception ex)
            {
                status = false;
                message = "error";
            }
            return Json(new { status = status, message = message });


        }
        public ActionResult Configuration()
        {
            ViewBag.Message = "Configuration Account Pro lisence for embed Power BI";
            var configurationAccountModel = SDConfig.GetConfigurationCache();
            if (configurationAccountModel == null)
            {
                configurationAccountModel = SDConfig.CreateDefaultConfiguration();
                SDConfig.SetConfigurationCache(configurationAccountModel);
            }
            ViewBag.userName = configurationAccountModel.userName;
            ViewBag.password = configurationAccountModel.password;
            return View();
        }
        public ActionResult DeleteInsights(string id)
        {
            try
            {
                var listInsights = SDConfig.GetInsightsCache();

                if (listInsights != null && listInsights.Count > 0)
                {
                    listInsights = listInsights.Where(p => p.id != id).ToList();
                    SDConfig.SetInsightsCache(listInsights);
                }

            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("InsightsManagement", "Home");


        }
        [System.Web.Http.HttpPost]
        public JsonResult SaveConfiguration(ConfigurationAccountModel configurationAccountModel)
        {
            var status = true;
            var message = "Success";
            try
            {
                if (configurationAccountModel != null)
                {
                    SDConfig.SetConfigurationCache(configurationAccountModel);
                }
                else
                {
                    SDConfig.CreateDefaultConfiguration();
                }

            }
            catch (Exception ex)
            {
                status = false;
                message = "error";
            }
            return Json(new { status = status, message = message });


        }
        public async Task<ActionResult> ViewReport(string id,string workspaceId, string reportId)
        {
            if (!string.IsNullOrEmpty(m_errorMessage))
            {
                return View("Error", BuildErrorModel(m_errorMessage));
            }

            try
            {
                var settingModel = SDConfig.GetInsightsCache().FirstOrDefault(p => p.id == id);
                if (settingModel != null)
                {
                    var workspaceIdV = Guid.Parse(settingModel.workspaceId);
                    var reportIdV = Guid.Parse(settingModel.reportId);
                    var embedResult = await EmbedService.GetEmbedParams(workspaceIdV, reportIdV);
                    return View(embedResult);

                }
                m_errorMessage = "";
                return View("Error", BuildErrorModel(m_errorMessage));

            }
            catch (HttpOperationException exc)
            {
                m_errorMessage = string.Format("Status: {0} ({1})\r\nResponse: {2}\r\nRequestId: {3}", exc.Response.StatusCode, (int)exc.Response.StatusCode, exc.Response.Content, exc.Response.Headers["RequestId"].FirstOrDefault());
                return View("Error", BuildErrorModel(m_errorMessage));
            }
            catch (Exception ex)
            {
                return View("Error", BuildErrorModel(ex.Message));
            }
        }



        private ErrorModel BuildErrorModel(string errorMessage)
        {
            return new ErrorModel
            {
                ErrorMessage = errorMessage
            };
        }
    }
}