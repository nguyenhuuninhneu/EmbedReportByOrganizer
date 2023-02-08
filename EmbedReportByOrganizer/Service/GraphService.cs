using EmbedReportByOrganizer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EmbedReportByOrganizer.Service
{
    public class GraphService : IGraphService
    {
        public StringBuilder LoadPopupLoginMicrosoft(string tenant, string clientId)
        {

            var url = new StringBuilder();
            url.AppendFormat(string.Format("https://login.microsoftonline.com/{0}/oauth2/authorize", tenant));
            url.AppendFormat("?client_id={0}", Uri.EscapeDataString(clientId));
            url.Append("&response_type=code");
            url.AppendFormat("&redirect_uri={0}", Constants.UriLoginCallBack);
            url.AppendFormat("&resource=https://analysis.windows.net/powerbi/api");
            return url;

        }

        public RefreshTokenModel RefreshToken(string tenantId, string clientId, string clientSecret, string refresh_token_old)
        {
            var result = new RefreshTokenModel();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Constants.EndPointLoginGrapshHost);
                    using (var request = new HttpRequestMessage(HttpMethod.Post, string.Format("/{0}/oauth2/token", tenantId)))
                    {
                        StringContent contentParams = new StringContent(string.Format(
                            "grant_type={0}" +
                            "&client_id={2}" +
                            "&client_secret={3}" +
                            "&refresh_token={4}"
                            , "refresh_token"
                            , Uri.EscapeDataString(clientId)
                            , clientSecret
                            , refresh_token_old
                            ), Encoding.UTF8, "application/x-www-form-urlencoded");

                        request.Content = contentParams;

                        using (HttpResponseMessage response = client.SendAsync(request).Result)
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var resultJson = response.Content.ReadAsStringAsync().Result;
                                try
                                {
                                    GetTokenResponseModel reportRespone = JsonConvert.DeserializeObject<GetTokenResponseModel>(resultJson);
                                    if (!string.IsNullOrEmpty(reportRespone.error) && reportRespone.error.Equals("invalid_grant"))
                                    {
                                        result.Message = reportRespone.error_description;
                                        return result;
                                    }
                                    result.UserPowerApp = new UserPowerAppModel();
                                    result.UserPowerApp.access_token = reportRespone.access_token;
                                    result.UserPowerApp.refresh_token = reportRespone.refresh_token;
                                    result.IsSuccess = true;
                                }
                                catch (Exception ex)
                                {
                                    result.Message = ex.Message;
                                }

                            }
                            else if (response.Content != null)
                            {
                                var errorDetails = response.Content.ReadAsStringAsync().Result;
                                result.Message = errorDetails;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GetTokenModel> GetToken(string tenantId, string code,string clientId, string clientSecret)
        {
            var result = new GetTokenModel();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Constants.EndPointLoginGrapshHost);
                    using (var request = new HttpRequestMessage(HttpMethod.Post, string.Format("/{0}/oauth2/token", tenantId)))
                    {
                        StringContent contentParams = new StringContent(string.Format(
                            "grant_type={0}" +
                            "&code={1}" +
                            "&client_id={2}" +
                            "&client_secret={3}" +
                            "&redirect_uri={4}" +
                            "&resource={5}"
                            , "authorization_code"
                            , code
                            , Uri.EscapeDataString(clientId)
                            , clientSecret
                            , HttpUtility.UrlEncode(Constants.UriLoginCallBack)
                            , "https://analysis.windows.net/powerbi/api"
                            ), Encoding.UTF8, "application/x-www-form-urlencoded");

                        request.Content = contentParams;

                        using (HttpResponseMessage response = client.SendAsync(request).Result)
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var resultJson = response.Content.ReadAsStringAsync().Result;
                                try
                                {
                                    GetTokenResponseModel reportRespone = JsonConvert.DeserializeObject<GetTokenResponseModel>(resultJson);
                                    if (!string.IsNullOrEmpty(reportRespone.error) && reportRespone.error.Equals("invalid_grant"))
                                    {
                                        result.Message = reportRespone.error_description;
                                        return result;
                                    }

                                    result.UserPowerApp = new UserPowerAppModel();
                                    result.UserPowerApp.access_token = reportRespone.access_token;
                                    result.UserPowerApp.refresh_token = reportRespone.refresh_token;
                                    result.IsSuccess = true;
                                }
                                catch (Exception ex)
                                {
                                    result.Message = ex.Message;
                                }

                            }
                            else if (response.Content != null)
                            {
                                var errorDetails = response.Content.ReadAsStringAsync().Result;
                                GetTokenResponseModel reportResponeError = JsonConvert.DeserializeObject<GetTokenResponseModel>(errorDetails);
                                if (!string.IsNullOrEmpty(reportResponeError.error) && reportResponeError.error.Equals("invalid_grant"))
                                {
                                    result.Message = reportResponeError.error_description;
                                    return result;
                                }
                                result.Message = errorDetails;
                                result.Redirect = "Embed";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GetReportInfoModel> GetReport(string reportId, string token)
        {
            GetReportInfoModel getReportInfoModel = new GetReportInfoModel();
            using (var client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.powerbi.com/v1.0/myorg/reports/" + reportId);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.OK)
                {
                    string resultJsonError = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(resultJsonError))
                    {
                        getReportInfoModel.IsNeedRefreshToken = true;
                        getReportInfoModel.Message = "Token is interupted";
                        return getReportInfoModel;
                    }
                    try
                    {
                        PowerBIEntityNotFound reportRespone = JsonConvert.DeserializeObject<PowerBIEntityNotFound>(resultJsonError);
                        if (reportRespone.error.code.Equals("TokenExpired"))
                        {
                            getReportInfoModel.IsNeedRefreshToken = true;
                            getReportInfoModel.Message = "TokenExpire";
                            return getReportInfoModel;
                        }
                        else if (reportRespone.error.code.Equals("PowerBIEntityNotFound"))
                        {
                            //if (string.IsNullOrEmpty(reportRespone.error.message))
                            //{
                            //    getReportInfoModel.Message = "Wrong report information.";
                            //}
                            //else
                            //{
                            //    getReportInfoModel.Message = "You don't have permission to see the report. Please contact to administrator to get it.";

                            //}
                            getReportInfoModel.Message = "You dont have permission to see the report. Please contact to administrator to get it.";
                            return getReportInfoModel;
                        }
                        else
                        {
                            getReportInfoModel.Message = reportRespone.error.message;
                            return getReportInfoModel;
                        }
                        //throw new HttpResponseException(response.StatusCode);
                    }
                    catch (Exception ex)
                    {
                        getReportInfoModel.Message = ex.Message;
                        return getReportInfoModel;
                    }
                    
                }

                // Record users in the data store (note that this only records the first page of users)

                try
                {
                    string resultJson = await response.Content.ReadAsStringAsync();
                    PowerBIEntityNotFound reportRespone = JsonConvert.DeserializeObject<PowerBIEntityNotFound>(resultJson);
                    if (reportRespone.error != null && !string.IsNullOrEmpty(reportRespone.error.code))
                    {
                        getReportInfoModel.Message = reportRespone.error.message;
                    }
                    else
                    {
                        ReportResponeSuccessModel reportResponeSuccess = JsonConvert.DeserializeObject<ReportResponeSuccessModel>(resultJson);

                        getReportInfoModel.IsSuccess = true;
                        getReportInfoModel.EmbedUrl = reportResponeSuccess.embedUrl;
                        getReportInfoModel.Token = token;
                        getReportInfoModel.ReportId = reportResponeSuccess.id;
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return getReportInfoModel;


        }
    }
}