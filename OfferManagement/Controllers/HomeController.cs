using OfferManagement.Helpers;
using OfferManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfferManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = string.Empty;

            DiscountTransaction transaction = new DiscountTransaction();

            var google = new GoogleSheetsHelper();

            Session["templates"] = google.ReadSMSTemplates(true);

            Session["reasons"] = google.ReadDiscontReasons(true);

            Session["names"] = google.ReadPCCNames(true);

            ViewData["SMSTemplates"] = Transform(Session["templates"] as IList<string>);

            ViewData["DiscountReasons"] = Transform(Session["reasons"] as IList<string>);

            ViewData["PCCNames"] = Transform(Session["names"] as IList<string>);

            return View(transaction);
        }

        public ActionResult About()
        {
            var google = new GoogleSheetsHelper();

            var transactions = google.ReadTransactions(true);

            if (transactions.Count == 0)
            {
                ViewBag.Message = "No Data Available";

                return View();
            }
            else
            {
                return View("Reports", transactions);
            }
        }

        [HttpPost]
        public ActionResult Index(DiscountTransaction transaction)
        {
            ViewData["SMSTemplates"] = Transform(Session["templates"] as IList<string>);

            ViewData["DiscountReasons"] = Transform(Session["reasons"] as IList<string>);

            ViewData["PCCNames"] = Transform(Session["names"] as IList<string>);

            if (ModelState.IsValid)
            {
               var google = new GoogleSheetsHelper();

                transaction.UserEmail = Session["UserEmail"].ToString();

                transaction.ValidationStatus = "Completed";

                google.CreateTransaction(transaction);

                //Send OTP
                MSGWowHelper helper =new  MSGWowHelper();

                var isOTPSent = helper.sendOTP(transaction.MobileNumber);

                if(isOTPSent)
                {
                    ViewBag.Message = "OTP Sent Succesfully, Kindly Enter the received OTP for validation";
                    transaction.enableValidatebtn = true;
                    transaction.enableResendbtn = false;
                    return View(transaction);
                    //enable validate otp button , disabled resend
                }
                else
                {
                    ViewBag.Message = "OTP failure,Try Resend Option";
                    transaction.enableValidatebtn = false;
                    transaction.enableResendbtn = true;
                    //Should we wait for 30 sec or enable resend button ??  -> enable resend button , disabled validate otp btn
                    return View(transaction);
                }

                ViewBag.Message = System.Configuration.ConfigurationManager.AppSettings["SuccessfulTransactionMsg"];

                return View("Transaction", transaction);
            }

            return View();

        }

        
        private List<SelectListItem> Transform(IList<string> values)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            if (values != null && values.Count > 0)
            {
                foreach (var item in values)
                {
                    items.Add(new SelectListItem { Text = item, Value = item });
                }
            }

            return items;
        }

        //public ActionResult ValidateOTP(string otp,string mobile, DiscountTransaction model)
        public ActionResult ValidateOTP( DiscountTransaction modelval)
        {
            MSGWowHelper helper = new MSGWowHelper();

            var isOTPSent = helper.verifyOTP(modelval.OTP, modelval.MobileNumber);

            return View();
        }


        public ActionResult ResendOTP(string mobile)
        {
            MSGWowHelper helper = new MSGWowHelper();

            var isOTPSent = helper.resendOTP(mobile);

            return View();
        }
    }
}