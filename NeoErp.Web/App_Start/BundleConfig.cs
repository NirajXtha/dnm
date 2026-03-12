using System.Web;
using System.Web.Optimization;

namespace NeoErp
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include());

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                      //  "~/Scripts/jquery-ui-{version}.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       // "~/Scripts/jquery.unobtrusive*",
                       // "~/Scripts/jquery.validate*"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                     //   "~/Scripts/modernizr-*"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                //"~/Content/NepaliDatePicker/nepali.datepicker.v2.1.min.js",               
                //"~/Content/tokeninput/src/jquery.tokeninput.js",
                //"~/Content/tinymce/tinymce.js",
                //"~/Scripts/gridmvc.js",
                //"~/Scripts/gridmvc_fixheader_overload.js",
                //"~/Scripts/NeoErpJS.js",
                //"~/Content/floatThead/jquery.floatThead.js"
                //"~/Scripts/tokenInputBind.js",
                //"~/Scripts/bootstrap-datepicker.js"
                ));

            bundles.Add(new StyleBundle("~/Content/csslist1").Include(
                        //"~/Content/css/layout.css",
                        //"~/Content/css/icons.css",                                   
                        // "~/Content/NepaliDatePicker/nepali.datepicker.css",
                        // "~/Content/themes/base/jquery.ui.all.css",
                        // "~/Content/Gridmvc/Gridmvc.css",
                        // "~/Content/Gridmvc/gridmvc.datepicker.css",
                        // "~/Content/bootstrap/bootstrap.min.css",
                        // "~/Content/bootstrap/bootstrap-theme.min.css",
                        // "~/Content/tokeninput/styles/token-input-facebook.css",
                        // "~/Content/tokeninput/styles/token-input.css",
                        //  "~/hs/highslide.css",
                        // "~/Content/css/NeoErp_integration.css",
                        // "~/Content/font-awesome/css/font-awesome.css" 
                        ));
            

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        //"~/Content/themes/base/jquery.ui.core.css",
                        //"~/Content/themes/base/jquery.ui.resizable.css",
                        //"~/Content/themes/base/jquery.ui.selectable.css",
                        //"~/Content/themes/base/jquery.ui.accordion.css",
                        //"~/Content/themes/base/jquery.ui.autocomplete.css",
                        //"~/Content/themes/base/jquery.ui.button.css",
                        //"~/Content/themes/base/jquery.ui.dialog.css",
                        //"~/Content/themes/base/jquery.ui.slider.css",
                        //"~/Content/themes/base/jquery.ui.tabs.css",
                        //"~/Content/themes/base/jquery.ui.datepicker.css",
                        //"~/Content/themes/base/jquery.ui.progressbar.css",
                        //"~/Content/themes/base/jquery.ui.theme.css"
                        ));
        }
    }
}