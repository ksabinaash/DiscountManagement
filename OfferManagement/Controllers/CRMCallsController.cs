using Newtonsoft.Json;
using NonFactors.Mvc.Grid;
using OfferManagement.ApiLayer;
using OfferManagement.Helpers;
using OfferManagement.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace OfferManagement.Controllers
{
    [Authorize]
    [HandleError]
    public class CRMCallsController : Controller
    {
       public ActionResult MissedCallsInformation()
        {
            ViewBag.ExportPermission = ((UserModel)Session["UserModel"]) != null ? (bool)((UserModel)Session["UserModel"]).Role.ToString().Equals("ADMINUSER", StringComparison.InvariantCultureIgnoreCase) : false;

            var apiResults = new APIResults();

            var callActions = apiResults.GetCallActions();

            var callPurpose = apiResults.GetCallPurpose();

            Session["CallActions"] = (callActions != null && callActions.Count > 0) ? callActions.Select(x => x.Actions).ToList() as List<string> : new List<string>();

            Session["CallPurpose"] = (callPurpose != null && callPurpose.Count > 0) ? callPurpose.Select(x => x.PurposeoftheCall).ToList() as List<string> : new List<string>();

            List<MissedCallGrid> missedcallsList = apiResults.GetMissedCallGrids() as List<MissedCallGrid>;

            bool IsChecked = (Session["NotRespondedCheckBox"]!=null) ? (bool)Session["NotRespondedCheckBox"] : false;

            ViewBag.NotRespondedCheckBox = IsChecked;

            if (IsChecked)
            {
                missedcallsList = missedcallsList.Where(x => x.CallBackStatus.Equals("Not Called Back Yet", StringComparison.InvariantCultureIgnoreCase)).ToList();

                Session["NotRespondedCheckBox"] = false;
            }

            Session["missedCallsForExport"] = missedcallsList;


            return View(CreateExportableMissedCallsGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GridPageCount"]), missedcallsList));
        }

        public void GetNonResondedMissedCalls()
        {
            Session["NotRespondedCheckBox"] = true;
        }

        public void FilterValidCallsInformation()
        {
            Session["IsValidCallsCheckBoxChecked"] = true;
        }

        public ActionResult Charts()
        {
            var google = new GoogleSheetsHelper();

            ChartsFilterModel chartsFilterModel = new ChartsFilterModel();

            ViewData["PCCNames"] = google.Transform(google.ReadPCCNames(true));

            return View(chartsFilterModel);
        }


        private IGrid<MissedCallGrid> CreateExportableMissedCallsGrid(int PageCount, List<MissedCallGrid> missedCalls)
        {
            var reports = missedCalls;

            IGrid<MissedCallGrid> grid = new Grid<MissedCallGrid>(reports);

            grid.ViewContext = new ViewContext { HttpContext = HttpContext };

            grid.Query = Request.QueryString;
            
            grid.Columns.Add(model => "<button type = \"button\" class=\"btn btn-primary glyphicon glyphicon-pencil\" data-id=\"" + model.ValidCallId + "\" data-toggle=\"modal\" id=\"btnModalPopup\"></button>").Encoded(false).Titled("Action");
            grid.Columns.Add(model => model.LabName).Titled("Lab Name");
            grid.Columns.Add(model => model.CustomerMobileNumber).Titled("Customer MobileNumber");
            grid.Columns.Add(model => model.CallBackStatus).Titled("CallBackS tatus");
            grid.Columns.Add(model => model.IsWhiteListedCall).Titled("Is WhiteListed");
            grid.Columns.Add(model => model.RespondedTime).Titled("Responded Time").Filterable(GridFilterType.Double);
            grid.Columns.Add(model => model.ValidCallId).Titled("Responded CallId");
            grid.Columns.Add(model => model.RespondedLabName).Titled("Responded LabName");
            grid.Columns.Add(model => model.RespondedCallType).Titled("Responded CallType");
            grid.Columns.Add(model => model.CallPurpose).Titled("CallPurpose");
            grid.Columns.Add(model => model.Action).Titled("Action");
            grid.Columns.Add(model => model.EventTime).Titled("Responded EventTime").Filterable(GridFilterType.Double);
            grid.Columns.Add(model => model.Comment).Titled("Comment");

            grid.Pager = new GridPager<MissedCallGrid>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = PageCount;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            grid.EmptyText = "No data found";

            return grid;
        }


        public ActionResult ValidCallsInformation()
        {
            var apiResults = new APIResults();

            var callActions = apiResults.GetCallActions();

            var callPurpose = apiResults.GetCallPurpose();

            var validCalls = apiResults.GetValidCallGrid();

            Session["CallActions"] = (callActions != null && callActions.Count > 0) ? callActions.Select(x => x.Actions).ToList() as List<string> : new List<string>();

            Session["CallPurpose"] = (callPurpose != null && callPurpose.Count > 0) ? callPurpose.Select(x => x.PurposeoftheCall).ToList() as List<string> : new List<string>();

            Session["ValidCallList"] = (validCalls != null && validCalls.Count > 0) ? validCalls as List<ValidCall> : new List<ValidCall>();

            var validCall = new OfferManagement.Models.ValidCall();

            Session["ValidCallsEdit"] = validCall;

            bool IsChecked = (Session["IsValidCallsCheckBoxChecked"] != null) ? (bool)Session["IsValidCallsCheckBoxChecked"] : false;

            ViewBag.IsFilterSelected = IsChecked;

            if (IsChecked)
            {
                //Todo: Filter needs to be applied here;

                validCalls = validCalls.Where(x => (x.Action != null && x.Action.ToUpper().Contains("FOLLOW"))).ToList();

                Session["IsValidCallsCheckBoxChecked"] = false;
            }

            Session["validCallsForExport"] = validCalls;

            ViewBag.ExportPermission = ((UserModel)Session["UserModel"]) != null ?
                                        (bool)((UserModel)Session["UserModel"]).Role.ToString().Equals("ADMINUSER", StringComparison.InvariantCultureIgnoreCase) : false;

            return View(CreateExportablecValidCallsGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GridPageCount"]), validCalls));
        }


        private IGrid<ValidCall> CreateExportablecValidCallsGrid(int PageCount, List<ValidCall> validCalls)
        {
            var reports = validCalls;

            IGrid<ValidCall> grid = new Grid<ValidCall>(reports);

            grid.ViewContext = new ViewContext { HttpContext = HttpContext };

            grid.Query = Request.QueryString;
            grid.Columns.Add(model => "<button type = \"button\" class=\"btn btn-primary glyphicon glyphicon-pencil\" data-id=\"" + model.ValidCallId + "\" data-toggle=\"modal\" id=\"btnModalPopup\"></button>").Encoded(false).Titled("Edit");
            grid.Columns.Add(model => model.ValidCallId).Titled("Id");
            grid.Columns.Add(model => model.LabName).Titled("Lab Name");
            grid.Columns.Add(model => model.CustomerMobileNumber).Titled("Customer MobileNumber");
            grid.Columns.Add(model => model.CallType).Titled("Call Type");
            grid.Columns.Add(model => model.MissedFollowUpOf).Titled("Missed Call Info");
            grid.Columns.Add(model => model.CallPurpose).Titled("Call Purpose");
            grid.Columns.Add(model => model.Action).Titled("Action");
            grid.Columns.Add(model => model.Comment).Titled("Comment");
            grid.Columns.Add(model => model.EventTime).Titled("Event Time").Filterable(GridFilterType.Double);
            grid.Columns.Add(model => model.UpdatedUser).Titled("Updated User");
            grid.Columns.Add(model => model.UpdatedDateTime).Titled("Updated DateTime").Filterable(GridFilterType.Double);
            grid.Columns.Add(model => model.FollowUpTime).Titled("FollowUp Time").Filterable(GridFilterType.Double);
            grid.Columns.Add(model => model.Comment).Titled("Comment");

            grid.Pager = new GridPager<ValidCall>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = PageCount;
            
            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            grid.EmptyText = "No data found";
            return grid;
        }

        [HttpGet]
        public ActionResult PrepareModalPopup(int id)
        {
            var apiResults = new APIResults();

            var validCalls = apiResults.GetValidCallGrid();

            var validCall = validCalls.Where(m => m.ValidCallId == id).ToList().FirstOrDefault();

            if (validCall == null)
            {
                validCall = new OfferManagement.Models.ValidCall();
            }

            if (validCall.Action != null && validCall.Action.Equals("Closed", StringComparison.InvariantCultureIgnoreCase))
            {
                ViewData["enableForm"] = "false";
            }
            else
            {
                ViewData["enableForm"] = "true";
            }

            return PartialView("ValidCallsInformationEdit", validCall);
        }

        [HttpPost]
        public ActionResult UpdateValidCallModel(ValidCall currentValidCall)
        {
            var validCalls = Session["ValidCallList"] as List<ValidCall>;

            var existingValidCall = validCalls.Where(m => m.ValidCallId == currentValidCall.ValidCallId).ToList().FirstOrDefault();

            var user = (UserModel)Session["UserModel"];

            existingValidCall.UpdatedUser = user?.UserName;

            existingValidCall.UpdatedDateTime = DateTime.Now;

            existingValidCall.Comment = currentValidCall.Comment;

            existingValidCall.CallPurpose = currentValidCall.CallPurpose;

            existingValidCall.Action = currentValidCall.Action;

            if (currentValidCall.FollowUpTime != null)
            {
                existingValidCall.FollowUpTime = currentValidCall.FollowUpTime.GetValueOrDefault().AddHours(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ServerHoursDifference"])).AddMinutes(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ServerMinsDifference"]));
            }

            var apiResults = new APIResults();

            apiResults.UpdateValidCall(existingValidCall);

            return View();
        }

        public JsonResult GetCallVolume(DateTime fromDate, DateTime toDate)
        {
            var apiResults = new APIResults();

            var CallVolume = apiResults.GetCallVolume(fromDate, toDate);

            return Json(CallVolume, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCallPurpose(DateTime fromDate, DateTime toDate)
        {
            var apiResults = new APIResults();

            var CallPurpose = apiResults.GetCallPurposeChartValues(fromDate, toDate);

            return Json(CallPurpose, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCallTrends(string labName, DateTime fromDate, DateTime toDate)
        {
            var apiResults = new APIResults();

            var CallTrends = apiResults.GetCallTrendsChartValues(labName, fromDate, toDate);

            return Json(CallTrends, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult ExportValidCalls()
        {
            var validCallGridValue = Session["validCallsForExport"] as List<ValidCall>;
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<ValidCall> grid = CreateExportablecValidCallsGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ExportPageCount"]), validCallGridValue);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns.Skip(1))
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;

                    column.IsEncoded = false;
                }

                foreach (IGridRow<ValidCall> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns.Skip(1))
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }

                return File(package.GetAsByteArray(), "application/unknown", @System.Configuration.ConfigurationManager.AppSettings["ValidCallsInformation"] + DateTime.UtcNow.AddHours(5).AddMinutes(30).ToString() + ".xlsx");
            }
        }


        [HttpGet]
        public ActionResult ExportMissedCalls()
        {
            var missedCallGridValue = Session["missedCallsForExport"] as List<MissedCallGrid>;
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<MissedCallGrid> grid = CreateExportableMissedCallsGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ExportPageCount"]), missedCallGridValue);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;

                    column.IsEncoded = false;
                }

                foreach (IGridRow<MissedCallGrid> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }

                return File(package.GetAsByteArray(), "application/unknown", @System.Configuration.ConfigurationManager.AppSettings["MissedCallsInformation"] + DateTime.UtcNow.AddHours(5).AddMinutes(30).ToString() + ".xlsx");
            }
        }

    }
}