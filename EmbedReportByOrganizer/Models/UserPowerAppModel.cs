using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmbedReportByOrganizer.Models
{
    public class UserPowerAppModel
    {
        public string email { set; get; }
        public string access_token { set; get; }
        public string refresh_token { set; get; }
    }
}