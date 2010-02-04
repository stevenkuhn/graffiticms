**************
 Graffiti 1.3
**************

IMPORTANT:
Before you can compile Graffiti CMS, you will first need to download a couple of extra assemblies from Telligent that the Graffiti source code relies on. These can be downloaded at:
http://graffiticms.com/lib/lib.zip


INSTRUCTIONS:

1. Unzip the "extra assemblies" lib.zip file and place the dll files in the \Branches\v1.3\src\Lib folder.

2. Open the \Branches\v1.3\src\Graffiti.sln solution file with Visual studio 2008.

3. Choose what type of database you will be using. Microsoft SQL Server 2000/2005/2008, Microsoft Access, and MySQL are supported in the current code base. VistaDB was supported in prior closed source versions of  Graffiti CMS but is not included in this project at this time due to licensing constraints. Telligent is working with them on making it available for download from their site in the future.

4. Update the web.config to use the appropriate datbase provider AppSetting value for the type of database you will use. Find the "DataBuddy::Provider" key and update the value (if needed) to one of the following:
Access:  DataBuddy.MSAccessProvider, DataBuddy
MySQL:   DataBuddy.MySQLDataProvider, DataBuddy
SQL Server 2000:  DataBuddy.SQLDataProvider, DataBuddy
SQL Server 2005/2008:  DataBuddy.SQL2K5DataProvider, DataBuddy

5. Update the web.config to use the appropriate database connection string. Sample connection strings for various database types are included in the web.config comments.

6. Setup the database using the instructions in the \Branches\v1.3\data\read_me.txt file.

7. Build the solution. You may need to set the Graffiti.Web project as the start up Project.


IMPORTANT NOTES:
* If you are using IIS 7 or above, and ASP.NET 3.5 SP1, the new url routing features will work great. If not, you'll need to enable the option to generate folders for posts and categories. This setting is found in the admin > Site Options > Configuration page.

* If you are currently using a prior version of Graffiti CMS with VistaDB and do not wish to migrate to a different database then you should keep using the version you have for now.

