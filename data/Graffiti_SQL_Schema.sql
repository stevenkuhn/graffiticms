/****** Object:  Table [dbo].[graffiti_Users]    Script Date: 08/26/2007 22:49:51 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_Users]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128)  NOT NULL,
	[Email] [nvarchar](128)  NOT NULL,
	[ProperName] [nvarchar](255)  NULL,
	[TimeZoneOffSet] [float] NOT NULL,
	[Bio] [nvarchar](2000)  NULL,
	[Avatar] [nvarchar](255)  NULL,
	[PublicEmail] [nvarchar](255)  NULL,
	[WebSite] [nvarchar](255)  NULL,
	[Password] [nvarchar](128)  NOT NULL,
	[Password_Salt] [nvarchar](128)  NOT NULL,
	[PasswordFormat] [int] NOT NULL,
	[UniqueId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_graffiti_Users_UniqueId]  DEFAULT (newid()),
 CONSTRAINT [PK_graffiti_Users] PRIMARY KEY NONCLUSTERED
(
	[Name] ASC
),
 CONSTRAINT [IX_graffiti_Users_Email] UNIQUE NONCLUSTERED
(
	[Email] ASC
),
 CONSTRAINT [IX_graffiti_Users_Id] UNIQUE CLUSTERED
(
	[Id] ASC
),
 CONSTRAINT [IX_graffiti_Users_UniqueId] UNIQUE NONCLUSTERED
(
	[UniqueId] ASC
)
) ON [PRIMARY]
END


/****** Object:  Table [dbo].[graffiti_Categories]    Script Date: 08/26/2007 22:49:51 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_Categories]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255)  NOT NULL,
	[View] [nvarchar](64)  NULL,
	[PostView] [nvarchar](64)  NULL,
	[FormattedName] [nvarchar](255)  NOT NULL,
	[LinkName] [nvarchar](255)  NOT NULL,
	[FeedUrlOverride] [nvarchar](255)  NULL,
	[Body] [ntext]  NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_graffiti_Categories_IsDeleted]  DEFAULT ((0)),
	[Post_Count] [int] NOT NULL CONSTRAINT [DF_graffiti_Categories_Post_Count]  DEFAULT ((0)),
	[UniqueId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_graffiti_Categories_UniqueId]  DEFAULT (newid()),
	[ParentId] [int] NOT NULL CONSTRAINT [DF_graffiti_Categories_ParentId]  DEFAULT ((-1)),
	[Type] [int] NOT NULL CONSTRAINT [DF_graffiti_Categories_Type]  DEFAULT ((0)),
	[ImageUrl] [nvarchar](255)  NULL,
	[MetaDescription] [nvarchar](255)  NULL,
	[MetaKeywords] [nvarchar](255)  NULL,
	[FeaturedId] [int] NOT NULL CONSTRAINT [DF_graffiti_Categories_FeaturedId]  DEFAULT ((0)),
	[SortOrderTypeId] [int] NOT NULL CONSTRAINT [DF_graffiti_Categories_SortTypeTypeId]  DEFAULT (0),
	[ExcludeSubCategoryPosts] [bit] NOT NULL CONSTRAINT [DF_graffiti_Categories_ExcludeSubCategoryPosts]  DEFAULT (0),
 CONSTRAINT [PK_graffiti_Categories] PRIMARY KEY CLUSTERED
(
	[Id] ASC
),
 CONSTRAINT [IX_graffiti_Categories_LinkName_ParentId] UNIQUE NONCLUSTERED
(
	[LinkName] ASC,
	[ParentId] ASC
),
 CONSTRAINT [IX_graffiti_Categories_Name_ParentId] UNIQUE NONCLUSTERED
(
	[Name] ASC,
	[ParentId] ASC
)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END


/****** Object:  Table [dbo].[graffiti_Posts]    Script Date: 08/26/2007 22:49:51 ******/
/* Will need to just deal with the maximum row length for SQL 2000 users */
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_Posts]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_Posts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255)  NOT NULL,
	[PostBody] [ntext]  NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_graffiti_Posts_Status]  DEFAULT ((1)),
	[Content_Type] [nvarchar](50)  NOT NULL,
	[Name] [nvarchar](255)  NOT NULL,
	[Comment_Count] [int] NOT NULL CONSTRAINT [DF_graffiti_Posts_Comment_Count]  DEFAULT ((0)),
	[Tag_List] [nvarchar](512)  NULL,
	[CategoryId] [int] NOT NULL,
	[Version] [int] NOT NULL CONSTRAINT [DF_graffiti_Posts_Version]  DEFAULT ((0)),
	[ModifiedBy] [nvarchar](128)  NULL,
	[CreatedBy] [nvarchar](128)  NOT NULL,
	[ExtendedBody] [ntext]  NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_graffiti_Posts_IsDeleted]  DEFAULT ((0)),
	[Published] [datetime] NOT NULL,
	[Pending_Comment_Count] [int] NOT NULL CONSTRAINT [DF_graffiti_Posts_Pending_Comment_Count]  DEFAULT ((0)),
	[Views] [int] NOT NULL CONSTRAINT [DF_graffiti_Posts_Views]  DEFAULT ((0)),
	[UniqueId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_graffiti_Posts_UniqueId]  DEFAULT (newid()),
	[EnableComments] [bit] NOT NULL CONSTRAINT [DF_graffiti_Posts_EnableComments]  DEFAULT ((1)),
	[PropertyKeys] ntext NULL,
	[PropertyValues] ntext NULL,
	[UserName] [nvarchar](128)  NOT NULL,
	[Notes] [nvarchar](2000)  NULL,
	[ImageUrl] [nvarchar](255)  NULL,
	[MetaDescription] [nvarchar](255)  NULL,
	[MetaKeywords] [nvarchar](255)  NULL,
	[IsPublished] [bit] NOT NULL CONSTRAINT [DF_graffiti_Posts_IsPublished]  DEFAULT ((0)),
	[SortOrder] [int] NOT NULL CONSTRAINT [DF_graffiti_Posts_SortOrder]  DEFAULT (0),
	[ParentId] [int] NOT NULL CONSTRAINT [DF_graffiti_Posts_ParentId]  DEFAULT (0),
	[IsHome] [bit] NOT NULL CONSTRAINT [DF_graffiti_Posts_IsHome]  DEFAULT ((0)),
	[HomeSortOrder] [int] NOT NULL CONSTRAINT [DF_graffiti_Posts_HomeSortOrder]  DEFAULT (0),
 CONSTRAINT [PK_graffiti_Posts] PRIMARY KEY CLUSTERED
(
	[Id] ASC
),
 CONSTRAINT [IX_graffiti_Posts_CategoryId_Name] UNIQUE NONCLUSTERED
(
	[CategoryId] ASC,
	[Name] ASC
)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF OBJECT_ID(N'[dbo].[FK_graffiti_Posts_graffiti_Categories]', N'F') IS NULL
BEGIN
	ALTER TABLE [dbo].[graffiti_Posts] WITH CHECK ADD CONSTRAINT [FK_graffiti_Posts_graffiti_Categories] FOREIGN KEY([CategoryId])
	REFERENCES [dbo].[graffiti_Categories] ([Id])
END

/****** Object:  Table [dbo].[graffiti_VersionStore]    Script Date: 08/26/2007 22:49:51 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_VersionStore]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_VersionStore](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UniqueId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_graffiti_VersionStore_UniqueId]  DEFAULT (newid()),
	[Data] [ntext]  NOT NULL,
	[Type] [nvarchar](128)  NOT NULL,
	[Version] [int] NOT NULL,
	[CreatedBy] [nvarchar](128)  NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Name] [nvarchar](255)  NOT NULL,
	[ItemId] [int] NOT NULL CONSTRAINT [DF_graffiti_VersionStore_ItemId]  DEFAULT ((0)),
	[Notes] [nvarchar](2000)  NULL,
 CONSTRAINT [PK_graffiti_VersionStore] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END


/****** Object:  Table [dbo].[graffiti_ObjectStore]    Script Date: 08/26/2007 22:49:51 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_ObjectStore]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_ObjectStore](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128)  NOT NULL,
	[Data] [ntext]  NOT NULL,
	[Type] [nvarchar](255)  NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[Content_Type] [nvarchar](128)  NOT NULL,
	[Version] [int] NOT NULL,
	[UniqueId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_graffiti_ObjectStore_UniqueId]  DEFAULT (newid()),
 CONSTRAINT [PK_ObjectStore] PRIMARY KEY CLUSTERED
(
	[Id] ASC
),
 CONSTRAINT [IX_graffiti_ObjectStore_Name] UNIQUE NONCLUSTERED
(
	[Name] ASC
)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END


/****** Object:  Table [dbo].[graffiti_Comments]    Script Date: 08/26/2007 22:49:51 ******/
/* Will need to just deal with the maximum row length for SQL 2000 users */
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_Comments]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_Comments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PostId] [int] NOT NULL,
	[Body] [nvarchar](4000)  NOT NULL,
	[CreatedBy] [nvarchar](128)  NULL,
	[Published] [datetime] NOT NULL,
	[Name] [nvarchar](128)  NOT NULL,
	[IsPublished] [bit] NOT NULL,
	[Version] [int] NOT NULL,
	[WebSite] [nvarchar](512)  NULL,
	[SpamScore] [int] NOT NULL,
	[IPAddress] [nvarchar](64)  NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](128)  NULL,
	[Email] [nvarchar](128)  NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_graffiti_Comments_IsDeleted]  DEFAULT ((0)),
	[UniqueId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_graffiti_Comments_UniqueId]  DEFAULT (newid()),
	[UserName] [nvarchar](128)  NULL,
	[IsTrackback] [bit] NOT NULL CONSTRAINT [DF_graffiti_Comments_IsTrackback]  DEFAULT (0),
 CONSTRAINT [PK_graffiti_Comments] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)
) ON [PRIMARY]
END

IF OBJECT_ID(N'[dbo].[FK_graffiti_Comments_graffiti_Posts]', N'F') IS NULL
BEGIN
	ALTER TABLE [dbo].[graffiti_Comments] WITH CHECK ADD CONSTRAINT [FK_graffiti_Comments_graffiti_Posts] FOREIGN KEY([PostId])
	REFERENCES [dbo].[graffiti_Posts] ([Id])
END

/* may consider at some point to make this a clustered index and the PK nonclustered */
IF indexproperty( OBJECT_ID(N'[dbo].[graffiti_Comments]'), N'IX_graffiti_Comments_PostId', N'IndexID' ) IS NULL
BEGIN
	CREATE NONCLUSTERED INDEX [IX_graffiti_Comments_PostId] ON [dbo].[graffiti_Comments] ([PostId]) ON [PRIMARY]
END


/****** Object:  Table [dbo].[graffiti_Logs]    Script Date: 08/26/2007 22:49:51 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_Logs]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_Logs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[Title] [nvarchar](255)  NOT NULL,
	[Message] [nvarchar](2000)  NOT NULL,
	[CreatedBy] [nvarchar](128)  NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_graffiti_Logs] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)
) ON [PRIMARY]
END


/****** Object:  Table [dbo].[graffiti_UserRoles]    Script Date: 08/26/2007 22:49:51 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_UserRoles]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_UserRoles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[RoleName] [nvarchar](128)  NOT NULL,
 CONSTRAINT [PK_graffiti_UserRoles] PRIMARY KEY CLUSTERED
(
	[Id] ASC
),
 CONSTRAINT [IX_graffiti_UserRoles_RoleName_UserId] UNIQUE NONCLUSTERED
(
	[RoleName] ASC,
	[UserId] ASC
)
) ON [PRIMARY]
END

IF OBJECT_ID(N'[dbo].[FK_graffiti_UserRoles_graffiti_Users]', N'F') IS NULL
BEGIN
	ALTER TABLE [dbo].[graffiti_UserRoles] WITH CHECK ADD CONSTRAINT [FK_graffiti_UserRoles_graffiti_Users] FOREIGN KEY([UserId])
	REFERENCES [dbo].[graffiti_Users] ([Id])
END


/****** Object:  Table [dbo].[graffiti_Post_Statistics]    Script Date: 08/26/2007 22:49:51 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_Post_Statistics]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_Post_Statistics](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PostId] [int] NOT NULL,
	[DateViewed] [datetime] NOT NULL,
 CONSTRAINT [PK_graffiti_Post_Statistics] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)
) ON [PRIMARY]
END

IF OBJECT_ID(N'[dbo].[FK_graffiti_Post_Statistics_graffiti_Posts]', N'F') IS NULL
BEGIN
	ALTER TABLE [dbo].[graffiti_Post_Statistics] WITH CHECK ADD CONSTRAINT [FK_graffiti_Post_Statistics_graffiti_Posts] FOREIGN KEY([PostId])
	REFERENCES [dbo].[graffiti_Posts] ([Id])
END


/****** Object:  Table [dbo].[graffiti_Tags]    Script Date: 08/26/2007 22:49:50 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_Tags]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_Tags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128)  NOT NULL,
	[PostId] [int] NOT NULL,
 CONSTRAINT [PK_graffiti_Tags] PRIMARY KEY CLUSTERED
(
	[Id] ASC
),
 CONSTRAINT [IX_graffiti_Tags_Name_PostId] UNIQUE NONCLUSTERED
(
	[Name] ASC,
	[PostId] ASC
)
) ON [PRIMARY]
END

IF OBJECT_ID(N'[dbo].[FK_graffiti_Tags_graffiti_Posts]', N'F') IS NULL
BEGIN
	ALTER TABLE [dbo].[graffiti_Tags] WITH CHECK ADD CONSTRAINT [FK_graffiti_Tags_graffiti_Posts] FOREIGN KEY([PostId])
	REFERENCES [dbo].[graffiti_Posts] ([Id])
END

IF indexproperty( OBJECT_ID(N'[dbo].[graffiti_Tags]', N'U'), N'IX_graffiti_Tags_PostId', N'IndexID' ) IS NULL
BEGIN
	CREATE NONCLUSTERED INDEX [IX_graffiti_Tags_PostId] ON [dbo].[graffiti_Tags] ([PostId]) ON [PRIMARY]
END


/****** Object:  Table [dbo].[graffiti_RolePermissions]    Script Date: 04/25/2008 09:00:00 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_RolePermissions]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_RolePermissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](128)  NOT NULL,
	[HasRead] [bit] NOT NULL CONSTRAINT [DF_graffiti_RolePermissions_HasRead]  DEFAULT ((0)),
	[HasEdit] [bit] NOT NULL CONSTRAINT [DF_graffiti_RolePermissions_HasEdit]  DEFAULT ((0)),
	[HasPublish] [bit] NOT NULL CONSTRAINT [DF_graffiti_RolePermissions_HasPublish]  DEFAULT ((0)),
	[CreatedBy] [nvarchar](128)  NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](128)  NULL,
 CONSTRAINT [PK_graffiti_RolePermissions] PRIMARY KEY CLUSTERED
(
	[Id] ASC
),
 CONSTRAINT [IX_graffiti_RolePermissions_RoleName] UNIQUE NONCLUSTERED
(
	[RoleName] ASC
)
) ON [PRIMARY]
END


/****** Object:  Table [dbo].[graffiti_RoleCategoryPermissions]    Script Date: 04/25/2008 09:00:00 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_RoleCategoryPermissions]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_RoleCategoryPermissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](128)  NOT NULL,
	[CategoryId] [int] NOT NULL,
	[HasRead] [bit] NOT NULL CONSTRAINT [DF_graffiti_RoleCategoryPermissions_HasRead]  DEFAULT ((0)),
	[HasEdit] [bit] NOT NULL CONSTRAINT [DF_graffiti_RoleCategoryPermissions_HasEdit]  DEFAULT ((0)),
	[HasPublish] [bit] NOT NULL CONSTRAINT [DF_graffiti_RoleCategoryPermissions_HasPublish]  DEFAULT ((0)),
	[CreatedBy] [nvarchar](128)  NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](128)  NULL,
 CONSTRAINT [PK_graffiti_RoleCategoryPermissions] PRIMARY KEY CLUSTERED
(
	[Id] ASC
),
 CONSTRAINT [IX_graffiti_RoleCategoryPermissions_RoleName] UNIQUE NONCLUSTERED
(
	[RoleName] ASC,
	[CategoryId] ASC
)
) ON [PRIMARY]
END


/****** Object:  View [dbo].[graffiti_TagWeights]    Script Date: 08/26/2007 22:49:51 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_TagWeights]', N'V') IS NULL
EXEC dbo.sp_executesql @statement = N'
create view [dbo].[graffiti_TagWeights]
AS
select
	t.[Name], count(*) AS [WEIGHT]
from
	[dbo].[graffiti_Tags] t
group by
	t.[Name]
'
