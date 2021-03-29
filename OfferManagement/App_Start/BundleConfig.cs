using System.Web;
using System.Web.Optimization;

namespace OfferManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                       "~/Scripts/moment-with-locales.js"));

            bundles.Add(new ScriptBundle("~/bundles/Chart").Include(
                       "~/Scripts/Chart.js"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/datetime").Include(
                     //"~/scripts/bootstrap-datetimepicker.js",
                     //"~/scripts/bootstrap-datepicker.js",
                     "~/scripts/jquery.datetimepicker.js"));



            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/themes/base/jquery-ui.css",
                      "~/Content/Site.css",
                      "~/Content/Chart.css",
                      "~/Content/jquery.datetimepicker.css"));
        }
    }
}
