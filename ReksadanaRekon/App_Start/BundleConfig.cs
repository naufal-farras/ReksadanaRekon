using System.Web;
using System.Web.Optimization;

namespace ReksadanaRekon
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/bootbox/bootbox.min.js",
                        "~/Scripts/datatables/jquery.datatables.js",
                        "~/Scripts/datatables/datatables.bootstrap.js",
                        "~/Scripts/datatables/dataTables.responsive.min.js",
                        "~/Scripts/datatables/dataTables.buttons.min.js",
                        "~/Scripts/datatables/dataTables.checkboxes.min.js",
                        "~/Scripts/dataTables.plugin.js",
                        //"~/Scripts/datatables/dataTables.select.js",
                        "~/Scripts/datatables/buttons.colVis.min.js",
                        "~/Scripts/moment-with-locales.min.js",
                        "~/Scripts/daterangepicker.js",
                        "~/Scripts/select2/dist/js/select2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/chart/dist/Chart.js"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap-sandstone.css",
                      "~/Content/daterangepicker.css",
                      "~/Content/datatables/css/datatables.bootstrap.css",
                      "~/Content/datatables/css/dataTables.checkboxes.css",
                      //"~/Content/datatables/css/select.dataTables.css",
                      "~/Content/font-awesome.css",
                      "~/Content/select2/dist/css/select2.min.css",
                      "~/Content/select2/dist/css/select2-bootstrap.css"
                      ));
        }
    }
}
