/* be sure to execute this in the database that you want the access to be granted */

declare @loginame sysname, @name_in_db sysname

/* set this to the windows user/principal that needs access to the database
 * in this example, it is the network service principal
 * please set this accordingly for your environment
 */
select @loginame = N'NT AUTHORITY\NETWORK SERVICE', @name_in_db = N'NT Authority\Network Service'

print 'The current database is ' + db_name()

if not exists (select * from master.dbo.syslogins where loginname = @loginame)
begin
	print 'Attempting to grant ' + @name_in_db + ' as a login to this SQL Server.'
	exec sp_grantlogin @loginame
end

/* checking to ensure that the login exists now */
if exists (select * from master.dbo.syslogins where loginname = @loginame)
begin
	/* Allows this windows user/pricipal access to the current context database */
	print 'Attempting to grant db access to ' + @name_in_db
	exec sp_grantdbaccess @loginame, @name_in_db

	/* adding login to the db_datareader roles */
	print 'Adding ' + @name_in_db + ' to the db_datareader role'
	exec sp_addrolemember N'db_datareader', @name_in_db

	/* adding login to the db_datawriter roles */
	print 'Adding ' + @name_in_db + ' to the db_datawriter role'
	exec sp_addrolemember N'db_datawriter', @name_in_db
end
else
begin
	print 'Login ' + @name_in_db + ' was not added to the server'
end

print 'Done with login action.'
