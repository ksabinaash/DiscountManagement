using OfferManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace OfferManagement.ApiLayer
{
    public class APIResults
    {
        readonly string crmApiURL = System.Configuration.ConfigurationManager.AppSettings["CRMApiUrl"];
        readonly string  MissedCallEndPoint = System.Configuration.ConfigurationManager.AppSettings["MissedCallEndPoint"];
        readonly string ValidCallEndPoint = System.Configuration.ConfigurationManager.AppSettings["ValidCallEndPoint"];

        public List<MissedCallGrid> GetMissedCallGrids()
        {
            List<MissedCallGrid> missedCallGrid = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://crmapi.somee.com/");
                //HTTP GET
                var responseTask = client.GetAsync(MissedCallEndPoint);
                
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<MissedCallGrid>>();
                    readTask.Wait();

                    missedCallGrid = readTask.Result;
                }
                else //web api sent error response 
                {
                    missedCallGrid = new List<MissedCallGrid>();

                }
            }
            return missedCallGrid;
        }

        public List<ValidCall> GeValidCallGrid()
        {
            List<ValidCall> missedCallGrid = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmApiURL);
                //HTTP GET
                var responseTask = client.GetAsync(ValidCallEndPoint);

                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<ValidCall>>();
                    readTask.Wait();

                    missedCallGrid = readTask.Result;
                }
                else //web api sent error response 
                {
                    missedCallGrid = new List<ValidCall>();

                }
            }
            return missedCallGrid;
        }

    }
}
