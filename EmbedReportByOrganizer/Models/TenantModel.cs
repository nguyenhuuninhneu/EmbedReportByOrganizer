using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmbedReportByOrganizer.Models
{
    public class TenantModel
    {
        public string tenantId { set; get; }
        public string clientId { set; get; }
        public string clientSecret { set; get; }
        //public string workspaceId { set; get; }
        //public string reportId { set; get; }
    }

}