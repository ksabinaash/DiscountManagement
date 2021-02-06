using OfferManagement.Helpers;
using OfferManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.Data;
using System.IO;
using OfficeOpenXml;
using NonFactors.Mvc.Grid;

namespace OfferManagement.Controllers
{
    [Authorize]
    [HandleError]
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

            //Session["enabletimer"] = false;

            ViewData["SMSTemplates"] = Transform(Session["templates"] as IList<string>);

            ViewData["DiscountReasons"] = Transform(Session["reasons"] as IList<string>);

            ViewData["PCCNames"] = Transform(Session["names"] as IList<string>);

            transaction.MessageTemplate = Transform(Session["templates"] as IList<string>).FirstOrDefault().Text;

            return View(transaction);
        }

        public ActionResult About()
        {
            var google = new GoogleSheetsHelper();

            var transactions = google.ReadTransactions(true);

            ViewBag.ExportPermission = ((UserModel)Session["UserModel"])!=null?(bool)((UserModel)Session["UserModel"]).Role.ToString().Equals("ADMINUSER", StringComparison.InvariantCultureIgnoreCase):false;

            Session["ReportsList"] = (transactions != null && transactions.Count >= 0) ? transactions as List<DiscountTransaction> : new List<DiscountTransaction>();

            return View(CreateExportableGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GridPageCount"])));
        }

        [HttpGet]
        public PartialViewResult ReportsGrid()
        {
            var reports = Session["ReportsList"] as List<DiscountTransaction>;
            // Only grid query values will be available here.
            return PartialView("ReportsGrid", reports.AsQueryable());
        }
        
        private IGrid<DiscountTransaction> CreateExportableGrid(int PageCount)
        {
            var reports = Session["ReportsList"] as List<DiscountTransaction>;
            IGrid<DiscountTransaction> grid = new Grid<DiscountTransaction>(reports);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;

            grid.Columns.Add(model => model.CustomerName).Titled("Customer Name");
            grid.Columns.Add(model => model.CustomerEmail).Titled("Customer Email");
            grid.Columns.Add(model => model.UserEmail).Titled("User Email");
            grid.Columns.Add(model => model.PCCName).Titled("PCC Name");
            grid.Columns.Add(model => model.DiscountReason).Titled("Discount Reason");
            grid.Columns.Add(model => model.BillNo).Titled("Bill No");
            grid.Columns.Add(model => model.BillValue).Titled("Bill Value");
            grid.Columns.Add(model => model.Discount).Titled("Discount");
            grid.Columns.Add(model => model.BilledValue).Titled("Billed Value");
            grid.Columns.Add(model => model.MessageTemplate).Titled("Template");
            grid.Columns.Add(model => model.BilledDateTime).Titled("Billed Date").Filterable(GridFilterType.Double);
            grid.Columns.Add(model => model.ValidationStatus).Titled("Status");

            grid.Pager = new GridPager<DiscountTransaction>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = PageCount;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;
        }


        [HttpGet]
        public ActionResult ExportIndex()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<DiscountTransaction> grid = CreateExportableGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ExportPageCount"]));
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;

                    column.IsEncoded = false;
                }

                foreach (IGridRow<DiscountTransaction> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }

                return File(package.GetAsByteArray(), "application/unknown", @System.Configuration.ConfigurationManager.AppSettings["ApplicationName"] + DateTime.UtcNow.AddHours(5).AddMinutes(30).ToString() + ".xlsx");
            }
        }

        public IList<DiscountTransaction> readGooglesheetvalues()
        {
            var google = new GoogleSheetsHelper();

            IList<DiscountTransaction> transactions = google.ReadTransactions(true);

            if (transactions.Count >= 0)
            {
                return transactions;
            }
            else
            {
                return null;
            }
        }

        //public FileResult Export(IList<DiscountTransaction> lst)
        //{

        //    DataTable dt = new DataTable("ElixerTransactions");
        //    dt.Columns.AddRange(new DataColumn[14] {
        //                                    new DataColumn("CustomerName"),
        //                                    new DataColumn("UserEmail"),
        //                                    new DataColumn("CustomerEmail"),
        //                                    new DataColumn("MobileNumber"),
        //                                    new DataColumn("PCC Name"),
        //                                    new DataColumn("OTP"),
        //                                    new DataColumn("OTP Mesage Template"),
        //                                    new DataColumn("Bill No"),
        //                                    new DataColumn("BillValue"),
        //                                    new DataColumn("Discount"),
        //                                    new DataColumn("BilledValue"),
        //                                    new DataColumn("DiscountReason"),
        //                                    new DataColumn("BilledDateTime"),
        //                                    new DataColumn("ValidationStatus")});

        //    IList<DiscountTransaction> list = readGooglesheetvalues();

        //    foreach (var modelval in list)
        //    {
        //        dt.Rows.Add(modelval.CustomerName, modelval.UserEmail, modelval.CustomerEmail, modelval.MobileNumber,
        //            modelval.PCCName,
        //            modelval.OTP,
        //            modelval.MessageTemplate,modelval.BillNo,
        //            modelval.BillValue, modelval.Discount, modelval.BilledValue, modelval.DiscountReason, modelval.BilledDateTime, modelval.ValidationStatus);
        //    }

        //    using (XLWorkbook wb = new XLWorkbook())
        //    {
        //        wb.Worksheets.Add(dt);
        //        using (MemoryStream stream = new MemoryStream())
        //        {
        //            wb.SaveAs(stream);
        //            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ElixerTransactions" + DateTime.Now + ".xlsx");
        //        }
        //    }
        //}

        [HttpPost]
        public ActionResult Index(DiscountTransaction model)
        {
            ViewData["SMSTemplates"] = Transform(Session["templates"] as IList<string>);

            ViewData["DiscountReasons"] = Transform(Session["reasons"] as IList<string>);

            ViewData["PCCNames"] = Transform(Session["names"] as IList<string>);

            if (ModelState.IsValid && Session["UserEmail"] != null)
            {
                var google = new GoogleSheetsHelper();

                model.UserEmail = Session["UserEmail"].ToString();

                model.ValidationStatus = System.Configuration.ConfigurationManager.AppSettings["OTPVerificationPendingMsg"];

                model.enableSubmitbtn = false;

                MSGWowHelper helper = new MSGWowHelper();

                var messageTempalte = model.MessageTemplate.
                    Replace(System.Configuration.ConfigurationManager.AppSettings["Customername"], model.CustomerName)
                    .Replace(System.Configuration.ConfigurationManager.AppSettings["Discount"], model.Discount.ToString() + " ")
                    .Replace(System.Configuration.ConfigurationManager.AppSettings["Discountreason"], model.DiscountReason)
                    .Replace(System.Configuration.ConfigurationManager.AppSettings["Billvalue"], model.BillValue.ToString());

                model.MessageTemplate = messageTempalte;

                google.CreateTransaction(model);

                var isOTPSent = helper.sendOTP(model.MobileNumber, messageTempalte);

                //var isOTPSent = true;

                if (isOTPSent)
                {
                    ViewBag.Message = System.Configuration.ConfigurationManager.AppSettings["SuccessfulTransactionMsg"];

                    //Session["enabletimer"] = true;

                    model.enableValidatebtn = true;

                    // model.enableResendbtn = true;

                    Session["transaction"] = model;

                    google.UpdateMsgTemplate(model);


                    return View(model);
                    //enable validate otp button , disabled resend
                }
                else
                {
                    ViewBag.Message = System.Configuration.ConfigurationManager.AppSettings["FailureOTPMsg"];

                    model.enableValidatebtn = true;  // Final fix

                    // model.enableResendbtn = true;

                    Session["transaction"] = model;

                    //Should we wait for 30 sec or enable resend button ??  -> enable resend button , disabled validate otp btn
                    return View(model);
                }
            }

            return View(model);

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
                    model.ValidationStatus = System.Configuration.ConfigurationManager.AppSettings["OTPVerifiedMsg"];

                    model.OTP = otp;

                    google.UpdateValidationStatus(model);

                    ViewBag.Message = System.Configuration.ConfigurationManager.AppSettings["SuccessfulTransactionMsg"];

                    Session["transaction"] = model;
                }
                else
                {
                    ViewBag.Message = System.Configuration.ConfigurationManager.AppSettings["InvalidOTPMsg"]; ;

                    //Session["enabletimer"] = true;

                    return View("Index", model);
                }

                return View("Transaction", model);
            }
            else
            {
                ViewBag.Message = System.Configuration.ConfigurationManager.AppSettings["ProvideOTPMsg"];

                return View("Index", model);
            }
        }

        //[HttpPost]
        public ActionResult ResendOTP()
        {
            MSGWowHelper helper = new MSGWowHelper();

            var google = new GoogleSheetsHelper();

            var model = Session["transaction"] as DiscountTransaction;

            if (model != null)
            {
                ViewData["SMSTemplates"] = Transform(Session["templates"] as IList<string>);

                ViewData["DiscountReasons"] = Transform(Session["reasons"] as IList<string>);

                ViewData["PCCNames"] = Transform(Session["names"] as IList<string>);

                var messageTempalte = model.MessageTemplate.
                    Replace(System.Configuration.ConfigurationManager.AppSettings["Customername"], model.CustomerName)
                    .Replace(System.Configuration.ConfigurationManager.AppSettings["Discount"], model.Discount.ToString() + " ")
                    .Replace(System.Configuration.ConfigurationManager.AppSettings["Discountreason"], model.DiscountReason)
                    .Replace(System.Configuration.ConfigurationManager.AppSettings["Billvalue"], model.BillValue.ToString());


                var isOTPSent = helper.resendOTP(model.MobileNumber, messageTempalte);

                if (isOTPSent)
                {
                    ViewBag.Message = System.Configuration.ConfigurationManager.AppSettings["SuccessfulOTPMsg"];

                    model.enableValidatebtn = true;

                    // model.enableResendbtn = false;

                }
                else
                {
                    ViewBag.Message = System.Configuration.ConfigurationManager.AppSettings["FailureOTPMsg"]; ;
                    model.enableValidatebtn = false;
                    // model.enableResendbtn = true;

                }

                Session["transaction"] = model;
            }
            return View("Index", model);
        }

        public ActionResult UserDetails()
        {
            UserModel us = ((UserModel)Session["UserModel"]);
            return View("UserDetails", us);
        }
    }


   
}