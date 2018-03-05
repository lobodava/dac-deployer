create table [dbo].[Users]
(
	Id			int				not null  identity  primary key,
	[Name]		nvarchar(200)	not null	,
	CategoryId	tinyint			not null	,

	constraint FK_Users_CategoryId foreign key (CategoryId) references dbo.Categories (Id)
)
