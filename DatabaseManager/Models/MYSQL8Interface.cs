using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using MySql.Data.MySqlClient;

namespace DatabaseManager.Models
{
    // FUNCTIONS //
    public class MYSQL8Interface
    {
        // Variables.
        private MySqlConnection mysqlConnection = null;
        private ManagementModel MgmtMdl = new ManagementModel();

        // Methods.
        private void OpenConnection(ServerInfo server, string database = "")
        {
            // Create the connection string.
            string connectionString = "Server=" + server.Hostname + ";" +
                               "Port=" + server.Port + ";" +
                               "Uid=" + server.GetAdminUsername() + ";" +
                               "Pwd=" + server.GetAdminPassword() + ";";
            if (database != "")
                connectionString = connectionString + "Database=" + database + ";";

            mysqlConnection = new MySqlConnection(connectionString);

            // Attempt to connect.
            try
            { mysqlConnection.Open(); }
            catch (MySqlException e)
            {
                if (e.Number == 0)
                {
                    HttpContext.Current.Session["ErrorMessage"] = "SERVER_UNREACHABLE";
                    HttpContext.Current.Session["ErrorInfo"] = "Unable to connect to database server '" + server.Hostname + "'.";
                }
                else if (e.Number == 1045)
                { 
                    HttpContext.Current.Session["ErrorMessage"] = "INVALID_ADMIN_CREDENTIALS";
                    HttpContext.Current.Session["ErrorInfo"] = "Unable to login to '" + server.Hostname + "'.\n" +
                                                               "This is an internal error and must be resolved by the system administrator.";
                }
                else 
                {
                    HttpContext.Current.Session["ErrorMessage"] = "MYSQL_ERROR_" + e.Number;
                    HttpContext.Current.Session["ErrorInfo"] = "Connecting to the MySQL server returned the following error code.\n" +
                                                               "Search MySQL documentation for the solution.";
                }
                throw e;
            }

            // Set default error message, in case we get disconnected.
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
            if (mysqlConnection == null)
                return;

            // Close connection.
            try
            { mysqlConnection.Close(); }
            catch (MySqlException e)
            {
                    if ((string)HttpContext.Current.Session["ErrorMessage"] == "SERVER_DISCONNECTED")
                        HttpContext.Current.Session["ErrorMessage"] = "CANNOT_CLOSE_CONNECTION";
                    throw e;
            }

            // Reset values to null.
            mysqlConnection = null;
            if ((string)HttpContext.Current.Session["ErrorMessage"] == "SERVER_DISCONNECTED")
                HttpContext.Current.Session["ErrorMessage"] = null;
        }
        private void ConfigServer(ServerInfo server)
        {
            // Set error message.
            HttpContext.Current.Session["ErrorMessage"] = "SERVER_NOT_CONFIGURABLE";

            // Perform any configuration.

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
                string sql = "CREATE DATABASE " + database + ";";
                MySqlCommand cmd = new MySqlCommand(sql, mysqlConnection);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Close the connection to the server.
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
                string sql = "DROP DATABASE IF EXISTS " + database + ";";
                MySqlCommand cmd = new MySqlCommand(sql, mysqlConnection);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Close the connection to the server.
            finally
            { CloseConnection(); }
        }
        public void CreateUser(ServerInfo server, string database, string username, string password)
        {
            // Connect to the server.
            try
            { OpenConnection(server); }
            catch (Exception e)
            { throw e; }

            try
            {

                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "USER_NOT_CREATED";

                // Create the database user.
                string sql = "FLUSH PRIVILEGES;";
                MySqlCommand cmd = new MySqlCommand(sql, mysqlConnection);
                cmd.ExecuteNonQuery();

                sql = "CREATE USER '" + username + "'@'%' IDENTIFIED BY '" + password + "';";
                cmd = new MySqlCommand(sql, mysqlConnection);
                cmd.ExecuteNonQuery();

                sql = "GRANT ALL PRIVILEGES ON " + database + ".* TO '" + username + "'@'%';";
                cmd = new MySqlCommand(sql, mysqlConnection);
                HttpContext.Current.Session["ErrorInfo"] = sql;
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Close the connection to the server.
            finally
            { CloseConnection(); }
        }
        public void DeleteUser(ServerInfo server, string username)
        {
            // Connect to the server.
            try
            { OpenConnection(server); }
            catch (Exception e)
            { throw e; }

            try
            {

                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "USER_NOT_DELETED";

                // Delete the database user.
                string sql = "DROP USER IF EXISTS '" + username + "'@'%';";
                MySqlCommand cmd = new MySqlCommand(sql, mysqlConnection);
                cmd.ExecuteNonQuery();

                sql = "FLUSH PRIVILEGES;";
                cmd = new MySqlCommand(sql, mysqlConnection);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Close the connection to the server.
            finally
            { CloseConnection(); }
        }
        public void ChangePassword(ServerInfo server, string username, string new_password)
        {
            // Connect to the server.
            try
            { OpenConnection(server); }
            catch (Exception e)
            { throw e; }

            try
            {

                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "PASSWORD_NOT_CHANGED";

                // Change user password.
                string sql = "ALTER USER '" + username + "'@'%' IDENTIFIED BY '" + new_password + "';";
                MySqlCommand cmd = new MySqlCommand(sql, mysqlConnection);
                cmd.ExecuteNonQuery();

                sql = "FLUSH PRIVILEGES;";
                cmd = new MySqlCommand(sql, mysqlConnection);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            { throw e; }

            // Close the connection to the server.
            finally
            { CloseConnection(); }
        }
        public void Backup(ServerInfo server, string database, string filepath)
        {
            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "BINARY_CONNECTION_FAILED";

                // Create command for the MySQL dump.
                string command = HttpContext.Current.Server.MapPath("/bin/mysql/mysqldump.exe");
                string parameters = "-P " + server.Port + " -h " + server.Hostname + " -u " + server.GetAdminUsername() + " -p" + server.GetAdminPassword() + " " + database;
                ProcessStartInfo startInfo = new ProcessStartInfo(command, parameters);
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.ErrorDialog = false;

                // Create process for MySQL dump.
                Process process = Process.Start(startInfo);
                StreamReader output = process.StandardOutput;

                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "BACKUP_SAVE_FAILED";

                // Write results to text file.
                string fullpath = HttpContext.Current.Server.MapPath(@"~\" + filepath);
                StreamWriter file = new StreamWriter(fullpath, true);
                using (file)
                    file.Write(output.ReadToEnd());

            }
            catch (Exception e)
            { throw e; }
        }
        public void Restore(ServerInfo server, string database, string filepath)
        {
            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "FILE_NOT_FOUND";

                // Fetch file contents.
                string fullpath = HttpContext.Current.Server.MapPath(@"~\" + filepath);
                FileInfo file = new FileInfo(fullpath);
                StreamReader reader = file.OpenText();
                string sqldump = reader.ReadToEnd();
                reader.Close();

                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "BINARY_CONNECTION_FAILED";

                // Create command for the MySQL dump.
                string command = HttpContext.Current.Server.MapPath("/bin/mysql/mysql.exe");
                string parameters = "-P " + server.Port + " -h " + server.Hostname + " -u " + server.GetAdminUsername() + " -p" + server.GetAdminPassword() + " " + database;
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

                // Input SQL dump file.
                input.AutoFlush = true;
                input.Write(sqldump);
                input.Close();

                // Check for errors.
                string errortext = error.ReadToEnd();
                if (errortext.IndexOf("ERROR") != -1)
                {
                    HttpContext.Current.Session["ErrorMessage"] = "INVALID_RESTORE_FILE";
                    throw new Exception();
                }
            }
            catch (Exception e)
            { throw e; }
        }
        public string Execute(DatabaseInfo database, string sql)
        {
            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "BINARY_CONNECTION_FAILED";

                // Create command for the MySQL dump.
                string command = HttpContext.Current.Server.MapPath("/bin/mysql/mysql.exe");
                string parameters = "-P " + database.Host.Port + " -h " + database.Host.Hostname + " -u " + database.Username + " -p" + database.Password + " " + database.Name;
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
                {
                    // Remove first 'warning' line from output.
                    int newline = -1;
                    for (int i = 0; i < errortext.Length; i++)
                        if (errortext[i] == '\n')
                        {
                            newline = i;
                            break;
                        }
                    sqlresults = errortext.Substring((newline + 1), (errortext.Length - (newline + 1)));
                }
                if (sqlresults == "")
                    sqlresults = "Query complete.";
                return sqlresults;
            }
            catch (Exception e)
            { throw e; }
        }
    }
}