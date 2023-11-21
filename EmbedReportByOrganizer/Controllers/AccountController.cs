using EmbedReportByOrganizer.Models;
using EmbedReportByOrganizer.Service;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            var url = new StringBuilder();
            //string scope = "https://graph.microsoft.com/.default";
            url.AppendFormat(string.Format("https://login.microsoftonline.com/{0}/oauth2/authorize", "1f32b049-29d1-44ba-877b-a634aed2109d"));
            url.AppendFormat("?client_id={0}", Uri.EscapeDataString("60cbc618-ab8d-4389-94a8-c98f38484b38"));
            url.Append("&response_type=code");
            url.AppendFormat("&redirect_uri={0}", HttpUtility.UrlEncode("https://localhost:44379/Account/LoginCallback"));
            //url.AppendFormat("&response_mode=query");
            //url.AppendFormat("&scope=" + scope);
            return Redirect(url.ToString());
        }

        public async Task<ActionResult> LoginCallback()
        {
            var code = Request.Params["code"];
            if (code == null)
            {
                return RedirectToAction("Login");
            }
            string endpointHost = "https://login.microsoftonline.com";
            string grant_type = "authorization_code";
            string resource = "https://analysis.windows.net/powerbi/api/.default";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpointHost);
                using (var request = new HttpRequestMessage(HttpMethod.Post, string.Format("/{0}/oauth2/token", "1f32b049-29d1-44ba-877b-a634aed2109d")))
                {
                    StringContent contentParams = new StringContent(string.Format(
                        "client_id={0}" +
                        "&resource={1}" +
                        "&client_secret={2}" +
                        "&grant_type={3}" +
                        "&code={4}" +
                        "&redirect_uri={5}"
                        , "60cbc618-ab8d-4389-94a8-c98f38484b38"
                        , resource
                        , "eot8Q~aupgVi4WJVBcKcCA6S5IygwSXxUYZnyb6~"
                        , grant_type
                        , code
                        , HttpUtility.UrlEncode("https://localhost:44379/Account/LoginCallback")
                        ), Encoding.UTF8, "application/x-www-form-urlencoded");

                    request.Content = contentParams;

                    using (HttpResponseMessage response = client.SendAsync(request).Result)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var resultJson = response.Content.ReadAsStringAsync().Result;
                            var obj = JsonConvert.DeserializeObject<JObject>(resultJson);
                            string access_token = obj["access_token"].Value<string>();
                            string refresh_token = obj["refresh_token"].Value<string>();

                        }
                        else if (response.Content != null)
                        {
                            return RedirectToAction("Login");
                        }
                    }
                }
            }

            return RedirectToAction("Index", "PowerBI");
        }
    }
}