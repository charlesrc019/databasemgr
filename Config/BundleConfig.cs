using System.Web;
using System.Web.Optimization;

namespace DatabaseManager
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Javascript bundles.
            bundles.Add(new ScriptBundle("~/scripts/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/scripts/jquery.validate").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/scripts/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/scripts/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/scripts/custom_viewdatabase").Include(
                      "~/Scripts/custom_viewdatabase.js"));

            bundles.Add(new ScriptBundle("~/scripts/custom_viewserver").Include(
                      "~/Scripts/custom_viewserver.js"));

            bundles.Add(new ScriptBundle("~/scripts/custom_actioncreate").Include(
                      "~/Scripts/custom_actioncreate.js"));

            bundles.Add(new ScriptBundle("~/scripts/custom_actionchangepassword").Include(
                      "~/Scripts/custom_actionchangepassword.js"));

            bundles.Add(new ScriptBundle("~/scripts/custom_actionbackup").Include(
                      "~/Scripts/custom_actionbackup.js"));

            bundles.Add(new ScriptBundle("~/scripts/custom_actionrestore").Include(
                      "~/Scripts/custom_actionrestore.js"));

            // CSS bundles.
            bundles.Add(new StyleBundle("~/styles/css").Include(
                      "~/Styles/bootstrap.css",
                      "~/Styles/custom.css"));
        }
    }
}
