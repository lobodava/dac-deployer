SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON
GO
-- SQLCMD Mode - http://msdn.microsoft.com/ru-ru/library/vstudio/aa833281(v=vs.100).aspx

--:R "_SqlCmdVariables.script.sql"
--GO

use [$(DatabaseName)];
print 'OK'
print ' '
GO

print 'Dropping everything of Categories'
GO
ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_Users_CategoryId]
GO
ALTER TABLE [dbo].[Users] DROP COLUMN [CategoryId]
GO
DROP TABLE [dbo].[Categories]
GO

DROP TABLE [dbo].Roles
GO

print 'OK'
print ' '



print '_BeforeDeployment Runs in $(DatabaseName)!!! Hooray!!'
print ' '
GO

:R "TestScript.sql"
GO

set nocount on;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Categories')
BEGIN
    create table [dbo].Categories
	(
		Id		tinyint			not null   primary key,
		[Name]  nvarchar(200)	not null
	);

	--exec ('insert into dbo.Categories values (1, ''AAA'');');
	insert into dbo.Categories values (1, 'AAA');
END


if not exists(SELECT 1 FROM sys.columns WHERE Name = N'CategoryId' AND Object_ID = Object_ID(N'dbo.Users'))
BEGIN
    ALTER TABLE dbo.Users ADD CategoryId tinyint NULL;

	exec ('update dbo.Users set CategoryId = 1;');

	ALTER TABLE dbo.Users alter column CategoryId tinyint not NULL;
END