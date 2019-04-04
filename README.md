# DATABASEMGR
Charles Christensen


## PREREQS
1. Windows IIS with .NET 4.5.
2. Visual Studio 2015+ (to compile code).
3. Ability to create globally accessable SMB shares.
4. Microsoft ODBC 11 driver.
5. Microsoft SQL command line utilities.

## CONFIG
The configuration for the routing and security setting of the
web app.

## CONTROLLERS
Handle the routing of the login, view, and action elements.
You'll find most of the web app logic here.

## DATA
DatabaseMgr uses an SQLite database to track current databases,
servers, etc. I recommend using a freeware database browser to
do administrative actions for DatabaseMgr (like adding a
database or transferring ownership).

## IMAGES
Contains an image of each SQL vendor (orignally Microsoft and 
MySQL) so that the database management pages can look pretty.

## MODELS
The backend logic for database management. Includes:
- connection to SQLite database (for management)
- connection to Active Directory (for login)
- interface node
- connection to MySQL servers
- connection to Microsoft SQL
- (you can add your own connection as needed)

## PROPERTIES
Properties for editing and debugging in Visual Studio.

## SCRIPTS
Javascript files to make the front-end pretty (mostly Bootstrap).

## STYLES
CSS stylesheets to make the front-end pretty (mostly Bootstrap).

## VIEWS
Razor HTML files to generate the front-end views.

## BACKUPS (not included in repo)
You'll need to create a globally accessable folder at the
address \\hostname\Backups so that servers can save their
backups to this central location.

## TMP (not included in repo)
You'll need to create a globally accessable folder at the
address \\hostname\Tmp so that servers can save their
backups to this central location. This folder needs two 
subfolders: Uploads and Downloads.

## BIN (not included in repo)
All the binary dependencies for DatabaseMgr to run.
Should contain all the following files:

- Antlr3.Runtime
- EntityFramework
- EntityFramework.SqlServer
- Google.Protobuf
- Microsoft.AI.Agent.Intercept
- Microsoft.AI.DependencyCollector
- Microsoft.AI.PerfCounterCollector
- Microsoft.AI.ServerTelemetryChannel
- Microsoft.AI.Web
- Microsoft.AI.WindowsServer
- Microsoft.ApplicationInsights
- Microsoft.AspNet.Identity.Core
- Microsoft.AspNet.Identity.EntityFramework
- Microsoft.AspNet.Identity.Owin
- Microsoft.AspNet.TelemetryCorrelation
- Microsoft.CodeDom.Providers.DotNetCompilerPlatform
- Microsoft.Owin
- Microsoft.Owin.Host.SystemWeb
- Microsoft.Owin.Security.Cookies
- Microsoft.Owin.Security
- Microsoft.Owin.Security.Facebook
- Microsoft.Owin.Security.OAuth
- Microsoft.SqlServer.ConnectionInfo
- Microsoft.Web.Infrastructure
- msodbcsql (MySQL CLI connector)
- MsSqlCmdLnUtils (SQL Server CLI Connector)
- MySql.Data
- Newtonsoft.Json
- Owin
- roslyn
- System.Data.SQLite
- System.Data.SQLite.EF6
- System.Data.SQLite.Linq
- System.Diagnostics.DiagnosticSource
- System.Web.Helpers
- System.Web.Mvc
- System.Web.Optimization
- System.Web.Razor
- System.Web.WebPages.Deployment
- System.Web.WebPages
- System.Web.WebPages.Razor
- WebGrease

In addition, it should contain a "mysql" folder that contains
the MySQL binaries avaible on their website.

That's it! Once you have the folder structure setup, 
DatabaseMgr will be ready to use.

*Originally compilied using Visual Studio 2015.*

So long, and thanks for all the fish!

*Charles*
