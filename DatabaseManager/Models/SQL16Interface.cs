using System;
using System.Web;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace DatabaseManager.Models
{
    // VARIABLES //
    public class SQL16Interface
    {
        // Variables.
        private SqlConnection sqlserverConnection = null;
        private ManagementModel MgmtMdl = new ManagementModel();

        // Methods.
        private void OpenConnection(ServerInfo server, string database = "")
        {           
            // Create connection string.
            string connectionString = "Server=" + server.Hostname + "," + server.Port + ";" +
                                      "User Id=" + server.GetAdminUsername() + ";" +
                                      "Password=" + server.GetAdminPassword() + ";";
            if (database != "")
                connectionString = connectionString + "Database=" + database + ";";
            sqlserverConnection = new SqlConnection(connectionString);

            // Attempt to open database connection.
            try
            { sqlserverConnection.Open(); }
            catch (Exception e)
            {
                HttpContext.Current.Session["ErrorMessage"] = "SERVER_UNREACHABLE";
                HttpContext.Current.Session["ErrorInfo"] = "Unable to connect to database server '" + server.Hostname + "'.";
                throw e;
            }

            // Set error message, in case we get disconnected.
            HttpContext.Current.Session["ErrorMessage"] = "SERVER_DISCONNECTED";

            // Check that the server has been configured.
            if (server.NewToDbMgr != "0")
            {
                try
                { ConfigServer(server); }
                catch (Exception e)
                {
                    CloseConnection();
                    throw e;
                }
            }
        }
        private void CloseConnection()
        {
            // Don't close if already closed.
            if (sqlserverConnection == null)
                return;

            // Close connection.
            try
            {
                SqlConnection.ClearPool(sqlserverConnection);
                sqlserverConnection.Close();
            }
            catch (Exception e)
            {
                if ((string)HttpContext.Current.Session["ErrorMessage"] == "SERVER_DISCONNECTED")
                    HttpContext.Current.Session["ErrorMessage"] = "CANNOT_CLOSE_CONNECTION";
                throw e;
            }

            // Reset values to null.
            sqlserverConnection = null;
            if ((string)HttpContext.Current.Session["ErrorMessage"] == "SERVER_DISCONNECTED")
                HttpContext.Current.Session["ErrorMessage"] = null;
        }
        private void ConfigServer(ServerInfo server)
        {
            // Set error message.
            HttpContext.Current.Session["ErrorMessage"] = "SERVER_NOT_CONFIGURABLE";

            // Configure for contained databases.
            string sql = "sp_configure 'show advanced options',1";
            SqlCommand cmd1 = new SqlCommand(sql, sqlserverConnection);
            sql = "sp_configure 'contained database authentication', 1";
            SqlCommand cmd2 = new SqlCommand(sql, sqlserverConnection);
            sql = "RECONFIGURE WITH OVERRIDE";
            SqlCommand cmdReconfigure = new SqlCommand(sql, sqlserverConnection);
            try
            {
                cmd1.ExecuteNonQuery();
                cmdReconfigure.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();
                cmdReconfigure.ExecuteNonQuery();
            }
            catch (Exception e)
            { throw e; }

            // Make configuration complete.
            MgmtMdl.UpdateNewToDbMgr(server.ServerID);
        }
        public void CreateDatabase(ServerInfo server, string database)
        {          
            // Connect to the server.
            try
            { OpenConnection(server); }
            catch (Exception e)
            { throw e; }

            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "DATABASE_NOT_CREATED";

                // Create the database.
                string sql = "CREATE DATABASE " + database + " CONTAINMENT = PARTIAL;";
                SqlCommand cmd = new SqlCommand(sql, sqlserverConnection);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Close connection to server.
            finally
            { CloseConnection(); }
        }
        public void DeleteDatabase(ServerInfo server, string database)
        {
            // Connect to the server.
            try
            { OpenConnection(server); }
            catch (Exception e)
            { throw e; }

            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "DATABASE_NOT_DELETED";

                // Delete the database.
                string sql = "DROP DATABASE " + database + ";";
                SqlCommand cmd = new SqlCommand(sql, sqlserverConnection);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Disconnect from server.
            finally
            { CloseConnection(); }
        }
        public void CreateUser(ServerInfo server, string database, string username, string password)
        {
            // Connect to the server.
            try
            { OpenConnection(server, database); }
            catch (Exception e)
            { throw e; }

            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "USER_NOT_CREATED";

                // Create the database user.
                string sql = "CREATE USER " + username + " WITH PASSWORD = '" + password + "';";
                SqlCommand cmd1 = new SqlCommand(sql, sqlserverConnection);
                sql = "EXEC sp_addrolemember 'db_owner','" + username + "';";
                SqlCommand cmd2 = new SqlCommand(sql, sqlserverConnection);
                cmd1.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Disconnect from server.
            finally
            { CloseConnection(); }
        }
        public void DeleteUser(ServerInfo server, string database, string username)
        {
            // Connect to the server.
            try
            { OpenConnection(server, database); }
            catch (Exception e)
            { throw e; }

            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "USER_NOT_DELETED";

                // Delete the database user.
                string sql = "DROP USER IF EXISTS " + username + ";";
                SqlCommand cmd = new SqlCommand(sql, sqlserverConnection);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Disconnect from server.
            finally
            { CloseConnection(); }
        }
        public void ChangePassword(ServerInfo server, string database, string username, string new_password)
        {
            // Connect to the server.
            try
            { OpenConnection(server, database); }
            catch (Exception e)
            { throw e; }

            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "PASSWORD_NOT_CHANGED";

                // Delete the database user.
                string sql = "ALTER USER " + username + " WITH PASSWORD = '" + new_password + "';";
                SqlCommand cmd = new SqlCommand(sql, sqlserverConnection);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Disconnect from server.
            finally
            { CloseConnection(); }
        }
        public void Backup(ServerInfo server, string database, string filepath)
        {
            // Connect to the server.
            try
            { OpenConnection(server); }
            catch (Exception e)
            { throw e; }

            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "BACKUP_FAILED";

                // Delete the database user.
                string sql = "BACKUP DATABASE " + database + @" TO DISK='\\" + System.Net.Dns.GetHostEntry("LocalHost").HostName + @"\" + filepath + "';";
                SqlCommand cmd = new SqlCommand(sql, sqlserverConnection);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Disconnect from server.
            finally
            { CloseConnection(); }
        }
        public void Restore(ServerInfo server, string database, string filepath)
        {
            // Connect to the server.
            try
            { OpenConnection(server); }
            catch (Exception e)
            { throw e; }

            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "RESTORE_FAILED";

                // Delete the database user.
                string sql = "RESTORE DATABASE " + database + @" FROM DISK='\\" + System.Net.Dns.GetHostEntry("LocalHost").HostName + @"\" + filepath + "' WITH REPLACE;";
                SqlCommand cmd = new SqlCommand(sql, sqlserverConnection);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Disconnect from server.
            finally
            { CloseConnection(); }
        }
        public string Execute(DatabaseInfo database, string sql)
        {
            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "BINARY_CONNECTION_FAILED";

                // Create command for the MySQL dump.
                string command = @"sqlcmd";
                string parameters = "/S " + database.Host.Hostname + "," + database.Host.Port + " /d " + database.Name + " -U " + database.Username + " -P " + database.Password + " -Q \"" + sql + "\" -y 15 -Y 15";
                ProcessStartInfo startInfo = new ProcessStartInfo(command, parameters);
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput = true;
                startInfo.UseShellExecute = false;
                startInfo.ErrorDialog = false;

                // Create process for MySQL restore.
                Process process = Process.Start(startInfo);
                StreamReader error = process.StandardError;
                StreamWriter input = process.StandardInput;
                StreamReader output = process.StandardOutput;

                // Input SQL.
                input.AutoFlush = true;
                input.Write(sql);
                input.Close();

                // Return results.
                string outputtext = output.ReadToEnd();
                string errortext = error.ReadToEnd();
                string sqlresults = "";
                if (outputtext.Length > 0)
                    sqlresults = outputtext;
                else
                    sqlresults = errortext;
                if (sqlresults == "")
                    sqlresults = "Query complete.";

                // Add username mismatch message.
                if (sqlresults.IndexOf("Login failed") != -1)
                    sqlresults += "There is a mismatch between the database username and the username stored in DatabaseMgr.\nTry using the 'Change Username' feature to restore functionality.\nIf the problem persists, contact a system adminstrator.";
    
                return sqlresults;
            }
            catch (Exception e)
            { throw e; }

        }
    }
}