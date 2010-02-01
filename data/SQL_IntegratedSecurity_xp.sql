/* be sure to execute this in the database that you want the access to be granted */

declare @AspNetUser nvarchar(128)

/* set this to the windows user that needs access to the database
 * in this example, it is the local machine's ASPNET account
 * please set this accordingly for your environment
 */
select @AspNetUser = @@SERVERNAME + N'\ASPNET'

print 'The current database is ' + db_name()

if not exists (select * from master.dbo.syslogins where loginname = @AspNetUser)
begin
	print 'Attempting to grant ' + @AspNetUser + ' as a login to this SQL Server.'
	exec sp_grantlogin @loginame = @AspNetUser
end

/* checking to ensure that the login exists now */
if exists (select * from master.dbo.syslogins where loginname = @AspNetUser)
begin
	/* Allows this windows user access to the current context database */
	print 'Attempting to grant db access to ' + @AspNetUser
	exec sp_grantdbaccess @loginame = @AspNetUser

	/* adding login to the db_datareader roles */
	print 'Adding ' + @AspNetUser + ' to the db_datareader role'
	exec sp_addrolemember N'db_datareader', @AspNetUser

	/* adding login to the db_datawriter roles */
	print 'Adding ' + @AspNetUser + ' to the db_datawriter role'
	exec sp_addrolemember N'db_datawriter', @AspNetUser
end
else
begin
	print 'Login ' + @AspNetUser + ' was not added to the server'
end

print 'Done with login action.'
