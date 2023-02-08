using EmbedReportByOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EmbedReportByOrganizer.Service
{
    public interface IGraphService
    {
        StringBuilder LoadPopupLoginMicrosoft(string tenant, string clientId);

        RefreshTokenModel RefreshToken(string tenantId, string clientId, string clientSecret, string refresh_token_old);

        Task<GetTokenModel> GetToken(string tenantId, string code, string clientId, string clientSecret);

        Task<GetReportInfoModel> GetReport(string reportId, string token);
    }
}