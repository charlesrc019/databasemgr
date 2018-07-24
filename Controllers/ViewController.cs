using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseManager.Models;

namespace DatabaseManager.Controllers
{
    public class ViewController : Controller
    {
        // VARIABLES //
        ManagementModel MgmtMdl = new ManagementModel();

        // VIEWS //
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                List<DatabaseInfo> databases = null;
                databases = MgmtMdl.GetDatabases("Databases.Owner", User.Identity.GetADUsername());
                return View("Index", databases);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [Authorize]
        public ActionResult Database(string ID)
        {
            try
            {
                // Get database details.
                DatabaseInfo database = MgmtMdl.GetDatabase(ID);

                // Verify user privileges.
                if (database.Owner != User.Identity.GetADUsername())
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "You do not have permisson to access the <strong>" + database.Name + "</strong> database.";
                    return RedirectToAction("Index", "View");
                }

                return View("Database", database);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [Authorize]
        public ActionResult Servers(string ID)
        {
            try
            {
                List<ServerInfo> serverList = null;
                serverList = MgmtMdl.GetServers("", "", User.Identity.GetADUsername());
                return View("Servers", serverList);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}