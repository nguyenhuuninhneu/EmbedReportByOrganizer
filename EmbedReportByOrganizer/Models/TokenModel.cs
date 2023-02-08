using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmbedReportByOrganizer.Models
{
    public class RefreshTokenModel
    {

        public bool IsSuccess { set; get; }
        public string Message { set; get; }
        public UserPowerAppModel UserPowerApp { set; get; }
    }

    public class GetTokenModel
    {
        public bool IsSuccess { set; get; }
        public string Message { set; get; }
        public UserPowerAppModel UserPowerApp { set; get; }
        public string Redirect { set; get; }
    }
}