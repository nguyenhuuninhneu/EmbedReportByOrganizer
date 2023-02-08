using EmbedReportByOrganizer.Models;
using EmbedReportByOrganizer.Service;
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
        IGraphService graphService = new GraphService();
        //Trang dang nhap: ListUser
        public ActionResult ListUser()
        {
            Session["Email"] = "";
            ViewBag.Message = "List User page.";
            SDConfig.RefreshSettingCache();
            SDConfig.RefreshUserPowerAppCache();
            ViewBag.ListUser = SDConfig.GetUserPowerAppCache();
            return View();
        }

        public ActionResult SettingEmbed()
        {
            ViewBag.Message = "Setting Power BI parameter.";
            var settingModel = SDConfig.GetSettingCache().FirstOrDefault(p => p.appcode == Constants.AppCode);
            ViewBag.TenantId = settingModel.tenantId;
            ViewBag.ClientId = settingModel.clientId;
            ViewBag.ClientSecret = settingModel.clientSecret;
            ViewBag.ReportId = settingModel.reportId;
            return View();
        }

        [System.Web.Http.HttpPost]
        public JsonResult SaveSetting(SettingModel settingModel)
        {
            var status = true;
            var message = "Success";
            try
            {
                var listSetting = SDConfig.GetSettingCache();
                if (listSetting != null && listSetting.Count > 0)
                {
                    foreach (var item in listSetting)
                    {
                        if (item.appcode == Constants.AppCode)
                        {
                            item.tenantId = settingModel.tenantId;
                            item.clientId = settingModel.clientId;
                            item.clientSecret = settingModel.clientSecret;
                            item.reportId = settingModel.reportId;
                        }
                    }
                    SDConfig.SetSettingCache(listSetting);
                }
                
            }
            catch (Exception ex)
            {
                status = false;
                message = "error";
            }
            return Json(new { status = status, message = message });


        }
        public async Task<ActionResult> Embed()
        {
            ViewBag.Message = "Embed page.";
            var report = new GetReportInfoModel();

            try
            {
                string emailLogin = Session["Email"]?.ToString();
                if (string.IsNullOrEmpty(emailLogin))
                {
                    return RedirectToAction("ListUser");
                }
                var userPowerApp = new UserPowerAppModel();
                var settingModel = new SettingModel();
                
                //Lay thong tin user + setting tu DB ra

                #region Get Data
                var listPowerApp = SDConfig.GetUserPowerAppCache();
                var listSetting = SDConfig.GetSettingCache();
                
                if (listPowerApp != null && listPowerApp.Count > 0)
                    userPowerApp = listPowerApp.FirstOrDefault(p => p.email == emailLogin);

                if (listSetting != null && listSetting.Count > 0)
                    settingModel = listSetting.FirstOrDefault(p => p.appcode == Constants.AppCode);

                string token = userPowerApp?.access_token;
                report.Token = token;
                #endregion


                if (settingModel == null || (settingModel != null && (string.IsNullOrEmpty(settingModel.tenantId) || string.IsNullOrEmpty(settingModel.clientSecret) || string.IsNullOrEmpty(settingModel.reportId)|| string.IsNullOrEmpty(settingModel.reportId))))
                {
                    //Chua cau hinh tham so thi thong bao loi
                    report.Message = "You havent't set up (tenantId, clientId,clientsecret,reportId) for embed";
                    report.EmbedUrl = "";
                    report.ReportId = settingModel.reportId;
                    ViewBag.Report = report;
                    return View();
                }
                if (string.IsNullOrEmpty(token))
                {
                    //Neu chua login => Login de lay token
                    var url = graphService.LoadPopupLoginMicrosoft(settingModel.tenantId, settingModel.clientId);
                    return Redirect(url.ToString());
                }
                else
                {
                    //Khi da co token => get report
                    report = await graphService.GetReport(settingModel.reportId, userPowerApp.access_token);
                    if (report.IsNeedRefreshToken)
                    {
                        //Token het han can refresh
                        var refreshModel = graphService.RefreshToken(settingModel.tenantId, settingModel.clientId, settingModel.clientSecret, userPowerApp.refresh_token);
                        if (!refreshModel.IsSuccess)
                        {
                            //Lay token khong thanh cong => Login lai
                            var url = graphService.LoadPopupLoginMicrosoft(settingModel.tenantId, settingModel.clientId);
                            return Redirect(url.ToString());
                        }
                        else
                        {
                            if (refreshModel != null && refreshModel.UserPowerApp != null)
                            {
                                //Lay Token luu lai vao DB
                                var listuser = SDConfig.GetUserPowerAppCache();
                                var email = Session["Email"]?.ToString();
                                if (listuser !=null && listuser.Count > 0 && !string.IsNullOrEmpty(email))
                                {
                                    foreach (var item in listuser)
                                    {
                                        if (item.email.Equals(email))
                                        {
                                            item.access_token = refreshModel.UserPowerApp.access_token;
                                            item.refresh_token = refreshModel.UserPowerApp.refresh_token;
                                        }
                                    }
                                    SDConfig.SetUserPowerAppCache(listuser);
                                    //Lay lai token moi duoc gan cho user de load report 
                                    userPowerApp = SDConfig.GetUserPowerAppCache().FirstOrDefault(p => p.email == emailLogin);
                                    if (userPowerApp != null)
                                    {
                                        report = await graphService.GetReport(settingModel.reportId, userPowerApp.access_token);
                                    }
                                }
                            }
                        }
                    }
                    if (report.IsNeedLoginPopup)
                    {
                        //popup login lay token
                        var url = graphService.LoadPopupLoginMicrosoft(settingModel.tenantId, settingModel.clientId);
                        return Redirect(url.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                report.Message = ex.Message;
            }
            ViewBag.Report = report;
            return View();
        }


        public async Task<ActionResult> LoginCallback()
        {
            try
            {
                var errorCode = Request.Params["error"];
                var errorDes = Request.Params["error_description"];
                if (errorCode != null)
                {
                    TempData["LoginErrorMessage"] = (errorDes == null ? "" : errorDes);
                    return RedirectToAction("ListUser");
                }
                var code = Request.Params["code"];
                if (code == null)
                {
                    TempData["LoginErrorMessage"] = "You cannot log in using Single Sign-On";
                    return RedirectToAction("ListUser");
                }
                var settingModel = SDConfig.GetSettingCache().FirstOrDefault(p => p.appcode == Constants.AppCode);
                if (settingModel != null)
                {
                    var loginResult = await graphService.GetToken(settingModel.tenantId, code, settingModel.clientId, settingModel.clientSecret);
                    if (loginResult != null)
                    {
                        if (loginResult.IsSuccess)
                        {
                            var listuser = SDConfig.GetUserPowerAppCache();
                            var email = Session["Email"]?.ToString();
                            if (!string.IsNullOrEmpty(email))
                            {
                                foreach (var item in listuser)
                                {
                                    if (item.email.Equals(email))
                                    {
                                        item.access_token = loginResult.UserPowerApp.access_token;
                                        item.refresh_token = loginResult.UserPowerApp.refresh_token;
                                    }
                                }
                                SDConfig.SetUserPowerAppCache(listuser);
                            }
                        }
                        else
                        {
                            TempData["LoginErrorMessage"] = loginResult.Message;
                            if (!string.IsNullOrEmpty(loginResult.Redirect))
                            {
                                RedirectToAction("ListUser");
                            }
                        }
                    }
                }
                else
                {
                    TempData["LoginErrorMessage"] = "Setting is not set up.";
                    return RedirectToAction("ListUser");
                }
            }
            catch (Exception ex)
            {
            }
            
            return RedirectToAction("Embed");
        }


        public ActionResult LoginView(string email)
        {
            Session["Email"] = email;

            return RedirectToAction("Embed");
        }

    }
}