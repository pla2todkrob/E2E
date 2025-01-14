﻿using System.Web.Optimization;

namespace E2E
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when
            // you're ready for production, use the build tool at https://modernizr.com to pick only
            // the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/Scripts/bootstrap.bundle.min.js",
                      "~/Scripts/DataTables/jquery.dataTables.min.js",
                      "~/Scripts/DataTables/dataTables.bootstrap4.min.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/select2.min.js",
                      "~/Scripts/spin.min.js",
                      "~/Scripts/toastr.min.js",
                      "~/Scripts/jquery.justifiedGallery.min.js",
                      "~/Scripts/colorbox/jquery.colorbox-min.js",
                      "~/Scripts/jquery.signalR-2.4.3.min.js",
                      "~/Scripts/Site.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/colorbox.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/toastr.min.css",
                      "~/Content/css/select2.min.css",
                      "~/Content/css/select2-bootstrap4.min.css",
                      "~/Content/DataTables/css/dataTables.bootstrap4.min.css",
                      "~/Content/justifiedGallery.min.css",
                      "~/Content/Site.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
