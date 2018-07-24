--------------------------------------------------------------------------------
                                  DATABASEMGR
                              Charles Christensen
--------------------------------------------------------------------------------

 1. SOURCE
    This folder contains all of the source code for the original DatabaseMgr,
    along with the Visual Studio solution file. All of the code necessary
    to stand up a new instance of DatabaseMgr are included in this folder.
    --> Includes: models, views, controllers, scripts, and stylesheets.

 2. BACKUPS
    Contains the backup files for all database backups stored on DatabaseMgr.
    Within this folder, backups are organized by username. Each backup is saved
    with a unique filename that helps identify it to DatabaseMgr. If the
    filename of a backup does not match the naming convention, it may not appear
    as an option in the DatabaseMgr web interface.
    --> Filename Convention: DatabaseName_ServerName_Version_Timestamp_Nickname

    In order for the backup/restore functionality to work for Microsoft SQL,
    this backup folder must be an accessible SMB share to everyone, anonymous,
    and guest at the following location.
    --> Address: \\hostname\Backups

 3. BIN
    Contains all of the binary reference files necessary for DatabaseMgr to
    function. For the most part, Visual Studio is effective at creating this
    file properly. Watch out for the "mysql" file though. Visual Studio does not
    recognize it, and it must be added manually. (The mysql binary folder can be
    found in the DEPENDENCIES folder.)

 4. DATA
    Contains the DatabaseMgr.sqlite3 file, which contains all of the database,
    server, and protocol info for DatabaseMgr. This file also contains a blank
    DatabaseMgr file, which can be used to stand up a new version of
    DatabaseMgr.

 5. DEPENDENCIES
    Contains binaries and utilities that are used to impliment DatabaseMgr.
     A. MySQL Binaries. Add these to the BIN folder if they are not already
        there. Allows for additional MySQL functionality.
     B. SQLiteStudio. Use this to alter the DatabaseMgr.sqlite3 database. This
        is necessary to add servers to DatabaseMgr and modify ownership of
        databases.
     C. MsSql Utilities. Install the ODBC driver and either Utils11 or Utils13
        in order to have additional Microsoft SQL functionality. (In the
        SQL16Interface model, it is necessary to point the application to the
        direct location of the sqlcmd.exe file in order to execute MSSQL
        commands through DatabaseMgr.)

 6. IMAGES
    Contains the image files for DatabaseMgr. There should be a PNG image for
    each SQL vendor in this file.
    --> Example: microsoft.png

 7. MAINTENANCE
    Contains optional PowerShell scripts for cleaning up the TMP folder via a
    scheduled task.

 8. SCRIPTS
    Javascript files for the front-end interface of DatabaseMgr.

 9. STYLES
    CSS files for the front-end interface of DatabaseMgr.

 10. TMP
     Contains temporary database files for users that opt to upload or download
     their own.
      A. Uploads. Contains restore files uploaded to DatabaseMgr.
      B. Downloads. Contains backup files downloaded from DatabaseMgr.

     In order for the backup/restore functionality to work for Microsoft SQL,
     this backup folder must be an accessible SMB share to everyone, anonymous,
     and guest at the following location.
     --> Address: \\hostname\Tmp

 11. VIEWS
     CSHTML Razor files for rendering the front-end interface of DatabaseMgr.

 12. OTHER
      A. ApplicationInsights.config. Configuration settings from reporting
         DatabaseMgr use back to Microsoft.
      B. favicon.ico. If you want to see a favicon for the site, add it at this
         root level.
      C. Web.config. General settings for starting the DatabaseMgr webapp.
