using System;
using System.DirectoryServices.AccountManagement;
using System.Collections.Generic;
using Microsoft.Owin.Security;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.Data.SQLite.EF6;
using System.ComponentModel.DataAnnotations;
using DatabaseManager.Controllers;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Web.Mvc;
using System.Runtime.InteropServices;
using System.Text;


namespace DatabaseManager.Models
{
    // DATA STRUCTURES //
    public class DatabaseInfo
    {
        public string DatabaseID;
        public string Name;
        public string Owner;
        public string Username;
        public string Password;
        public string Created;
        public ServerInfo Host;

        public DatabaseInfo(string _databaseid, string _name, string _owner, string _username, string _password, string _created)
        {
            DatabaseID = _databaseid;
            Name = _name;
            Owner = _owner;
            Username = _username;
            Password = _password;
            Created = _created;
        }
    }
    public class DatabaseInfoLite
    {
        public string DatabaseID;
        public string Name;

        public DatabaseInfoLite(string _databaseid, string _name)
        {
            DatabaseID = _databaseid;
            Name = _name;
        }
    }
    public class ServerInfo
    {
        public string ServerID;
        public string Hostname;
        public string Port;
        public string ProtocolID;
        public string Protocol;
        public string Type;
        public string Developer;
        public string Version;
        public string NewToDbMgr;
        private string AdminUsername;
        private string AdminPassword;
        public string Location;
        public int TotalDatabases;
        public List<DatabaseInfoLite> YourDatabases;

        public ServerInfo(string _serverid, string _hostname, string _port, string _protocolID, string _protocol, string _type, string _developer, string _version, string _newtodbmgr, string _adminusername, string _adminpassword, string _location)
        {
            ServerID = _serverid;
            Hostname = _hostname;
            Port = _port;
            ProtocolID = _protocolID;
            Protocol = _protocol;
            Type = _type;
            Developer = _developer;
            Version = _version;
            NewToDbMgr = _newtodbmgr;
            AdminUsername = _adminusername;
            AdminPassword = _adminpassword;
            Location = _location;
            TotalDatabases = 0;
            YourDatabases = new List<DatabaseInfoLite>();
        }

        public string GetAdminUsername()
        {
            return AdminUsername;
        }

        public string GetAdminPassword()
        {
            return AdminPassword;
        }
    }

    // FUNCTIONS //
    public class ManagementModel
    {
        // Variables.
        private SQLiteConnection sqliteConnection = null;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName(
                           [MarshalAs(UnmanagedType.LPTStr)]
                           string path,
                           [MarshalAs(UnmanagedType.LPTStr)]
                           StringBuilder shortPath,
                           int shortPathLength
                           );

        // Methods.
        private void OpenConnection()
        {
            // Set address to management database.
            sqliteConnection = new SQLiteConnection(@"Data Source=" + HttpContext.Current.Server.MapPath("/Data/DatabaseMgr.sqlite3") + ";Version=3;");

            // Attempt to open the management database.
            try
            { sqliteConnection.Open(); }

            // Error. Cannot open connection to the SQLite database.
            catch (Exception e)
            {
                HttpContext.Current.Session["ErrorMessage"] = "MANAGEMENT_NOT_OPENED_FROM_" + HttpContext.Current.Server.MapPath("/Data/DatabaseMgr.sqlite3");
                throw e;
            }

            // Set error message, in case we get disconnected.
            HttpContext.Current.Session["ErrorMessage"] = "MANAGEMENT_DISCONNECTED";
        }
        private void CloseConnection()
        {
            try
            { sqliteConnection.Close(); }
            catch (Exception e)
            {
                HttpContext.Current.Session["ErrorMessage"] = "MANAGEMENT_NOT_CLOSED";
                throw e;
            }
            HttpContext.Current.Session["ErrorMessage"] = null;
        }
        public DatabaseInfo GetDatabase(string databaseID)
        {
            // Compile SQL query and variables.
            string sql = "SELECT Databases.Name, Databases.ServerID, Databases.Owner, Databases.Username, Databases.Password, Databases.Created " +
                         "FROM Databases " +
                         "WHERE Databases.DatabaseID = '" + databaseID + "'" +
                         "ORDER BY Databases.Name ASC;";
            string _name, serverID, _owner, _username, _password, _created;
            _name = serverID = _owner = _username = _password = _created = "";


            // Open connection.
            try
            { OpenConnection(); }
            catch (Exception e)
            { throw e; }

            // Execute SQL query.
            SQLiteCommand command = new SQLiteCommand(sql, sqliteConnection);
            SQLiteDataReader results = command.ExecuteReader();

            // Read SQL results.
            int rows_returned = 0;
            while (results.Read())
            {
                _name = Convert.ToString(results["Name"]);
                serverID = Convert.ToString(results["ServerID"]);
                _owner = Convert.ToString(results["Owner"]);
                _username = Convert.ToString(results["Username"]);
                _password = Convert.ToString(results["Password"]);
                _created = Convert.ToString(results["Created"]);
                rows_returned++;
            }

            // Close connection.
            try
            { CloseConnection(); }
            catch (Exception e)
            { throw e; }

            // Catch errors.
            if (rows_returned < 1)
            {
                HttpContext.Current.Session["ErrorMessage"] = "DATABASE_NOT_FOUND";
                throw new Exception();
            }
            if (rows_returned > 1)
            {
                HttpContext.Current.Session["ErrorMessage"] = "MULTIPLE_DATABASES";
                throw new Exception();
            }

            // Create database.
            DatabaseInfo database = new DatabaseInfo(databaseID, _name, _owner, _username, _password, _created);

            // Add server info.
            try
            { database.Host = GetServer(serverID); }
            catch (Exception e)
            { throw e; }

            return database;
        }
        public List<DatabaseInfo> GetDatabases(string column = "", string condition = "")
        {
            // Get server infomation.
            List<ServerInfo> servers = new List<ServerInfo>();
            try
            { servers = GetServers(); }
            catch (Exception e)
            { throw e; }

            // Compile SQL query and variables.
            string where_statement = "";
            if (column != "")
                where_statement = "WHERE " + column + " = '" + condition + "' ";

            string sql = "SELECT Databases.DatabaseID, Databases.Name, Databases.ServerID, Databases.Owner, Databases.Username, Databases.Password, Databases.Created " +
                         "FROM Databases " +
                         "INNER JOIN Servers ON Databases.ServerID = Servers.ServerID " +
                         "INNER JOIN Protocols ON Servers.ProtocolID = Protocols.ProtocolID " +
                         where_statement +
                         "ORDER BY Databases.Name ASC;";

            List<DatabaseInfo> databases = new List<DatabaseInfo>();

            // Open connnection.
            try
            { OpenConnection(); }
            catch (Exception e)
            { throw e; }

            // Execute SQL query.
            SQLiteCommand command = new SQLiteCommand(sql, sqliteConnection);
            SQLiteDataReader results = command.ExecuteReader();

            // Read SQL results.
            while (results.Read())
            {
                string _databaseID = Convert.ToString(results["DatabaseID"]);
                string _name = Convert.ToString(results["Name"]);
                string _owner = Convert.ToString(results["Owner"]);
                string serverID = Convert.ToString(results["ServerID"]);
                string _username = Convert.ToString(results["Username"]);
                string _password = Convert.ToString(results["Password"]);
                string _created = Convert.ToString(results["Created"]);

                databases.Add(new DatabaseInfo(_databaseID, _name, _owner, _username, _password, _created));
                for (int i = 0; i < servers.Count; i++)
                    if (serverID == servers[i].ServerID)
                        databases[databases.Count - 1].Host = servers[i];
            }

            // Close connection.
            try
            { CloseConnection(); }
            catch (Exception e)
            { throw e; }

            return databases;
        }
        public ServerInfo GetServer(string serverID, string username = "")
        {
            // Compile SQL query and variables.
            string sql = "SELECT Servers.Hostname, Servers.Port, Protocols.ProtocolID, Protocols.Protocol, Protocols.Type, Protocols.Developer , Servers.Version, Servers.NewToDbMgr, Servers.AdminUsername, Servers.AdminPassword, Servers.Location " +
                    "FROM Servers " +
                    "INNER JOIN Protocols ON Servers.ProtocolID = Protocols.ProtocolID " +
                    "WHERE Servers.ServerID = '" + serverID + "';";
            string _hostname, _port, _protocolID, _protocol, _type, _developer, _version, _newtodbmgr, _adminusername, _adminpassword, _location;
            _hostname = _port = _protocolID = _protocol = _type = _developer = _version = _newtodbmgr = _adminusername = _adminpassword = _location = "";

            // Open connnection.
            try
            { OpenConnection(); }
            catch (Exception e)
            { throw e; }

            // Execute SQL query.
            SQLiteCommand command = new SQLiteCommand(sql, sqliteConnection);
            SQLiteDataReader results = command.ExecuteReader();

            // Read SQL results.
            int rows_returned = 0;
            while (results.Read())
            {
                _hostname = Convert.ToString(results["Hostname"]);
                _port = Convert.ToString(results["Port"]);
                _protocolID = Convert.ToString(results["ProtocolID"]);
                _protocol = Convert.ToString(results["Protocol"]);
                _type = Convert.ToString(results["Type"]);
                _developer = Convert.ToString(results["Developer"]);
                _version = Convert.ToString(results["Version"]);
                _newtodbmgr = Convert.ToString(results["NewToDbMgr"]);
                _adminusername = Convert.ToString(results["AdminUsername"]);
                _adminpassword = Convert.ToString(results["AdminPassword"]);
                _location = Convert.ToString(results["Location"]);
                rows_returned++;
            }

            // Close connection.
            try
            { CloseConnection(); }
            catch (Exception e)
            { throw e; }

            // Catch errors.
            if (rows_returned < 1)
            {
                HttpContext.Current.Session["ErrorMessage"] = "SERVER_NOT_FOUND";
                throw new Exception();
            }
            else if (rows_returned > 1)
            {
                HttpContext.Current.Session["ErrorMessage"] = "MULTIPLE_SERVERS";
                throw new Exception();
            }

            // Create basic server info.
            ServerInfo server = new ServerInfo(serverID, _hostname, _port, _protocolID, _protocol, _type, _developer, _version, _newtodbmgr, _adminusername, _adminpassword, _location);

            // Add advanced server info.
            List<DatabaseInfo> databases = new List<DatabaseInfo>();
            try
            { databases = GetDatabases("Databases.ServerID", serverID); }
            catch (Exception e)
            { throw e; }

            server.TotalDatabases = databases.Count;
            if (username != "")
                for (int i = 0; i < databases.Count; i++)
                    if (databases[i].Owner == username)
                        server.YourDatabases.Add(new DatabaseInfoLite(databases[i].DatabaseID, databases[i].Name));


            return server;
        }
        public List<ServerInfo> GetServers(string column = "", string condition = "", string username = "")
        {
            // Compile SQL query and variables.
            string where_statement = "";
            if (column != "")
                where_statement = "WHERE " + column + " = '" + condition + "' ";

            string sql = "SELECT Servers.ServerID, Servers.Hostname, Servers.Port, Protocols.ProtocolID, Protocols.Protocol, Protocols.Type, Protocols.Developer , Servers.Version, Servers.NewToDbMgr, Servers.AdminUsername, Servers.AdminPassword, Servers.Location " +
                         "FROM Servers " +
                         "INNER JOIN Protocols ON Servers.ProtocolID = Protocols.ProtocolID " +
                         where_statement +
                         "ORDER BY Servers.Hostname ASC;";

            List<ServerInfo> servers = new List<ServerInfo>();

            // Open connnection.
            try
            { OpenConnection(); }
            catch (Exception e)
            { throw e; }

            // Execute SQL query.
            SQLiteCommand command = new SQLiteCommand(sql, sqliteConnection);
            SQLiteDataReader results = command.ExecuteReader();

            // Read SQL results.
            while (results.Read())
            {
                string _serverID = Convert.ToString(results["ServerID"]);
                string _hostname = Convert.ToString(results["Hostname"]);
                string _port = Convert.ToString(results["Port"]);
                string _protocolID = Convert.ToString(results["ProtocolID"]);
                string _protocol = Convert.ToString(results["Protocol"]);
                string _type = Convert.ToString(results["Type"]);
                string _developer = Convert.ToString(results["Developer"]);
                string _newtodbmgr = Convert.ToString(results["NewToDbMgr"]);
                string _version = Convert.ToString(results["Version"]);
                string _adminusername = Convert.ToString(results["AdminUsername"]);
                string _adminpassword = Convert.ToString(results["AdminPassword"]);
                string _location = Convert.ToString(results["Location"]);

                servers.Add(new ServerInfo(_serverID, _hostname, _port, _protocolID, _protocol, _type, _developer, _version, _newtodbmgr, _adminusername, _adminpassword, _location));
            }

            // Close connection.
            try
            { CloseConnection(); }
            catch (Exception e)
            { throw e; }

            // Add advanced server info.
            if (username != "")
            {
                List<DatabaseInfo> databases = new List<DatabaseInfo>();
                try
                { databases = GetDatabases(); }
                catch (Exception e)
                { throw e; }

                for (int i = 0; i < servers.Count; i++)
                {
                    int _totaldatabases = 0;
                    for (int j = 0; j < databases.Count; j++)
                        if (databases[j].Host.ServerID == servers[i].ServerID)
                        {
                            _totaldatabases++;
                            if (databases[j].Owner == username)
                                servers[i].YourDatabases.Add(new DatabaseInfoLite(databases[j].DatabaseID, databases[j].Name));
                        }
                    servers[i].TotalDatabases = _totaldatabases;
                }
            }

            return servers;
        }
        public void UpdateNewToDbMgr(string serverID)
        {
            // Compile SQL query.
            string sql = "UPDATE Servers " +
                         "SET NewToDbMgr='0' " +
                         "WHERE ServerID='" + serverID + "';";

            // Open connnection.
            try
            { OpenConnection(); }
            catch (Exception e)
            { throw e; }

            // Set error message.
            HttpContext.Current.Session["ErrorMessage"] = "MANAGEMENT_ENTRY_NOT_UPDATED";

            // Execute SQL query.
            SQLiteCommand command = new SQLiteCommand(sql, sqliteConnection);
            command.ExecuteNonQuery();

            // Close connection.
            try
            { CloseConnection(); }
            catch (Exception e)
            { throw e; }
        }
        public string AddDatabase(string name, string serverID, string owner, string username, string password)
        {
            // Compile SQL insert query.
            string sql = "INSERT INTO Databases (Name, ServerID, Owner, Username, Password) " +
                         "VALUES ('" + name + "', '" + serverID + "', '" + owner + "', '" + username + "', '" + password + "');";

            // Open connnection.
            try
            { OpenConnection(); }
            catch (Exception e)
            { throw e; }

            // Set error message.
            HttpContext.Current.Session["ErrorMessage"] = "MANAGEMENT_ENTRY_NOT_ADDED";

            // Execute SQL query.
            SQLiteCommand command = new SQLiteCommand(sql, sqliteConnection);
            command.ExecuteNonQuery();

            // Compile select query.
            sql = "SELECT Databases.DatabaseID " +
                  "FROM Databases " +
                  "WHERE Databases.Name = '" + name + "' AND Databases.ServerID = '" + serverID + "' " +
                  "ORDER BY Databases.Name ASC;";

            // Execute select query.
            command = new SQLiteCommand(sql, sqliteConnection);
            SQLiteDataReader results = command.ExecuteReader();
            string databaseID = "";
            while (results.Read())
                databaseID = Convert.ToString(results["DatabaseID"]);

            // Close connection.
            try
            { CloseConnection(); }
            catch (Exception e)
            { throw e; }

            return databaseID;
        }
        public void RemoveDatabase(string databaseID)
        {
            // Compile SQL query.
            string sql = "DELETE FROM Databases " +
                         "WHERE DatabaseID='" + databaseID + "';";

            // Open connnection.
            try
            { OpenConnection(); }
            catch (Exception e)
            { throw e; }

            // Set error message.
            HttpContext.Current.Session["ErrorMessage"] = "MANAGEMENT_ENTRY_NOT_REMOVED";

            // Execute SQL query.
            SQLiteCommand command = new SQLiteCommand(sql, sqliteConnection);
            command.ExecuteNonQuery();

            // Close connection.
            try
            { CloseConnection(); }
            catch (Exception e)
            { throw e; }
        }
        public void UpdateUsername(string databaseID, string new_username)
        {
            // Compile SQL query.
            string sql = "UPDATE Databases " +
                         "SET Username='" + new_username + "' " +
                         "WHERE DatabaseID='" + databaseID + "';";

            // Open connnection.
            try
            { OpenConnection(); }
            catch (Exception e)
            { throw e; }

            // Set error message.
            HttpContext.Current.Session["ErrorMessage"] = "MANAGEMENT_ENTRY_NOT_UPDATED";

            // Execute SQL query.
            SQLiteCommand command = new SQLiteCommand(sql, sqliteConnection);
            command.ExecuteNonQuery();

            // Close connection.
            try
            { CloseConnection(); }
            catch (Exception e)
            { throw e; }
        }
        public void UpdatePassword(string databaseID, string new_password)
        {
            // Compile SQL query.
            string sql = "UPDATE Databases " +
                         "SET Password='" + new_password + "' " +
                         "WHERE DatabaseID='" + databaseID + "';";

            // Open connnection.
            try
            { OpenConnection(); }
            catch (Exception e)
            { throw e; }

            // Set error message.
            HttpContext.Current.Session["ErrorMessage"] = "MANAGEMENT_ENTRY_NOT_UPDATED";

            // Execute SQL query.
            SQLiteCommand command = new SQLiteCommand(sql, sqliteConnection);
            command.ExecuteNonQuery();

            // Close connection.
            try
            { CloseConnection(); }
            catch (Exception e)
            { throw e; }
        }
        public List<SelectListItem> GetBackups(string username, string database, ServerInfo server, string extension)
        {
            // Don't check if the directory doesn't exist.
            if (!VerifyBackupDirectory(username, false))
                return new List<SelectListItem>();

            // Fetch data for the list.
            string[] backuppaths = Directory.GetFiles(HttpContext.Current.Server.MapPath("/Backups/" + username), "*." + extension);
            List<SelectListItem> backupfiles = new List<SelectListItem>();
            List<DatabaseInfo> databases = GetDatabases("Databases.ServerID", server.ServerID);
            string shortservername = server.Hostname.Substring(0, server.Hostname.IndexOf('.'));

            // Compile a list of compatible backups.
            for (int i = 0; i < backuppaths.Length; i++)
            {
                string filename = Path.GetFileName(backuppaths[i]);

                // All MySQL files are compatible.
                if (extension.Equals("mysql", StringComparison.InvariantCultureIgnoreCase))
                {
                    backupfiles.Add(new SelectListItem { Text = filename, Value = filename });
                    continue;
                }
                // All backups of the exact same database are compatible.
                if (filename.Contains(database + "_" + shortservername + "_"))
                {
                    backupfiles.Add(new SelectListItem { Text = filename, Value = filename });
                    continue;
                }
                // Backups of old databases are compatible??????
                if (filename.Contains(shortservername))
                {
                    bool nomatch = true;
                    for (int j = 0; j < databases.Count; j++)
                        if (filename.Substring(0, filename.IndexOf('_')) == databases[j].Name)
                            nomatch = false;

                    if (nomatch)
                    {
                        backupfiles.Add(new SelectListItem { Text = filename, Value = filename });
                        continue;
                    }
                }
                // Backups from similar versions are compatible, if the server is different.
                if (filename.Contains("_" + server.Version + "_") && !filename.Contains("_" + shortservername + "_"))
                {
                    backupfiles.Add(new SelectListItem { Text = filename, Value = filename });
                    continue;
                }
            }

            return backupfiles;
        }
        public bool VerifyBackupDirectory(string username, bool createoption)
        {
            if (Directory.Exists(HttpContext.Current.Server.MapPath("/Backups/" + username)))
                return true;
            else
            {
                if (createoption)
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("/Backups/" + username));
                return false;
            }
        }
        public string ZipForDownload(string filename)
        {
            try
            {
                // Set error message.
                HttpContext.Current.Session["ErrorMessage"] = "ZIP_NOT_CREATED";

                // Set variables.
                StringBuilder pathstream = new StringBuilder(255);
                GetShortPathName(HttpContext.Current.Server.MapPath(@"~\Tmp\Downloads\"), pathstream, pathstream.Capacity);
                string path = pathstream.ToString();
                HttpContext.Current.Session["ErrorMessage"] = path;

                string name = Path.GetFileNameWithoutExtension(filename);
                string zippath = path + @"\" + name + ".zip";

                // Move file to tmp directory.
                Directory.CreateDirectory(path + @"\" + name);
                StringBuilder tmppathstream = new StringBuilder(255);
                GetShortPathName((path + @"\" + name), tmppathstream, tmppathstream.Capacity);
                string tmppath = tmppathstream.ToString();
                HttpContext.Current.Session["ErrorMessage"] = "LINE_600" + tmppath + @"\" + filename;
                File.Move((path + @"\" + filename), (tmppath + @"\" + filename));
                HttpContext.Current.Session["ErrorMessage"] = "LINE_602" + tmppath;

                //File.Delete(filepath);

                // Create zip file and delete tmp.
                if (File.Exists(zippath))
                    File.Delete(zippath);
                ZipFile.CreateFromDirectory((path + @"\" + name), zippath);
                HttpContext.Current.Session["ErrorMessage"] = "LINE_608" + tmppath;

                Directory.Delete(tmppath, true);

                return Path.GetFileName(zippath);
            }
            catch (Exception e)
            { throw e; }
        }
    }
}
