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

            if (ModelState.IsValid && Session["UserEmail"].ToString() != null)  //check useremail session failure case
            {
               var google = new GoogleSheetsHelper();

                transaction.UserEmail = Session["UserEmail"].ToString();

                transaction.ValidationStatus = "OTP Verification Pending";

                google.CreateTransaction(transaction);

                transaction.enableSubmitbtn = false;

                MSGWowHelper helper = new MSGWowHelper();


                var messageTempalte = transaction.MessageTemplate.Replace("#Customername", transaction.CustomerName)
                    .Replace("#discount ", transaction.Discount.ToString()+" ")
                    .Replace("#discountreason", transaction.DiscountReason)
                    .Replace("#billedvalue", transaction.BillValue.ToString());

                var isOTPSent = helper.sendOTP(transaction.MobileNumber, messageTempalte);

                //var isOTPSent = true;

                if (isOTPSent)
                {
                    ViewBag.Message = "OTP Sent Succesfully, Kindly Enter the received OTP for validation";
                    transaction.enableValidatebtn = true;
                    transaction.enableResendbtn = false;
                    Session["transaction"] = transaction;
                    return View(transaction);
                    //enable validate otp button , disabled resend
                }
                else
                {
                    ViewBag.Message = "OTP failure,Try Resend Option";
                    transaction.enableValidatebtn = false;
                    transaction.enableResendbtn = true;
                    Session["transaction"] = transaction;
                    //Should we wait for 30 sec or enable resend button ??  -> enable resend button , disabled validate otp btn
                    return View(transaction);
                }

               
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

        public ActionResult ValidateOTP(string otp)
        //public ActionResult ValidateOTP(DiscountTransaction model)
        //public ActionResult ValidateOTP( DiscountTransaction modelval)
        {
            var google = new GoogleSheetsHelper();

            var model = Session["transaction"] as DiscountTransaction;

            ViewData["SMSTemplates"] = Transform(Session["templates"] as IList<string>);

            ViewData["DiscountReasons"] = Transform(Session["reasons"] as IList<string>);

            ViewData["PCCNames"] = Transform(Session["names"] as IList<string>);

            if (otp.Length == 4)
            {
                MSGWowHelper helper = new MSGWowHelper();

                var isOTPSent = helper.verifyOTP(otp, model.MobileNumber);

                //var isOTPSent = true;

                if (isOTPSent)
                {
                    model.ValidationStatus = "OTP Verified";

                    model.OTP = otp;

                    //update google sheet
                    google.UpdateTransaction(model);

                    ViewBag.Message = System.Configuration.ConfigurationManager.AppSettings["SuccessfulTransactionMsg"];

                    Session["transaction"] = model;
                }
                else
                {
                    ViewBag.Message = "Invalid OTP, Please try with valid OTP";

                    return View("Index",model);
                }

                return View("Transaction", model);
            }
            else
            {
                ViewBag.Message = "Please provide OTP";

                return View("Index", model);
            }
        }


        public ActionResult ResendOTP(DiscountTransaction model)
        {
            MSGWowHelper helper = new MSGWowHelper();

            ViewData["SMSTemplates"] = Transform(Session["templates"] as IList<string>);

            ViewData["DiscountReasons"] = Transform(Session["reasons"] as IList<string>);

            ViewData["PCCNames"] = Transform(Session["names"] as IList<string>);

            var messageTempalte = model.MessageTemplate.Replace("#Customername", model.CustomerName)
                   .Replace("#discount ", model.Discount.ToString() + " ")
                   .Replace("#discountreason", model.DiscountReason)
                   .Replace("#billedvalue", model.BillValue.ToString());

            var isOTPSent = helper.resendOTP(model.MobileNumber, messageTempalte);

            //var isOTPSent = true;


            if (isOTPSent)
            {
                ViewBag.Message = "OTP Sent Succesfully, Kindly Enter the received OTP for validation";
                model.enableValidatebtn = true;
                model.enableResendbtn = false;

            }
            else
            {
                ViewBag.Message = "OTP failure,Try Resend Option";
                model.enableValidatebtn = false;
                model.enableResendbtn = true;

            }

            Session["transaction"] = model;

            return View("Index", model);
        }
    }
}