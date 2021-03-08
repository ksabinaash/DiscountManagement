using Newtonsoft.Json;
using OfferManagement.Models;
using System;
using System.Collections.Generic;
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

        public void UpdateValidCall(ValidCall validcall)
        {
            APIHttpPut<ValidCall>(ValidCallEndPoint,validcall);
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

        private void APIHttpPut<T>(string endpoint, ValidCall validCall )
        {
            var response = string.Empty;

            var payload = new JavaScriptSerializer().Serialize(validCall);

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
