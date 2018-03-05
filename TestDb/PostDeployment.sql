use [$(DatabaseName)];
GO

set nocount on;


merge into dbo.Categories as target 
using (
	values 

	--	Id	,	,	Name								
	
	(	1	,	N'АИ-98'			),	
	(	2	,	N'АИ-95'			),
	(	3	,	N'АИ-92'			),
	(	4	,	N'Дизельное топливо')
	
) 	as source (Id,  Name) on target.Id = source.Id
 
when matched then 
	update set 
		Name	= source.Name	
		 
when not matched by target then 
	insert (Id, Name) 
	values (Id, Name) 

when not matched by source then 
	delete;

declare @Param1 varchar(50) = '$(Param1)';
declare @Param2 varchar(50) = '$(Param2)';

print 'OK';
print @Param1;
print @Param2;

print ' '



GO