using Newtonsoft.Json;
using NonFactors.Mvc.Grid;
using OfferManagement.ApiLayer;
using OfferManagement.Helpers;
using OfferManagement.Models;
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
            var google = new GoogleSheetsHelper();

            ViewBag.ExportPermission = ((UserModel)Session["UserModel"]) != null ? (bool)((UserModel)Session["UserModel"]).Role.ToString().Equals("ADMINUSER", StringComparison.InvariantCultureIgnoreCase) : false;

            return PopulateChartData();
        }

        public ActionResult PopulateChartData()
        {
            var apiResults = new APIResults();

            List<MissedCallGrid> missedcallsList = apiResults.GetMissedCallGrids() as List<MissedCallGrid>;

            Session["MissedCallsList"] = (missedcallsList != null && missedcallsList.Count >= 0) ? missedcallsList as List<MissedCallGrid> : new List<MissedCallGrid>();

            return View(CreateExportableMissedCallsGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GridPageCount"])));
        }

        public ActionResult PopulateNotRespondedChartData()
        {
            var apiResults = new APIResults();

            List<MissedCallGrid> missedcallsList = apiResults.GetMissedCallGrids() as List<MissedCallGrid>;

            missedcallsList = missedcallsList.Where(x => x.CallBackStatus.Equals("Not Called Back Yet",StringComparison.InvariantCultureIgnoreCase)).ToList();

            Session["MissedCallsList"] = (missedcallsList != null && missedcallsList.Count >= 0) ? missedcallsList as List<MissedCallGrid> : new List<MissedCallGrid>();

            return View("MissedCallsInformation", CreateExportableMissedCallsGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GridPageCount"])));
        }

        public ActionResult PopulateRespondedChartData()
        {
            var apiResults = new APIResults();

            List<MissedCallGrid> missedcallsList = apiResults.GetMissedCallGrids() as List<MissedCallGrid>;

            missedcallsList = missedcallsList.Where(x => x.CallBackStatus.Equals("Already Called Back", StringComparison.InvariantCultureIgnoreCase)).ToList();

            Session["MissedCallsList"] = (missedcallsList != null && missedcallsList.Count >= 0) ? missedcallsList as List<MissedCallGrid> : new List<MissedCallGrid>();

            return View("MissedCallsInformation", CreateExportableMissedCallsGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GridPageCount"])));
        }



        public ActionResult Charts()
        {
            var google = new GoogleSheetsHelper();

            ChartsFilterModel chartsFilterModel = new ChartsFilterModel();

            ViewData["PCCNames"] = google.Transform(google.ReadPCCNames(true));

            return View(chartsFilterModel);
        }


        private IGrid<MissedCallGrid> CreateExportableMissedCallsGrid(int PageCount)
        {
            var reports = Session["MissedCallsList"] as List<MissedCallGrid>;

            IGrid<MissedCallGrid> grid = new Grid<MissedCallGrid>(reports);

            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;


            //columns.Add(model => Html.CheckBox("Check_" + model.Id)).Titled(Html.CheckBox("CheckAll"));

            grid.Columns.Add(model => model.LabName).Titled("LabName");
            grid.Columns.Add(model => model.LabPhoneNumber).Titled("LabPhoneNumber");
            grid.Columns.Add(model => model.CustomerMobileNumber).Titled("CustomerMobileNumber");
            grid.Columns.Add(model => model.CallBackStatus).Titled("CallBackStatus");
            grid.Columns.Add(model => model.RespondedTime).Titled("RespondedTime").Filterable(GridFilterType.Double);
            grid.Columns.Add(model => model.RespondedLabName).Titled("RespondedLabName");
            grid.Columns.Add(model => model.RespondedLabPhoneNumber).Titled("RespondedLabPhoneNumber");
            grid.Columns.Add(model => model.RespondedCallDuration).Titled("RespondedCallDuration");
            grid.Columns.Add(model => model.RespondedCallType).Titled("RespondedCallType");
            grid.Columns.Add(model => model.CallPurpose).Titled("CallPurpose");
            grid.Columns.Add(model => model.Action).Titled("Action");
            grid.Columns.Add(model => model.EventTime).Titled("Missedcall EventTime").Filterable(GridFilterType.Double);
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
            var google = new GoogleSheetsHelper();

            var apiResults = new APIResults();

            var callActions = apiResults.GetCallActions();

            var callPurpose = apiResults.GetCallPurpose();

            var validCalls = apiResults.GetValidCallGrid();

            Session["CallActions"] = (callActions != null && callActions.Count > 0) ? callActions.Select(x => x.Actions).ToList() as List<string> : new List<string>();

            Session["CallPurpose"] = (callPurpose != null && callPurpose.Count > 0) ? callPurpose.Select(x => x.PurposeoftheCall).ToList() as List<string> : new List<string>();

            Session["ValidCallList"] = (validCalls != null && validCalls.Count > 0) ? validCalls as List<ValidCall> : new List<ValidCall>();

            var validCall = new OfferManagement.Models.ValidCall();

            Session["ValidCallsEdit"] = validCall;

            ViewBag.ExportPermission = ((UserModel)Session["UserModel"]) != null ?
                                        (bool)((UserModel)Session["UserModel"]).Role.ToString().Equals("ADMINUSER", StringComparison.InvariantCultureIgnoreCase) : false;

            return View(CreateExportablecValidCallsGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GridPageCount"])));
        }


        private IGrid<ValidCall> CreateExportablecValidCallsGrid(int PageCount)
        {
            var reports = Session["ValidCallList"] as List<ValidCall>;

            IGrid<ValidCall> grid = new Grid<ValidCall>(reports);

            grid.ViewContext = new ViewContext { HttpContext = HttpContext };

            grid.Query = Request.QueryString;

            //grid.Columns.Add(model => "<button type = \"button\" class=\"btn btn-primary\" data-toggle=\"modal\" data-target=\"#validCallsModal\" id=\"btnModalPopup\">Edit</button>").Encoded(false);
            grid.Columns.Add(model => "<button type = \"button\" class=\"btn btn-primary glyphicon glyphicon-pencil\" data-id=\"" + model.ValidCallId + "\" data-toggle=\"modal\" id=\"btnModalPopup\"></button>").Encoded(false).Titled("Edit").Filter.IsEnabled = false;
            grid.Columns.Add(model => model.ValidCallId).Titled("Id");
            grid.Columns.Add(model => model.LabName).Titled("LabName");
            grid.Columns.Add(model => model.LabPhoneNumber).Titled("LabPhoneNumber");
            grid.Columns.Add(model => model.CustomerMobileNumber).Titled("CustomerMobileNumber");
            grid.Columns.Add(model => model.CallDuration).Titled("CallDuration");
            grid.Columns.Add(model => model.CallType).Titled("CallType");
            grid.Columns.Add(model => model.CallPurpose).Titled("CallPurpose");
            grid.Columns.Add(model => model.Action).Titled("Action");
            grid.Columns.Add(model => model.Comment).Titled("Comment");
            grid.Columns.Add(model => model.CallStatus).Titled("CallStatus");
            grid.Columns.Add(model => model.EventTime).Titled("Missedcall EventTime").Filterable(GridFilterType.Double);
            grid.Columns.Add(model => model.UpdatedUser).Titled("UpdatedUser");
            grid.Columns.Add(model => model.UpdatedDateTime).Titled("UpdatedDateTime").Filterable(GridFilterType.Double);
            grid.Columns.Add(model => model.FollowUpTime).Titled("FollowUpTime").Filterable(GridFilterType.Double);
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
            var validCalls = Session["ValidCallList"] as List<ValidCall>;

            var validCall = validCalls.Where(m => m.ValidCallId == id).ToList().FirstOrDefault();

            if (validCall == null)
            {
                validCall = new OfferManagement.Models.ValidCall();
            }


            if (validCall.Comment?.Length > 0 && validCall.Action?.Length > 0 && validCall.Comment?.Length > 0)
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

            existingValidCall.FollowUpTime = currentValidCall.FollowUpTime;

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

            var CallTrends = apiResults.GetCallTrendsChartValues(labName,fromDate, toDate);

            return Json(CallTrends, JsonRequestBehavior.AllowGet);
        }
    }
}