using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmbedReportByOrganizer.Models
{
    public class ReportResponeSuccessModel
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        public string id { get; set; }
        public string reportType { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
        public string embedUrl { get; set; }
        public bool isOwnedByMe { get; set; }
        public string datasetId { get; set; }
        public List<object> users { get; set; }
        public List<object> subscriptions { get; set; }
    }

    public class GetTokenResponseModel
    {
        public string token_type { get; set; }
        public string scope { get; set; }
        public string expires_in { get; set; }
        public string ext_expires_in { get; set; }
        public string expires_on { get; set; }
        public string not_before { get; set; }
        public string resource { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
    }

    public class ErrorPowerBIEntityNotFound
    {
        public string code { get; set; }

        [JsonProperty("pbi.error")]
        public PbiError pbierror { get; set; }
        public string message { get; set; }
    }


    public class PbiError
    {
        public string code { get; set; }
        public List<object> details { get; set; }
        public int exceptionCulprit { get; set; }
    }

    public class PowerBIEntityNotFound
    {
        public ErrorPowerBIEntityNotFound error { get; set; }
    }
}