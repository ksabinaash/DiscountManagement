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
            var google = new GoogleSheetsHelper();

            Session["templates"] = google.ReadSMSTemplates(true);

            Session["reasons"] = google.ReadDiscontReasons(true);

            Session["names"] = google.ReadPCCNames(true);

            ViewData["SMSTemplates"] = Transform(Session["templates"] as IList<string>);

            ViewData["DiscountReasons"] = Transform(Session["reasons"] as IList<string>);

            ViewData["PCCNames"] = Transform(Session["names"] as IList<string>);

            return View();
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

    }
}