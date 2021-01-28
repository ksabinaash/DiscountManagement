using OfferManagement.Models;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Helpers;

namespace OfferManagement.Helpers
{
    public class MSGWowHelper
    {
        string senderid = System.Configuration.ConfigurationManager.AppSettings["MsgSenderId"]; //"ELIXIR";
        string authKey = System.Configuration.ConfigurationManager.AppSettings["MsgAuthKey"];//"156632AfcQbHHTG0594552e4";
        string messageApiURL = System.Configuration.ConfigurationManager.AppSettings["MsgApiUrl"];//"http://my.msgwow.com/api";


        public bool sendOTP(string mobileNumber,string messageTemplate)
        {
            messageTemplate = HttpUtility.UrlEncode(messageTemplate);
            bool result = false;
            var url = messageApiURL + "/otp.php?authkey=" + authKey +"&mobile=" + mobileNumber +"&message=" + messageTemplate + "&sender=" + senderid + "";
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

        public bool resendOTP(string mobileNumber, string messageTemplate)
        {
            messageTemplate = HttpUtility.UrlEncode(messageTemplate);

            bool result = false;

            var url = messageApiURL + "/retryotp.php?authkey=" + authKey + "&mobile=" + mobileNumber + "&message=" + messageTemplate + "&retrytype=text";
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