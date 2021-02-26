using NonFactors.Mvc.Grid;
using OfferManagement.ApiLayer;
using OfferManagement.Helpers;
using OfferManagement.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;


namespace OfferManagement.Controllers
{
    public class CRMCallsController : Controller
    {
        public ActionResult MissedCallsInformation()
        {
            var google = new GoogleSheetsHelper();

            List<MissedCallGrid> missedcallsList = APIResults.GetMissedCallGrids() as List<MissedCallGrid>;

            ViewBag.ExportPermission = ((UserModel)Session["UserModel"]) != null ? (bool)((UserModel)Session["UserModel"]).Role.ToString().Equals("ADMINUSER", StringComparison.InvariantCultureIgnoreCase) : false;

            Session["MissedCallsList"] = (missedcallsList != null && missedcallsList.Count >= 0) ? missedcallsList as List<MissedCallGrid> : new List<MissedCallGrid>();

            return View(CreateExportableMissedCallsGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GridPageCount"])));
        }


        private IGrid<MissedCallGrid> CreateExportableMissedCallsGrid(int PageCount)
        {
            var reports = Session["MissedCallsList"] as List<MissedCallGrid>;


            //APIResults aPIResults = new APIResults();

            //var reports = aPIResults.GetMissedCallGrids() as List<MissedCallGrid>;


            IGrid<MissedCallGrid> grid = new Grid<MissedCallGrid>(reports);


            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;

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

            return grid;
        }


        public ActionResult ValidCallsInformation()
        {
            var google = new GoogleSheetsHelper();

            List<MissedCallGrid> missedcallsList = APIResults.GetMissedCallGrids() as List<MissedCallGrid>;

            ViewBag.ExportPermission = ((UserModel)Session["UserModel"]) != null ? (bool)((UserModel)Session["UserModel"]).Role.ToString().Equals("ADMINUSER", StringComparison.InvariantCultureIgnoreCase) : false;

            Session["MissedCallsList"] = (missedcallsList != null && missedcallsList.Count >= 0) ? missedcallsList as List<MissedCallGrid> : new List<MissedCallGrid>();

            return View(CreateExportablecValidCallsGrid(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GridPageCount"])));
        }


        private IGrid<MissedCallGrid> CreateExportablecValidCallsGrid(int PageCount)
        {
            var reports = Session["MissedCallsList"] as List<MissedCallGrid>;


            //APIResults aPIResults = new APIResults();

            //var reports = aPIResults.GetMissedCallGrids() as List<MissedCallGrid>;


            IGrid<MissedCallGrid> grid = new Grid<MissedCallGrid>(reports);


            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;

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

            return grid;
        }


    }
}