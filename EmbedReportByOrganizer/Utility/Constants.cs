using EmbedReportByOrganizer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace EmbedReportByOrganizer
{
    public static class Constants
    {
        public static string AppCode = "Sensor";
        public static string EndPointLoginGrapshHost = "https://login.microsoftonline.com";
        public static string UriLoginCallBack = "https://localhost:44379/Home/LoginCallback";
        public static string TenantId = "1f32b049-29d1-44ba-877b-a634aed2109d";
        public static string ClientID = "60cbc618-ab8d-4389-94a8-c98f38484b38";
        public static string ClientSecret = "eot8Q~aupgVi4WJVBcKcCA6S5IygwSXxUYZnyb6~";
        public static string ReportID = "8d7cc0f9-e43f-4f24-b315-cc9a39431538";
        public static string EmailDefault = "nhn@staod.onmicrosoft.com";
    }
}
