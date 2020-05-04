AspSqlProceduresUpdater - easy way to update your user stored procedures in .Net project.
========================================

# Introduction

You use .Net to create your application and you need to create/update SQL user-defined stored procedures that you need. AspSqlProceduresUpdater - easy way to update your SQL user-defined stored procedures in .Net project. There is no need to connect to the database and update SQL procedures manually every time you change. It is enough to store their code in your .Net project and AspSqlProceduresUpdater will do everything itself when you start the project. To do this, you will need to create a directory in the project that will contain files with the extension .sql with code for creating procedures. When calling AspSqlProceduresUpdater, it will delete all already created procedures and execute all scripts to create new procedures, which are found in the transferred directory.

At the moment AspSqlProceduresUpdater works only with MS SQL.

# Installation

The easiest way to use AspSqlProceduresUpdater is through Nuget. Simply issue the following command at the Package Manager Console in Visual Studio:

```
PM> Install-Package AspSqlProceduresUpdater
```

https://www.nuget.org/packages/AspSqlProceduresUpdater

# Usage

You need to create a directory in which to place files with the extension .sql, which create SQL procedures. This directory can have subdirectories, the method will find all .sql scripts and execute them. Then call AspSqlProceduresUpdater.UpdateProcedures and pass the database connection string and the full absolute path to the scripted directory.
Before that you can check which SQL procedures the method will delete at startup. To do this, run the following code in the desired database (it will print the names of SQL procedures that will be removed):

```sql
SELECT [name] FROM sysobjects WHERE [type] = 'P' AND category = 0;
```

# Examples

### ASP .NET MVC Project

When starting the application, the ***Application_Start*** method from ***Global.asax.cs*** will be executed, which will run ***AspSqlProceduresUpdater.UpdateProcedures***. It will delete all SQL user-defined stored procedures in the database and create new ones, scripts of which are located in the project directory ***"~/DatabaseScripts/"***.

File ***Global.asax.cs***:

```c#
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AspSqlProceduresUpdater.UpdateProcedures(
                ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString,
                HttpContext.Current.Server.MapPath("~/Procedures/")
                );

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
```

File ***Web.config***:

```xml
  <connectionStrings>
    <add name="DBConnection" connectionString="Server=localhost\SQLEXPRESS;Database=DBConnection;Trusted_Connection=True;" providerName="System.Data.SqlClient" />
  </connectionStrings>
```
