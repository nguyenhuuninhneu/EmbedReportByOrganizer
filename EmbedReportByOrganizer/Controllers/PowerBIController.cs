using EmbedReportByOrganizer.Models;
using EmbedReportByOrganizer.Service;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.Models;
using Microsoft.PowerBI.Api;
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
    public class PowerBIController : Controller
    {
        public async Task<ActionResult> Test()
        {
            var embedConfig = new EmbedConfig
            {
                EmbedToken = new EmbedToken()
                {
                    Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjlHbW55RlBraGMzaE91UjIybXZTdmduTG83WSIsImtpZCI6IjlHbW55RlBraGMzaE91UjIybXZTdmduTG83WSJ9.eyJhdWQiOiJodHRwczovL2FuYWx5c2lzLndpbmRvd3MubmV0L3Bvd2VyYmkvYXBpIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvMWYzMmIwNDktMjlkMS00NGJhLTg3N2ItYTYzNGFlZDIxMDlkLyIsImlhdCI6MTcwMDUzNTEwMywibmJmIjoxNzAwNTM1MTAzLCJleHAiOjE3MDA1MzkyODIsImFjY3QiOjAsImFjciI6IjEiLCJhaW8iOiJBVFFBeS84VkFBQUFOS0hsZkVtcGMxaWJTaUR0RWFpek0zR3FET3ZlQ0lBWVlUaUJQbWIyZUxINE01ZTNHd3RVWjVNcDRjK3BQR0E4IiwiYW1yIjpbInB3ZCJdLCJhcHBpZCI6IjYwY2JjNjE4LWFiOGQtNDM4OS05NGE4LWM5OGYzODQ4NGIzOCIsImFwcGlkYWNyIjoiMSIsImZhbWlseV9uYW1lIjoiTmd1eWVuIiwiZ2l2ZW5fbmFtZSI6Ik5pbmgiLCJpcGFkZHIiOiIxMTMuMTkwLjI0Mi4xNTUiLCJuYW1lIjoiTmluaCBOZ3V5ZW4iLCJvaWQiOiI3NzUwZGRlZS02MGQzLTRmYmQtYWZlNS1kYzFjNDExZjEzOGUiLCJwdWlkIjoiMTAwMzIwMDE1RTNDOEVFRCIsInJoIjoiMC5BWEFBU2JBeUg5RXB1a1NIZTZZMHJ0SVFuUWtBQUFBQUFBQUF3QUFBQUFBQUFBREVBQTQuIiwic2NwIjoiQ29udGVudC5DcmVhdGUgRGF0YXNldC5SZWFkLkFsbCBSZXBvcnQuUmVhZC5BbGwiLCJzdWIiOiJfRFZNRFFGWlA1VE5Rek1YYzJyOXFPTUxOVEtzY2VPQWplSGxnNGZyM0ZBIiwidGlkIjoiMWYzMmIwNDktMjlkMS00NGJhLTg3N2ItYTYzNGFlZDIxMDlkIiwidW5pcXVlX25hbWUiOiJuaG5Ac3Rhb2Qub25taWNyb3NvZnQuY29tIiwidXBuIjoibmhuQHN0YW9kLm9ubWljcm9zb2Z0LmNvbSIsInV0aSI6IlJ2YlJ3T1J3V1VhZy1BZi1TS002QUEiLCJ2ZXIiOiIxLjAiLCJ3aWRzIjpbImI3OWZiZjRkLTNlZjktNDY4OS04MTQzLTc2YjE5NGU4NTUwOSJdfQ.e_Q-CjJD7LLhr0Gj_fjCOmR0otJOOTLt7eVT-D0yiKVasmnWKiLDZo1A2cd0DAnJ0VlaM-vlJUk5M_CbffRRSiQFmSDnsSQSAG5QmROUWBxlzUgLlBaBWKyEmOJ6HPJg_9XpMIpGJzRUAirGPFCyoBjwC3TpbTYntVcj4oy9y3tALJXd_wkzZtOa4b0KhM7zkvqGZy-WqkEekB6il8mVi7xbLitWv5r9DaN4YnlgK5dg3hyVsXtAZ0vlgY12T7NNdUlzlCZbTiF4B64ws9ofRkWMf-RrYvti7p3jhaLcFv_5XSaVoCgqZfAXZZLBjhCcwzdRA5Me4vfaZ4oJ-Jvg0g"
                },
                EmbedUrl = "https://app.powerbi.com/reportEmbed?reportId=16ffde8d-6f64-469a-b07a-da13b3d78f3e&appId=480bc969-73aa-4ca3-a83b-5f6fb6508c20&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVNPVVRILUVBU1QtQVNJQS1CLVBSSU1BUlktcmVkaXJlY3QuYW5hbHlzaXMud2luZG93cy5uZXQiLCJlbWJlZEZlYXR1cmVzIjp7InVzYWdlTWV0cmljc1ZOZXh0Ijp0cnVlfX0%3d",
                ReportId = "16ffde8d-6f64-469a-b07a-da13b3d78f3e",
            };

            return View(embedConfig);
        }
        public async Task<ActionResult> Index()
        {
            var powerBIClient = GetPowerBIClient();
            var report = await GetReport(powerBIClient);

            if (report != null)
            {
                var embedToken = await GetEmbedToken(powerBIClient, report);
                var embedConfig = new EmbedConfig
                {
                    EmbedToken = embedToken,
                    EmbedUrl = report.EmbedUrl,
                    ReportId = report.Id.ToString(),
                };

                return View(embedConfig);
            }

            return View();
        }

        private static PowerBIClient GetPowerBIClient()
        {
            var clientId = "60cbc618-ab8d-4389-94a8-c98f38484b38";
            var clientSecret = "2NK8Q~HD1-aiFmSpUkdDRZp1T7FBcVNIs5~rEci-";
            var tenantId = "1f32b049-29d1-44ba-877b-a634aed2109d";
            var authorityUrl = $"https://login.microsoftonline.com/{tenantId}";
            var credential = new ClientCredential(clientId, clientSecret);
            var authenticationContext = new AuthenticationContext(authorityUrl);
            var authenticationResult = authenticationContext.AcquireTokenAsync("https://analysis.windows.net/powerbi/api", credential).Result;
            var tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");

            return new PowerBIClient(tokenCredentials);
        }

        private static async Task<Report> GetReport(PowerBIClient powerBIClient)
        {
            var groupId = new Guid("dd6a635a-131b-4ab1-8bea-4f206f5f5ecb"); // The Power BI group where the report is located
            var reportId = new Guid("a1c4aaab-faf2-4867-be99-479a24f95085"); // The ID of the report to embed

            return await powerBIClient.Reports.GetReportInGroupAsync(groupId, reportId);
        }

        private static async Task<EmbedToken> GetEmbedToken(PowerBIClient powerBIClient, Report report)
        {
            var groupId = new Guid("dd6a635a-131b-4ab1-8bea-4f206f5f5ecb"); // The Power BI group where the report is located
            var reportId = new Guid("a1c4aaab-faf2-4867-be99-479a24f95085"); // The ID of the report to embed

            var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view")
            {
               AccessLevel = "view",
               AllowSaveAs = true,
            };

            return await powerBIClient.Reports.GenerateTokenInGroupAsync(groupId, reportId, generateTokenRequestParameters);
        }
    }
}