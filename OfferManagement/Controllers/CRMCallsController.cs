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
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult MissedCallsInformation()
        {
            ViewBag.ExportPermission = ((UserModel)Session["UserModel"]) != null ? (bool)((UserModel)Session["UserModel"]).Role.ToString().Equals("ADMINUSER", StringComparison.InvariantCultureIgnoreCase) : false;

            var apiResults = new APIResults();

            List<MissedCallGrid> missedcallsList = apiResults.GetMissedCallGrids() as List<MissedCallGrid>;

            bool IsChecked = (Session["NotRespondedCheckBox"]!=null) ? (bool)Session["NotRespondedCheckBox"] : false;

            ViewBag.NotRespondedCheckBox = IsChecked;

            if (IsChecked)
            {
                missedcallsList = missedcallsList.Where(x => x.CallBackStatus.Equals("Not Called Back Yet", StringComparison.InvariantCultureIgnoreCase)).ToList();

                Session["NotRespondedCheckBox"] = false;
            }                       

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

            grid.Columns.Add(model => model.LabName).Titled("LabName");
            grid.Columns.Add(model => model.LabPhoneNumber).Titled("LabPhoneNumber");
            grid.Columns.Add(model => model.CustomerMobileNumber).Titled("CustomerMobileNumber");
            grid.Columns.Add(model => model.CallBackStatus).Titled("CallBackStatus");
            grid.Columns.Add(model => model.IsWhiteListedCall).Titled("IsWhiteListed");
            grid.Columns.Add(model => model.RespondedTime).Titled("RespondedTime").Filterable(GridFilterType.Double);
            grid.Columns.Add(model => model.ValidCallId).Titled("RespondedCallId");
            grid.Columns.Add(model => model.RespondedLabName).Titled("RespondedLabName");
            grid.Columns.Add(model => model.RespondedLabPhoneNumber).Titled("RespondedLabPhoneNumber");
            grid.Columns.Add(model => model.RespondedCallType).Titled("RespondedCallType");
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
            grid.Columns.Add(model => "<button type = \"button\" class=\"btn btn-primary glyphicon glyphicon-pencil\" data-id=\"" + model.ValidCallId + "\" data-toggle=\"modal\" id=\"btnModalPopup\"></button>").Encoded(false).Titled("Edit").Filter.IsEnabled = false;
            grid.Columns.Add(model => model.ValidCallId).Titled("Id");
            grid.Columns.Add(model => model.LabName).Titled("LabName");
            //grid.Columns.Add(model => model.LabPhoneNumber).Titled("LabPhoneNumber");
            grid.Columns.Add(model => model.CustomerMobileNumber).Titled("CustomerMobileNumber");
            //grid.Columns.Add(model => model.CallDuration).Titled("CallDuration");
            grid.Columns.Add(model => model.CallType).Titled("CallType");
            grid.Columns.Add(model => model.CallPurpose).Titled("CallPurpose");
            grid.Columns.Add(model => model.Action).Titled("Action");
            grid.Columns.Add(model => model.Comment).Titled("Comment");
            //grid.Columns.Add(model => model.CallStatus).Titled("CallStatus");
            grid.Columns.Add(model => model.EventTime).Titled("EventTime").Filterable(GridFilterType.Double);
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

            var CallTrends = apiResults.GetCallTrendsChartValues(labName, fromDate, toDate);

            return Json(CallTrends, JsonRequestBehavior.AllowGet);
        }
    }
}