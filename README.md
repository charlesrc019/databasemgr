### DATABASEMGR ###
Charles Christensen


PREREQS
1. Windows IIS with .NET 4.5.
2. Visual Studio 2015+ (to compile code).
3. Ability to create globally accessable SMB shares.
4. Microsoft ODBC 11 driver.
5. Microsoft SQL command line utilities.

CONFIG
The configuration for the routing and security setting of the
web app.

CONTROLLERS
Handle the routing of the login, view, and action elements.
You'll find most of the web app logic here.

DATA
DatabaseMgr uses an SQLite database to track current databases,
servers, etc. I recommend using a freeware database browser to
do administrative actions for DatabaseMgr (like adding a
database or transferring ownership).

IMAGES
Contains an image of each SQL vendor (orignally Microsoft and 
MySQL) so that the database management pages can look pretty.

MODELS
The backend logic for database management. Includes:
- connection to SQLite database (for management)
- connection to Active Directory (for login)
- interface node
- connection to MySQL servers
- connection to Microsoft SQL
- (you can add your own connection as needed)

PROPERTIES
Properties for editing and debugging in Visual Studio.

SCRIPTS
Javascript files to make the front-end pretty (mostly
Bootstrap).

STYLES
CSS stylesheets to make the front-end pretty (mostly 
Bootstrap).

VIEWS
Razor HTML files to generate the front-end views.

BACKUPS (not included in repo)
You'll need to create a globally accessable folder at the
address \\hostname\Backups so that servers can save their
backups to this central location.

TMP (not included in repo)
You'll need to create a globally accessable folder at the
address \\hostname\Tmp so that servers can save their
backups to this central location. This folder needs two 
subfolders: Uploads and Downloads.

BIN (not included in repo)
All the binary dependencies for DatabaseMgr to run.
Should contain all the following files:

Antlr3.Runtime.dll
Antlr3.Runtime.pdb
ApplicationInsights.config
DatabaseManager.dll
DatabaseManager.dll.config
DatabaseManager.pdb
EntityFramework.dll
EntityFramework.SqlServer.dll
EntityFramework.SqlServer.xml
EntityFramework.xml
Google.Protobuf.dll
Google.Protobuf.xml
Microsoft.AI.Agent.Intercept.dll
Microsoft.AI.DependencyCollector.dll
Microsoft.AI.PerfCounterCollector.dll
Microsoft.AI.ServerTelemetryChannel.dll
Microsoft.AI.Web.dll
Microsoft.AI.WindowsServer.dll
Microsoft.ApplicationInsights.dll
Microsoft.AspNet.Identity.Core.dll
Microsoft.AspNet.Identity.Core.xml
Microsoft.AspNet.Identity.EntityFramework.dll
Microsoft.AspNet.Identity.EntityFramework.xml
Microsoft.AspNet.Identity.Owin.dll
Microsoft.AspNet.Identity.Owin.xml
Microsoft.AspNet.TelemetryCorrelation.dll
Microsoft.AspNet.TelemetryCorrelation.xml
Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll
Microsoft.CodeDom.Providers.DotNetCompilerPlatform.xml
Microsoft.Owin.dll
Microsoft.Owin.Host.SystemWeb.dll
Microsoft.Owin.Host.SystemWeb.xml
Microsoft.Owin.Security.Cookies.dll
Microsoft.Owin.Security.Cookies.xml
Microsoft.Owin.Security.dll
Microsoft.Owin.Security.Facebook.dll
Microsoft.Owin.Security.Facebook.xml
Microsoft.Owin.Security.OAuth.dll
Microsoft.Owin.Security.OAuth.xml
Microsoft.Owin.Security.xml
Microsoft.Owin.xml
Microsoft.SqlServer.ConnectionInfo.dll
Microsoft.Web.Infrastructure.dll
msodbcsql.msi
MsSqlCmdLnUtils.msi
MySql.Data.dll
MySql.Data.xml
Newtonsoft.Json.dll
Newtonsoft.Json.xml
Owin.dll
roslyn
System.Data.SQLite.dll
System.Data.SQLite.dll.config
System.Data.SQLite.EF6.dll
System.Data.SQLite.Linq.dll
System.Data.SQLite.xml
System.Diagnostics.DiagnosticSource.dll
System.Diagnostics.DiagnosticSource.xml
System.Web.Helpers.dll
System.Web.Helpers.xml
System.Web.Mvc.dll
System.Web.Mvc.xml
System.Web.Optimization.dll
System.Web.Optimization.xml
System.Web.Razor.dll
System.Web.Razor.xml
System.Web.WebPages.Deployment.dll
System.Web.WebPages.Deployment.xml
System.Web.WebPages.dll
System.Web.WebPages.Razor.dll
System.Web.WebPages.Razor.xml
System.Web.WebPages.xml
WebGrease.dll

In addition, it should contain a "mysql" folder that contains
the MySQL binaries avaible on their website.

That's it! Once you have the folder structure setup, 
DatabaseMgr will be ready to use.

*Originally compilied using Visual Studio 2015.
