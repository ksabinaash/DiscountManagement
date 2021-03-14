﻿using Newtonsoft.Json;
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
            var endpoint = Path.Combine(CallVolumeChartEndpoint, fromDate.ToString(), toDate.ToString());

            return APIHttpGet(endpoint);
        }

        public CallPurposeChart GetCallPurposeChartValues(DateTime? fromDate, DateTime? toDate)
        {
            var endpoint = Path.Combine(CallPurposeChartEndpoint, fromDate.ToString(), toDate.ToString());

            return APIHttpGetPurposeChartValues(endpoint);
        }

        public CallTrendChart GetCallTrendsChartValues(String labName, DateTime? fromDate, DateTime? toDate)
        {
            labName = "RJKPM";
            var endpoint = Path.Combine(CallTrendChartEndpoint, labName,fromDate.ToString(), toDate.ToString());

            return APIHttpGetTrendsChartValues(endpoint);
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

        private CallVolumeChart APIHttpGet(string endpoint)
        {
            CallVolumeChart response = null;

            var baseAddress = crmApiURL;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);

                var responseTask = client.GetAsync(endpoint);

                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<CallVolumeChart>();

                    readTask.Wait();

                    response = readTask.Result;
                }
                else //web api sent error response 
                {

                    response = new CallVolumeChart();
                }
            }
            return response;
        }

        private CallPurposeChart APIHttpGetPurposeChartValues(string endpoint)
        {
            CallPurposeChart response = null;

            var baseAddress = crmApiURL;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);

                var responseTask = client.GetAsync(endpoint);

                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<CallPurposeChart>();

                    readTask.Wait();

                    response = readTask.Result;
                }
                else //web api sent error response 
                {

                    response = new CallPurposeChart();
                }
            }
            return response;
        }

        private CallTrendChart APIHttpGetTrendsChartValues(string endpoint)
        {
            CallTrendChart response = null;

            var baseAddress = crmApiURL;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);

                var responseTask = client.GetAsync(endpoint);

                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<CallTrendChart>();

                    readTask.Wait();

                    response = readTask.Result;
                }
                else //web api sent error response 
                {

                    response = new CallTrendChart();
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
