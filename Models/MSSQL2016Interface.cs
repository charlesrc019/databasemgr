using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace DatabaseManager.Models
{
    // VARIABLES //
    public class MSSQL2016Interface
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
                string sql = "DROP DATABASE IF EXISTS " + database + ";";
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
                SqlCommand cmd = new SqlCommand(sql, sqlserverConnection);
                cmd.ExecuteNonQuery();

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

    }
}