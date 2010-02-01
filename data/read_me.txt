****************************
Database Setup Instructions:
****************************

ACCESS:
1. Copy the Graffiti.mdb file into the \Trunk\src\Graffiti.Web\App_Data folder (or alternate location depending on connection string value)

-----------------------------

SQL Server:

1. Create a database (default SQL Server conn string uses a database name of "Graffiti")
2. Run the Graffiti_Schema.sql script
3. Run the the Graffiti_Data.sql script (this will create a user with the credentials admin/password)

Optional 1:
If you want to use integrated security, you can run either SQL_IntegratedSecurity_xp.sql (for Windows XP) or SQL_IntegratedSecurity.sql (for Windows Server or Vista) to automatically setup the permissions.

Optional 2:
If you want to use the ASP.Net Membershp provider:
  A. Run the Graffiti_ASPNet_Membership_Provider_Schema.sql script
  B. Run the Graffiti_ASPNet_Membership_Provider_Data.sql script
  C. Update & uncomment the Graffiti_ASPNetMembership conn string in the web.config file. Note: you can use the same database for Graffiti and Membership.

-----------------------------

MySQL:

1. Run the Graffiti_mySQL_Schema.sql script
2. Run the Graffiti_mySQL_Data.sql script

