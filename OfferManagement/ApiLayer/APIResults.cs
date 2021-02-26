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
        public static List<MissedCallGrid> GetMissedCallGrids()
        {
            List<MissedCallGrid> missedCallGrid = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://crmapi.somee.com/");
                //HTTP GET
                var responseTask = client.GetAsync("api/MissedCalls/GetMissedCallsForGrid");
                
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
    }
}
