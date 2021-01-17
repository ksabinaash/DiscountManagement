﻿using OfferManagement.Models;
using System.IO;
using System.Net;
using System.Web.Helpers;

namespace OfferManagement.Helpers
{
    public class MSGWowHelper
    {
        string senderid = "ELIXIR";
        string authKey = "156632AfcQbHHTG0594552e4";
        string messageApiURL = "http://my.msgwow.com/api";

        public bool sendOTP(string mobileNumber)
        {
            bool result = false;
            var url = messageApiURL + "/otp.php?authkey=" + authKey +"&mobile=" + mobileNumber + "&sender=" + senderid + "";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            string responsestring;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                responsestring = reader.ReadToEnd();
            }

            var jsonresponse = Json.Decode<MsgWowApiResponse>(responsestring);

            result= jsonresponse.type.Equals("success", System.StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        public bool verifyOTP(string otpNumber,string mobileNumber)
        {
            bool result = false;

            var url = messageApiURL + "/verifyRequestOTP.php?authkey=" + authKey + "&mobile=" + mobileNumber + "&otp=" + otpNumber + "";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            string responsestring;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                responsestring = reader.ReadToEnd();
            }
            var jsonresponse = Json.Decode<MsgWowApiResponse>(responsestring);

            result = jsonresponse.type.Equals("success", System.StringComparison.InvariantCultureIgnoreCase) &&
                     ! jsonresponse.message.Equals("otp_not_verified", System.StringComparison.InvariantCultureIgnoreCase);

            return result;
        }

        public bool resendOTP(string mobileNumber)
        {
            bool result = false;

            var url = messageApiURL + "/retryotp.php?authkey=" + authKey + "&mobile=" + mobileNumber + "&retrytype=text";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            string responsestring;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                responsestring = reader.ReadToEnd();
            }
            var jsonresponse = Json.Decode<MsgWowApiResponse>(responsestring);

            result = jsonresponse.type.Equals("success", System.StringComparison.InvariantCultureIgnoreCase);
            return result;
        }
    }
}