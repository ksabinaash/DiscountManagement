using Newtonsoft.Json;
using OfferManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;

namespace OfferManagement.ApiLayer
{
    public class APIResults
    {
        readonly string crmApiURL = System.Configuration.ConfigurationManager.AppSettings["CRMApiUrl"];

        readonly string MissedCallEndPoint = System.Configuration.ConfigurationManager.AppSettings["MissedCallEndPoint"];

        readonly string ValidCallEndPoint = System.Configuration.ConfigurationManager.AppSettings["ValidCallEndPoint"];

        readonly string ActionsEndPoint = System.Configuration.ConfigurationManager.AppSettings["ActionsEndPoint"];

        readonly string PurposeEndPoint = System.Configuration.ConfigurationManager.AppSettings["PurposeEndPoint"];

        readonly string ValidCallPutEndPoint = System.Configuration.ConfigurationManager.AppSettings["ValidCallPutEndPoint"];

        readonly string CallVolumeChartEndpoint = System.Configuration.ConfigurationManager.AppSettings["CallVolumeChartEndpoint"];

        readonly string CallPurposeChartEndpoint = System.Configuration.ConfigurationManager.AppSettings["CallPurposeChartEndpoint"];

        readonly string CallTrendChartEndpoint = System.Configuration.ConfigurationManager.AppSettings["CallTrendChartEndpoint"];

        public List<MissedCallGrid> GetMissedCallGrids()
        {
            return APIHttpGet<MissedCallGrid>(MissedCallEndPoint);
        }

        public List<ValidCall> GetValidCallGrid()
        {
            return APIHttpGet<ValidCall>(ValidCallEndPoint);
        }

        public List<CallAction> GetCallActions()
        {
            return APIHttpGet<CallAction>(ActionsEndPoint);
        }

        public List<CallPurpose> GetCallPurpose()
        {
            return APIHttpGet<CallPurpose>(PurposeEndPoint);
        }

        public CallVolumeChart GetCallVolume(DateTime? fromDate, DateTime? toDate)
        {
            //var endpoint = Path.Combine(CallVolumeChartEndpoint, fromDate.ToString(), toDate.ToString());

            var endpoint = CallVolumeChartEndpoint + "?fromDate=" + fromDate + "&toDate = " + toDate;


            return APIHttpGetChartData<CallVolumeChart>(endpoint);
        }

        public CallPurposeChart GetCallPurposeChartValues(DateTime? fromDate, DateTime? toDate)
        {
            //var endpoint = Path.Combine(CallPurposeChartEndpoint, fromDate.ToString(), toDate.ToString());

            var endpoint = CallPurposeChartEndpoint + "?fromDate=" + fromDate + "&toDate = " + toDate;

            return APIHttpGetChartData<CallPurposeChart>(endpoint);
        }

        public CallTrendChart GetCallTrendsChartValues(String labName, DateTime? fromDate, DateTime? toDate)
        {
            //labName = "RJKPM";//Todo: Remove this

            //var endpoint = Path.Combine(CallTrendChartEndpoint, labName, fromDate.ToString(), toDate.ToString());

            var endpoint = CallTrendChartEndpoint + "?labName=" + labName + "&fromDate=" + fromDate + "&toDate = " + toDate;

            return APIHttpGetChartData<CallTrendChart>(endpoint);
        }

        public void UpdateValidCall(ValidCall validcall)
        {
            var endpoint = ValidCallPutEndPoint + validcall.ValidCallId;

            APIHttpPut<ValidCall>(endpoint, validcall);
        }

        private List<T> APIHttpGet<T>(string endpoint)
        {
            List<T> response = null;

            var baseAddress = crmApiURL;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);

                var responseTask = client.GetAsync(endpoint);

                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<T>>();

                    readTask.Wait();

                    response = readTask.Result;
                }
                else //web api sent error response 
                {

                    response = new List<T>();
                }
            }
            return response;
        }

        private T APIHttpGetChartData<T>(string endpoint)
        {
            T response = default(T);

            var baseAddress = crmApiURL;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);

                var responseTask = client.GetAsync(endpoint);

                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<T>();

                    readTask.Wait();

                    response = readTask.Result;
                }
                else //web api sent error response 
                {

                    response = default(T);
                }
            }
            return response;
        }

        private void APIHttpPut<T>(string endpoint, T dataObject)
        {
            var response = string.Empty;

            var payload = new JavaScriptSerializer().Serialize(dataObject);

            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            var baseAddress = crmApiURL;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);

                var responseTask = client.PutAsync(endpoint, content);

                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var statusCode = result.StatusCode.ToString();

                }
                else //web api sent error response 
                {
                    var statusCode = result.StatusCode;
                }
            }

        }
    }
}
