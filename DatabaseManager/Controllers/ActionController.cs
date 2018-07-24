using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using DatabaseManager.Models;
using System.Text.RegularExpressions;

namespace DatabaseManager.Controllers
{
    public class ActionController : Controller
    {
        // VARIABLES //
        ManagementModel MgmtMdl = new ManagementModel();
        InterfaceModel IntfcMdl = new InterfaceModel();

        // VIEWS //
        [Authorize]
        public virtual ActionResult Create(string ID)
        {
            try
            {
                // Preset form.
                CreateSubmission createSubmission = new CreateSubmission();
                createSubmission.DoNotSave = false;
                createSubmission.DatabasePassword = System.Web.Security.Membership.GeneratePassword(16, 2);
                createSubmission.DatabasePasswordVerify = createSubmission.DatabasePassword;

                // Fetch server info.
                ServerInfo serverInfo = MgmtMdl.GetServer(ID, User.Identity.GetADUsername());
                createSubmission.Host = serverInfo;

                return View("Create", createSubmission);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }
        [Authorize]
        public virtual ActionResult ChangeUsername(string ID)
        {
            try
            {
                // Preset form.
                ChangeUsernameSubmission changeUsernameSubmission = new ChangeUsernameSubmission();

                // Fetch database info.
                DatabaseInfo database = MgmtMdl.GetDatabase(ID);
                changeUsernameSubmission.Database = database;

                // Verify user privileges.
                if (database.Owner != User.Identity.GetADUsername())
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "You do not have permisson to access the <strong>" + database.Name + "</strong> database.";
                    return RedirectToAction("Index", "View");
                }

                // Verify database information.
                if (database.Password == "")
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "Username cannot be changed. Password is missing.";
                    return RedirectToAction("Database", "View", new { id = database.DatabaseID });
                }


                return View("ChangeUsername", changeUsernameSubmission);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [Authorize]
        public virtual ActionResult ChangePassword(string ID)
        {
            try
            {
                // Preset form.
                ChangePasswordSubmission changePasswordSubmission = new ChangePasswordSubmission();
                changePasswordSubmission.NewPassword = System.Web.Security.Membership.GeneratePassword(16, 2);
                changePasswordSubmission.NewPasswordVerify = changePasswordSubmission.NewPassword;
                changePasswordSubmission.DoNotSave = false;

                // Fetch database info.
                DatabaseInfo database = MgmtMdl.GetDatabase(ID);
                changePasswordSubmission.Database = database;

                // Verify user privileges.
                if (database.Owner != User.Identity.GetADUsername())
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "You do not have permisson to access the <strong>" + database.Name + "</strong> database.";
                    return RedirectToAction("Index", "View");
                }

                return View("ChangePassword", changePasswordSubmission);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [Authorize]
        public virtual ActionResult Backup(string ID)
        {
            try
            {
                // Preset form.
                BackupSubmission backupSubmission = new BackupSubmission();
                backupSubmission.Method = "Save";

                // Fetch database info.
                DatabaseInfo database = MgmtMdl.GetDatabase(ID);
                backupSubmission.Database = database;

                // Verify user privileges.
                if (database.Owner != User.Identity.GetADUsername())
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "You do not have permisson to access the <strong>" + database.Name + "</strong> database.";
                    return RedirectToAction("Index", "View");
                }

                return View("Backup", backupSubmission);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [Authorize]
        public virtual ActionResult Restore(string ID)
        {
            try
            {
                // Fetch database info.
                RestoreSubmission restoreSubmission = new RestoreSubmission();
                DatabaseInfo database = MgmtMdl.GetDatabase(ID);
                restoreSubmission.Database = database;

                // Preset form.
                restoreSubmission.Method = "Select";
                restoreSubmission.Backups = MgmtMdl.GetBackups(User.Identity.GetADUsername(), database.Name, database.Host, restoreSubmission.Database.Host.Protocol.ToLower());
                restoreSubmission.Backups.Insert(0, new SelectListItem { Text = "Select a Backup", Value = "default" });

                // Verify user privileges.
                if (database.Owner != User.Identity.GetADUsername())
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "You do not have permisson to access the <strong>" + database.Name + "</strong> database.";
                    return RedirectToAction("Index", "View");
                }

                return View("Restore", restoreSubmission);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [Authorize]
        public virtual ActionResult ExecuteSQL(string ID)
        {
            try
            {
                // Fetch database info.
                ExecuteSubmission executeSubmission = new ExecuteSubmission();
                DatabaseInfo database = MgmtMdl.GetDatabase(ID);
                executeSubmission.Database = database;

                // Verify user privileges.
                if (database.Owner != User.Identity.GetADUsername())
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "You do not have permisson to access the <strong>" + database.Name + "</strong> database.";
                    return RedirectToAction("Index", "View");
                }

                return View("ExecuteSQL", executeSubmission);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }



        // FUNCTIONS //
        [HttpPost]
        [Authorize]
        public virtual ActionResult Create(CreateSubmission createSubmission, string ID)
        {
            try
            {
                // Reattach server data to the submission.
                createSubmission.Host = MgmtMdl.GetServer(ID, User.Identity.GetADUsername());

                // Verify data.
                if (!ModelState.IsValid) // Invalid HTML form.
                    return View(createSubmission);
                if (createSubmission.DatabasePassword != createSubmission.DatabasePasswordVerify) // Passwords don't match.
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "Passwords do not match.";
                    return View(createSubmission);
                }
                Match PasswordVerification = Regex.Match(createSubmission.DatabasePassword, @"(?=^.{8,255}$)((?=.*\d)(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[^A-Za-z0-9])(?=.*[a-z])|(?=.*[^A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[A-Z])(?=.*[^A-Za-z0-9]))^.*", RegexOptions.ECMAScript);
                if (!PasswordVerification.Success) // Password requirements.
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "Password does not meet minimal password requirments."; 
                    return View(createSubmission);
                }
                Match DatabaseNameVerification = Regex.Match(createSubmission.DatabaseName, @"^[0-9A-Za-z_]+$");
                if (!DatabaseNameVerification.Success) // Invalid database name.
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "Database name contains invalid characters.";
                    return View(createSubmission);
                }
                Match DatabaseUserVerification = Regex.Match(createSubmission.DatabaseUser, @"^[0-9A-Za-z_]+$");
                if (!DatabaseUserVerification.Success) // Invalid database username.
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "Administrator username contains invalid characters.";
                    return View(createSubmission);
                }
                List<DatabaseInfo> databases = MgmtMdl.GetDatabases("Databases.ServerID", createSubmission.Host.ServerID);
                for (int i = 0; i < databases.Count; i++)
                {
                    if (databases[i].Name == createSubmission.DatabaseName) // Database already exists.
                    {
                        System.Web.HttpContext.Current.Session["StatusMessage"] = @"The database <strong>" + createSubmission.DatabaseName + @"</strong> already exists on this server.";
                        return View(createSubmission);
                    }
                    if (databases[i].Username == createSubmission.DatabaseUser) // Username already exists.
                    {
                        System.Web.HttpContext.Current.Session["StatusMessage"] = @"The username <strong>" + createSubmission.DatabaseUser + @"/<strong> already exists on this server.";
                        return View(createSubmission);
                    }
                }

                // Add the database info to the management database.
                if (createSubmission.DoNotSave)
                    createSubmission.DatabasePassword = "";
                string databaseID = MgmtMdl.AddDatabase(createSubmission.DatabaseName, createSubmission.Host.ServerID, User.Identity.GetADUsername(), createSubmission.DatabaseUser, createSubmission.DatabasePassword);

                // Attempt to create the database.
                try
                { IntfcMdl.Create(createSubmission); }
                catch (Exception e) // Self-clean.
                {
                    MgmtMdl.RemoveDatabase(databaseID);
                    throw e;
                }

                // Redirect and display success message.
                System.Web.HttpContext.Current.Session["StatusMessage"] = @"The database <strong>" + createSubmission.DatabaseName + @"</strong> was created on <strong>" + createSubmission.Host.Hostname + @"</strong>.";
                System.Web.HttpContext.Current.Session["StatusStyle"] = "success";
                return RedirectToAction("Database", "View", new { id = databaseID });
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [Authorize]
        public ActionResult Delete(string ID)
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

                // Attempt to delete the database.
                IntfcMdl.Delete(database);
                MgmtMdl.RemoveDatabase(ID);

                // Redirect and display success message.
                System.Web.HttpContext.Current.Session["StatusMessage"] = "The database <strong>" + database.Name + "</strong> was deleted.";
                return RedirectToAction("Index", "View");
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [Authorize]
        public ActionResult Reset(string ID)
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

                // Verify database information.
                if (database.Username == "" || database.Password == "")
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "This database cannot be reset. Username and password are missing.";
                    return RedirectToAction("Database", "View", new { id = database.DatabaseID });
                }

                // Attempt to reset the database.
                IntfcMdl.Reset(database);

                // Redirect and display success message.
                System.Web.HttpContext.Current.Session["StatusMessage"] = "The database <strong>" + database.Name + "</strong> was reset.";
                System.Web.HttpContext.Current.Session["StatusStyle"] = "warning";
                return RedirectToAction("Database", "View", new { id = database.DatabaseID });
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [HttpPost]
        [Authorize]
        public virtual ActionResult ChangeUsername(ChangeUsernameSubmission changeUsernameSubmission, string ID)
        {
            try
            {
                // Reattach database info to submission.
                changeUsernameSubmission.Database = MgmtMdl.GetDatabase(ID);

                // Verify data.
                if (!ModelState.IsValid) // Invalid HTML form.
                    return View(changeUsernameSubmission);
                List<DatabaseInfo> databases = MgmtMdl.GetDatabases("Databases.ServerID", changeUsernameSubmission.Database.Host.ServerID);
                for (int i = 0; i < databases.Count; i++)
                {
                    if (databases[i].Username == changeUsernameSubmission.NewUsername) // Username already exists.
                    {
                        System.Web.HttpContext.Current.Session["StatusMessage"] = @"The username <strong>" + changeUsernameSubmission.NewUsername + @"</strong> already exists on this server.";
                        return View(changeUsernameSubmission);
                    }
                }

                // Attempt to change the username.
                IntfcMdl.ChangeUsername(changeUsernameSubmission);
                MgmtMdl.UpdateUsername(ID, changeUsernameSubmission.NewUsername);

                // Redirect and display success message.
                System.Web.HttpContext.Current.Session["StatusMessage"] = "The username on <strong>" + changeUsernameSubmission.Database.Name + "</strong> was changed to <strong>" + changeUsernameSubmission.NewUsername + "</strong>.";
                System.Web.HttpContext.Current.Session["StatusStyle"] = "success";
                return RedirectToAction("Database", "View", new { id = changeUsernameSubmission.Database.DatabaseID });
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [HttpPost]
        [Authorize]
        public virtual ActionResult ChangePassword(ChangePasswordSubmission changePasswordSubmission, string ID)
        {
            try
            {
                // Reattach database info to submission.
                changePasswordSubmission.Database = MgmtMdl.GetDatabase(ID);

                // Verify data.
                if (!ModelState.IsValid) // Invalid HTML form.
                    return View(changePasswordSubmission);
                if (changePasswordSubmission.NewPassword != changePasswordSubmission.NewPasswordVerify) // Passwords don't match.
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "Passwords do not match.";
                    return View(changePasswordSubmission);
                }
                Match PasswordVerification = Regex.Match(changePasswordSubmission.NewPassword, @"(?=^.{8,255}$)((?=.*\d)(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[^A-Za-z0-9])(?=.*[a-z])|(?=.*[^A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[A-Z])(?=.*[^A-Za-z0-9]))^.*", RegexOptions.ECMAScript);
                if (!PasswordVerification.Success) // Password requirements.
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "New password does not meet minimal password requirments.";
                    return View(changePasswordSubmission);
                }

                // Attempt to change the username.
                IntfcMdl.ChangePassword(changePasswordSubmission);
                if (changePasswordSubmission.DoNotSave == true)
                    MgmtMdl.UpdatePassword(ID, "");
                else
                    MgmtMdl.UpdatePassword(ID, changePasswordSubmission.NewPassword);

                // Redirect and display success message.
                System.Web.HttpContext.Current.Session["StatusMessage"] = "The password to <strong>" + changePasswordSubmission.Database.Name + "</strong> was changed.";
                System.Web.HttpContext.Current.Session["StatusStyle"] = "success";
                return RedirectToAction("Database", "View", new { id = changePasswordSubmission.Database.DatabaseID });
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [HttpPost]
        [Authorize]
        public virtual ActionResult Backup(BackupSubmission backupSubmission, string ID)
        {
            try
            {
                // Reattach database info to submission.
                backupSubmission.Database = MgmtMdl.GetDatabase(ID);

                // Verify data.
                if (!ModelState.IsValid) // Invalid HTML form.
                    return View(backupSubmission);
                if (backupSubmission.Nickname != null)
                {
                    Match DatabaseNameVerification = Regex.Match(backupSubmission.Nickname, @"^[0-9A-Za-z_]+$");
                    if (!DatabaseNameVerification.Success) // Invalid database name.
                    {
                        System.Web.HttpContext.Current.Session["StatusMessage"] = "Backup nickname contains invalid characters.";
                        return View(backupSubmission);
                    }
                }

                // Compile file save location.
                string nickname = "";
                if (backupSubmission.Nickname != null)
                    nickname = "_" + backupSubmission.Nickname;
                string filename = backupSubmission.Database.Name + "_" + backupSubmission.Database.Host.Hostname.Substring(0, backupSubmission.Database.Host.Hostname.IndexOf('.')) + "_" + backupSubmission.Database.Host.Version + "_" + DateTime.Now.ToString("yyyyMMdd-HHmm") + nickname + "." + backupSubmission.Database.Host.Protocol.ToLower();
                string filepath = "";
                if (backupSubmission.Method == "Save")
                {
                    MgmtMdl.VerifyBackupDirectory(User.Identity.GetADUsername(), true);
                    filepath = @"Backups\" + User.Identity.GetADUsername() + @"\" + filename;
                }
                else
                    filepath = @"Tmp\Downloads\" + filename;

                // Attempt to backup the database.
                IntfcMdl.Backup(backupSubmission, filepath);
                if (backupSubmission.Method == "Download")
                    filename = MgmtMdl.ZipForDownload(filename);

                // Redirect and display success message.
                if (backupSubmission.Method == "Save")
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "The database <strong>" + backupSubmission.Database.Name + "</strong> was backed up to <strong>" + filename + "</strong>.";
                else
                {
                    System.Web.HttpContext.Current.Session["DownloadPath"] = "/Tmp/Downloads/" + filename;
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "The database <strong>" + backupSubmission.Database.Name + "</strong> was backed up to <a href='/Tmp/Downloads/" + filename + "' target=\"_blank\"><strong><u>" + filename + "</u></strong></a>.";
                }
                System.Web.HttpContext.Current.Session["StatusStyle"] = "success";
                return RedirectToAction("Database", "View", new { id = backupSubmission.Database.DatabaseID });
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [HttpPost]
        [Authorize]
        public virtual ActionResult Restore(RestoreSubmission restoreSubmission, string ID)
        {
            try
            {
                // Reattach database info to submission.
                restoreSubmission.Database = MgmtMdl.GetDatabase(ID);
                restoreSubmission.Backups = MgmtMdl.GetBackups(User.Identity.GetADUsername(), restoreSubmission.Database.Name, restoreSubmission.Database.Host, restoreSubmission.Database.Host.Protocol.ToLower());
                restoreSubmission.Backups.Insert(0, new SelectListItem { Text = "Select a Backup", Value = "default" });

                // Verify data.
                if (!ModelState.IsValid) // Invalid HTML form.
                    return View(restoreSubmission);
                if (restoreSubmission.Method == "Select" && restoreSubmission.FilePath == "default") // No backup selected.
                {
                    System.Web.HttpContext.Current.Session["StatusMessage"] = "No backup file selected.";
                    return View(restoreSubmission);
                }
                if (restoreSubmission.Method == "Upload")
                {
                    if (restoreSubmission.File == null) // No backup selected.
                    {
                        System.Web.HttpContext.Current.Session["StatusMessage"] = "No backup file uploaded.";
                        return View(restoreSubmission);
                    }
                    if (restoreSubmission.File.ContentLength == 0) // Empty backup selected.
                    {
                        System.Web.HttpContext.Current.Session["StatusMessage"] = "Empty file uploaded. Please select a valid " + restoreSubmission.Database.Host.Protocol + " file.";
                        return View(restoreSubmission);
                    }
                    if (Path.GetExtension(restoreSubmission.File.FileName) != ("." + restoreSubmission.Database.Host.Protocol.ToLower())) // Invalid file extension.
                    {
                        System.Web.HttpContext.Current.Session["StatusMessage"] = "Invalid file uploaded. Please select a valid " + restoreSubmission.Database.Host.Protocol + " file.";
                        return View(restoreSubmission);
                    }
                }

                // Extract backup file path.
                string filepath = "";
                if (restoreSubmission.Method == "Select")
                    filepath = @"Backups\" + User.Identity.GetADUsername() + @"\" + restoreSubmission.FilePath;

                // Save file, if needed.
                if (restoreSubmission.Method == "Upload")
                {
                    string filename = Path.GetFileName(restoreSubmission.File.FileName);
                    string savepath = Path.Combine(Server.MapPath("/Tmp/Uploads/" + filename));
                    restoreSubmission.File.SaveAs(savepath);
                    filepath = @"Tmp\Uploads\" + filename;
                }

                // Attempt to restore the database.
                IntfcMdl.Restore(restoreSubmission, filepath);

                // Redirect and display success message.
                System.Web.HttpContext.Current.Session["StatusMessage"] = "The database <strong>" + restoreSubmission.Database.Name + "</strong> was restored using <strong>" + Path.GetFileName(filepath) + "</strong>.";
                System.Web.HttpContext.Current.Session["StatusStyle"] = "success";
                return RedirectToAction("Database", "View", new { id = restoreSubmission.Database.DatabaseID });
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

        [HttpPost]
        [Authorize]
        public virtual ActionResult ExecuteSQL(ExecuteSubmission executeSubmission, string ID)
        {
            try
            {
                // Reattach database info to submission.
                executeSubmission.Database = MgmtMdl.GetDatabase(ID);

                // Verify data.
                if (!ModelState.IsValid) // Invalid HTML form.
                    return View(executeSubmission);
                for (int i = 0; i < executeSubmission.Command.Length - 1; i++)
                    if (executeSubmission.Command[i] == ';') // Nonsensical SQL command.
                    {
                        System.Web.HttpContext.Current.Session["StatusMessage"] = "Invalid SQL command.";
                        return View(executeSubmission);
                    }

                // Fix missing semicolon, if needed.
                if (executeSubmission.Command[executeSubmission.Command.Length - 1] != ';')
                    executeSubmission.Command += ";";

                // Attempt to restore the database.
                string sqlresults = IntfcMdl.Execute(executeSubmission);
                executeSubmission.Result = "<strong>sql></strong> " + executeSubmission.Command + "\n\n" + sqlresults;

                // Clear command and return results.
                return View(executeSubmission);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session["ErrorInfo"] = e.ToString();
                return RedirectToAction("Error", "View");
            }
        }

    }
}