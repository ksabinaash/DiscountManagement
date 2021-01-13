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
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpPost]
        public ActionResult Index(DiscountTransaction transaction)
        {

            if (ModelState.IsValid)
            {
                var google = new GoogleSheetsHelper();

                google.CreateEntry(transaction);

                ViewBag.Message = System.Configuration.ConfigurationManager.AppSettings["SuccessfulTransactionMsg"];

                return View("Transaction", transaction);
            }

            return View();

        }

    }
}