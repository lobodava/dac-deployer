create table [dbo].Roles
(
	Id			int				not null  identity  primary key,
	[Name]		nvarchar(200)	not null	,
	[Type]		tinyint			null

)
