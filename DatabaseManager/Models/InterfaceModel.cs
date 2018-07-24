using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Security.Principal;

namespace DatabaseManager.Models
{
    // DATA STRUCTURES //
    public class CreateSubmission
    {
        public ServerInfo Host;

        [Required(ErrorMessage = "Please enter a database name.")]
        [AllowHtml]
        public string DatabaseName { get; set; }

        [Required(ErrorMessage = "Please enter a username.")]
        [AllowHtml]
        public string DatabaseUser { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [AllowHtml]
        [DataType(DataType.Password)]
        public string DatabasePassword { get; set; }

        [Required(ErrorMessage = "Please verify your password.")]
        [AllowHtml]
        [DataType(DataType.Password)]
        public string DatabasePasswordVerify { get; set; }

        [Required]
        [AllowHtml]
        public bool DoNotSave { get; set; }
    }
    public class ChangeUsernameSubmission
    {
        public DatabaseInfo Database;

        [Required(ErrorMessage = "Please enter a new username.")]
        [AllowHtml]
        public string NewUsername { get; set; }
    }
    public class ChangePasswordSubmission
    {
        public DatabaseInfo Database;

        [Required(ErrorMessage = "Please enter a new password.")]
        [AllowHtml]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please verify your new password.")]
        [AllowHtml]
        [DataType(DataType.Password)]
        public string NewPasswordVerify { get; set; }

        [Required]
        [AllowHtml]
        public bool DoNotSave { get; set; }

    }
    public class BackupSubmission
    {
        public DatabaseInfo Database;

        [AllowHtml]
        public string Nickname { get; set; }

        [Required]
        [AllowHtml]
        public string Method { get; set; }
    }
    public class RestoreSubmission
    {
        public DatabaseInfo Database;

        [AllowHtml]
        public string FilePath { get; set; }

        [Required]
        [AllowHtml]
        public string Method { get; set; }

        [AllowHtml]
        public HttpPostedFileBase File { get; set; }

        public List<SelectListItem> Backups;

    }
    public class ExecuteSubmission
    {
        public DatabaseInfo Database;

        [Required(ErrorMessage = "Please enter an SQL command.")]
        [AllowHtml]
        public string Command { get; set; }

        public string Result;
    }


    // FUNCTIONS //
    public class InterfaceModel
    {
        // Variables.
        MYSQL8Interface MYSQL8Intfc = new MYSQL8Interface();
        SQL16Interface SQL16Intfc = new SQL16Interface();

        // Methods.
        public void Create(CreateSubmission createSubmission)
        {
            try
            {
                // Perform the database and user creation, based on protocol type.
                if (createSubmission.Host.Protocol == "MYSQL8")
                {
                    MYSQL8Intfc.CreateDatabase(createSubmission.Host, createSubmission.DatabaseName);
                    MYSQL8Intfc.CreateUser(createSubmission.Host, createSubmission.DatabaseName, createSubmission.DatabaseUser, createSubmission.DatabasePassword);
                }
                else if (createSubmission.Host.Protocol == "SQL16")
                {
                    SQL16Intfc.CreateDatabase(createSubmission.Host, createSubmission.DatabaseName);
                    SQL16Intfc.CreateUser(createSubmission.Host, createSubmission.DatabaseName, createSubmission.DatabaseUser, createSubmission.DatabasePassword);
                }

                // Throw exception if not found.
                else
                {
                    System.Web.HttpContext.Current.Session["ErrorMessage"] = "PROTOCOL_NOT_FOUND";
                    throw new Exception();
                }
            }
            catch (Exception e)
            { throw e; }
        }
        public void Delete(DatabaseInfo database)
        {
            try
            {
                // Perform the database delete, based on protocol type.
                if (database.Host.Protocol == "MYSQL8")
                {
                    if (database.Username != "") // Only delete a user if we have one listed.
                        MYSQL8Intfc.DeleteUser(database.Host, database.Username);
                    MYSQL8Intfc.DeleteDatabase(database.Host, database.Name);
                }
                else if (database.Host.Protocol == "SQL16")
                {
                    SQL16Intfc.DeleteDatabase(database.Host, database.Name); // Will take users with it.
                }

                // Throw exception if not found.
                else
                {
                    System.Web.HttpContext.Current.Session["ErrorMessage"] = "PROTOCOL_NOT_FOUND";
                    throw new Exception();
                }
            }
            catch (Exception e)
            { throw e; }
        }
        public void Reset(DatabaseInfo database)
        {
            try
            {
                // Perform the database reset, based on protocol type.
                if (database.Host.Protocol == "MYSQL8")
                {
                    MYSQL8Intfc.DeleteUser(database.Host, database.Username);
                    MYSQL8Intfc.DeleteDatabase(database.Host, database.Name);
                    MYSQL8Intfc.CreateDatabase(database.Host, database.Name);
                    MYSQL8Intfc.CreateUser(database.Host, database.Name, database.Username, database.Password);
                }
                else if (database.Host.Protocol == "SQL16")
                {
                    SQL16Intfc.DeleteDatabase(database.Host, database.Name); // Will take users with it.
                    SQL16Intfc.CreateDatabase(database.Host, database.Name);
                    SQL16Intfc.CreateUser(database.Host, database.Name, database.Username, database.Password);
                }

                // Throw exception if not found.
                else
                {
                    HttpContext.Current.Session["ErrorMessage"] = "PROTOCOL_NOT_FOUND";
                    throw new Exception();
                }
            }
            catch (Exception e)
            { throw e; }
        }
        public void ChangeUsername(ChangeUsernameSubmission changeUsernameSubmission)
        {
            try
            {
                // Delete user and create new one.
                if (changeUsernameSubmission.Database.Host.Protocol == "MYSQL8")
                {
                    MYSQL8Intfc.DeleteUser(changeUsernameSubmission.Database.Host, changeUsernameSubmission.Database.Username);
                    MYSQL8Intfc.CreateUser(changeUsernameSubmission.Database.Host, changeUsernameSubmission.Database.Name, changeUsernameSubmission.NewUsername, changeUsernameSubmission.Database.Password);
                }
                else if (changeUsernameSubmission.Database.Host.Protocol == "SQL16")
                {
                    SQL16Intfc.DeleteUser(changeUsernameSubmission.Database.Host, changeUsernameSubmission.Database.Name, changeUsernameSubmission.Database.Username);
                    SQL16Intfc.CreateUser(changeUsernameSubmission.Database.Host, changeUsernameSubmission.Database.Name, changeUsernameSubmission.NewUsername, changeUsernameSubmission.Database.Password);
                }

                // Throw exception if not found.
                else
                {
                    HttpContext.Current.Session["ErrorMessage"] = "PROTOCOL_NOT_FOUND";
                    throw new Exception();
                }
            }
            catch (Exception e)
            { throw e; }
        }
        public void ChangePassword(ChangePasswordSubmission changePasswordSubmission)
        {
            try
            {
                // Delete user and create new one.
                if (changePasswordSubmission.Database.Host.Protocol == "MYSQL8")
                {
                    MYSQL8Intfc.ChangePassword(changePasswordSubmission.Database.Host, changePasswordSubmission.Database.Username, changePasswordSubmission.NewPassword);
                }
                else if (changePasswordSubmission.Database.Host.Protocol == "SQL16")
                {
                    SQL16Intfc.ChangePassword(changePasswordSubmission.Database.Host, changePasswordSubmission.Database.Name, changePasswordSubmission.Database.Username, changePasswordSubmission.NewPassword);
                }

                // Throw exception if not found.
                else
                {
                    HttpContext.Current.Session["ErrorMessage"] = "PROTOCOL_NOT_FOUND";
                    throw new Exception();
                }
            }
            catch (Exception e)
            { throw e; }
        }
        public void Backup(BackupSubmission backupSubmission, string filepath)
        {
            try
            {
                // Backup databse.
                if (backupSubmission.Database.Host.Protocol == "MYSQL8")
                {
                    MYSQL8Intfc.Backup(backupSubmission.Database.Host, backupSubmission.Database.Name, filepath);
                }
                else if (backupSubmission.Database.Host.Protocol == "SQL16")
                {
                    SQL16Intfc.Backup(backupSubmission.Database.Host, backupSubmission.Database.Name, filepath);
                }

                // Throw exception if not found.
                else
                {
                    HttpContext.Current.Session["ErrorMessage"] = "PROTOCOL_NOT_FOUND";
                    throw new Exception();
                }
            }
            catch (Exception e)
            { throw e; }
        }
        public void Restore(RestoreSubmission restoreSubmission, string filepath)
        {
            try
            {
                // Backup databse.
                if (restoreSubmission.Database.Host.Protocol == "MYSQL8")
                {
                    MYSQL8Intfc.Restore(restoreSubmission.Database.Host, restoreSubmission.Database.Name, filepath);
                }
                else if (restoreSubmission.Database.Host.Protocol == "SQL16")
                {
                    SQL16Intfc.Restore(restoreSubmission.Database.Host, restoreSubmission.Database.Name, filepath);
                }

                // Throw exception if not found.
                else
                {
                    HttpContext.Current.Session["ErrorMessage"] = "PROTOCOL_NOT_FOUND";
                    throw new Exception();
                }
            }
            catch (Exception e)
            { throw e; }
        }
        public string Execute(ExecuteSubmission executeSubmission)
        {
            try
            {
                // Execute SQL on databse.
                if (executeSubmission.Database.Host.Protocol == "MYSQL8")
                {
                    return MYSQL8Intfc.Execute(executeSubmission.Database, executeSubmission.Command);
                }
                else if (executeSubmission.Database.Host.Protocol == "SQL16")
                {
                    return SQL16Intfc.Execute(executeSubmission.Database, executeSubmission.Command);
                }

                // Throw exception if not found.
                else
                {
                    HttpContext.Current.Session["ErrorMessage"] = "PROTOCOL_NOT_FOUND";
                    throw new Exception();
                }
            }
            catch (Exception e)
            { throw e; }
        }

    }
}