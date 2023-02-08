using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmbedReportByOrganizer.Models
{
    public class GetReportInfoModel
    {
        public string EmbedUrl { set; get; }
        public string ReportId { set; get; }
        public string Token { set; get; }
        public bool IsSuccess { set; get; }
        public bool IsNeedRefreshToken { set; get; }
        public bool IsNeedLoginPopup { set; get; }
        public string Message { set; get; }

        public UserPowerAppModel UserPowerApp { set; get; }
    }
}