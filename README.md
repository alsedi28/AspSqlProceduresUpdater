AspSqlProceduresUpdater - easy way to update your user stored procedures in .Net project.
========================================

# Introduction

You use .Net to create your application and you need to create/update SQL user-defined stored procedures that you need. AspSqlProceduresUpdater - easy way to update your SQL user-defined stored procedures in .Net project. To do this, you will need to create a directory in the project that will contain files with the extension .sql with code for creating procedures. When calling AspSqlProceduresUpdater, it will delete all already created procedures and execute all scripts to create new procedures, which are found in the transferred directory.

# Installation

The easiest way to use AspSqlProceduresUpdater is through Nuget. Simply issue the following command at the Package Manager Console in Visual Studio:

```
PM> Install-Package AspSqlProceduresUpdater
```

https://www.nuget.org/packages/AspSqlProceduresUpdater
