/* in order of foreign key constraints */
drop table if exists graffiti_Tags
	, graffiti_Post_Statistics
	, graffiti_UserRoles
	, graffiti_Users
	, graffiti_Logs
	, graffiti_Comments
	, graffiti_ObjectStore
	, graffiti_VersionStore
	, graffiti_Posts
	, graffiti_Categories
	, graffiti_RolePermissions
	, graffiti_RoleCategoryPermissions
;

CREATE TABLE graffiti_Users(
	Id INT NOT NULL AUTO_INCREMENT,
	Name varchar(128) CHARACTER SET utf8  NOT NULL,
	Email varchar(128) CHARACTER SET utf8  NOT NULL,
	ProperName varchar(255) CHARACTER SET utf8  NULL,
	TimeZoneOffSet DOUBLE NOT NULL,
	Bio varchar(2000) CHARACTER SET utf8  NULL,
	Avatar varchar(255) CHARACTER SET utf8  NULL,
	PublicEmail varchar(255) CHARACTER SET utf8  NULL,
	WebSite varchar(255) CHARACTER SET utf8  NULL,
	Password varchar(128) CHARACTER SET utf8  NOT NULL,
	Password_Salt varchar(128) CHARACTER SET utf8  NOT NULL,
	PasswordFormat INT NOT NULL,
	UniqueId char(38) NOT NULL /*CONSTRAINT DF_graffiti_Users_UniqueId DEFAULT newid()*/,
	CONSTRAINT IX_graffiti_Users_Id PRIMARY KEY ( Id ), /* mySQL forces AUTO_INCREMENT columns to be the PK */
	CONSTRAINT PK_graffiti_Users UNIQUE INDEX ( Name ),
	CONSTRAINT IX_graffiti_Users_Email UNIQUE INDEX ( Email ),
	CONSTRAINT IX_graffiti_Users_UniqueId UNIQUE INDEX ( UniqueId )
)
;

/*drop table if exists graffiti_Categories
;*/

CREATE TABLE graffiti_Categories(
	Id INT NOT NULL AUTO_INCREMENT,
	Name varchar(255) CHARACTER SET utf8  NOT NULL,
	View varchar(64) CHARACTER SET utf8  NULL,
	PostView varchar(64) CHARACTER SET utf8  NULL,
	FormattedName varchar(255) CHARACTER SET utf8  NOT NULL,
	LinkName varchar(255) CHARACTER SET utf8  NOT NULL,
	FeedUrlOverride varchar(255) CHARACTER SET utf8  NULL,
	Body text CHARACTER SET utf8  NULL,
	IsDeleted boolean NOT NULL /*CONSTRAINT [DF_graffiti_Categories_IsDeleted] */ DEFAULT 0,
	Post_Count INT NOT NULL /*CONSTRAINT [DF_graffiti_Categories_Post_Count] */ DEFAULT 0,
	UniqueId char(38) NOT NULL /*CONSTRAINT [DF_graffiti_Categories_UniqueId]  DEFAULT (newid())*/,
	ParentId INT NOT NULL /*CONSTRAINT [DF_graffiti_Categories_ParentId] */ DEFAULT -1,
	Type INT NOT NULL /*CONSTRAINT [DF_graffiti_Categories_Type] */ DEFAULT 0,
	ImageUrl varchar(255) CHARACTER SET utf8  NULL,
	MetaDescription varchar(255) CHARACTER SET utf8  NULL,
	MetaKeywords varchar(255) CHARACTER SET utf8  NULL,
	FeaturedId INT NOT NULL /*CONSTRAINT [DF_graffiti_Categories_FeaturedId] */ DEFAULT 0,
	SortOrderTypeId INT NOT NULL /*CONSTRAINT [DF_graffiti_Categories_SortTypeTypeId] */ DEFAULT 0,
	ExcludeSubCategoryPosts boolean NOT NULL /*CONSTRAINT [DF_graffiti_Categories_ExcludeSubCategoryPosts] */ DEFAULT 0,
	CONSTRAINT PK_graffiti_Categories PRIMARY KEY ( Id ),
	CONSTRAINT IX_graffiti_Categories_LinkName_ParentId UNIQUE INDEX ( LinkName, ParentId ),
	CONSTRAINT IX_graffiti_Categories_Name_ParentId UNIQUE INDEX ( Name, ParentId )
)
;

/*drop table if exists graffiti_Posts
;*/

CREATE TABLE graffiti_Posts(
	Id INT NOT NULL AUTO_INCREMENT,
	Title varchar(255) CHARACTER SET utf8  NOT NULL,
	PostBody text CHARACTER SET utf8  NOT NULL,
	CreatedOn datetime NOT NULL,
	ModifiedOn datetime NOT NULL,
	Status INT NOT NULL /*CONSTRAINT [DF_graffiti_Posts_Status] */ DEFAULT 1,
	Content_Type varchar(50) CHARACTER SET utf8  NOT NULL,
	Name varchar(255) CHARACTER SET utf8  NOT NULL,
	Comment_Count INT NOT NULL /*CONSTRAINT [DF_graffiti_Posts_Comment_Count] */ DEFAULT 0,
	Tag_List varchar(512) CHARACTER SET utf8  NULL,
	CategoryId INT NOT NULL,
	Version INT NOT NULL /*CONSTRAINT [DF_graffiti_Posts_Version] */ DEFAULT 0,
	ModifiedBy varchar(128) CHARACTER SET utf8  NULL,
	CreatedBy varchar(128) CHARACTER SET utf8  NOT NULL,
	ExtendedBody text CHARACTER SET utf8  NULL,
	IsDeleted boolean NOT NULL /*CONSTRAINT [DF_graffiti_Posts_IsDeleted] */ DEFAULT 0,
	Published datetime NOT NULL,
	Pending_Comment_Count INT NOT NULL /*CONSTRAINT [DF_graffiti_Posts_Pending_Comment_Count] */ DEFAULT 0,
	Views INT NOT NULL /*CONSTRAINT [DF_graffiti_Posts_Views] */ DEFAULT 0,
	UniqueId char(38) NOT NULL /*CONSTRAINT [DF_graffiti_Posts_UniqueId]  DEFAULT (newid())*/,
	EnableComments boolean NOT NULL /*CONSTRAINT [DF_graffiti_Posts_EnableComments] */ DEFAULT 1,
	PropertyKeys text CHARACTER SET utf8 NULL,
	PropertyValues text CHARACTER SET utf8 NULL,
	UserName varchar(128) CHARACTER SET utf8  NOT NULL,
	Notes varchar(2000) CHARACTER SET utf8  NULL,
	ImageUrl varchar(255) CHARACTER SET utf8  NULL,
	MetaDescription varchar(255) CHARACTER SET utf8  NULL,
	MetaKeywords varchar(255) CHARACTER SET utf8  NULL,
	IsPublished boolean NOT NULL /*CONSTRAINT [DF_graffiti_Posts_IsPublished] */ DEFAULT 0,
	SortOrder INT NOT NULL /*CONSTRAINT [DF_graffiti_Posts_SortOrder] */ DEFAULT 0,
	ParentId INT NOT NULL /*CONSTRAINT [DF_graffiti_Posts_ParentId] */ DEFAULT 0,
	IsHome boolean NOT NULL /*CONSTRAINT [DF_graffiti_Posts_IsHome] */ DEFAULT 0,
	HomeSortOrder INT NOT NULL /*CONSTRAINT [DF_graffiti_Posts_HomeSortOrder] */ DEFAULT 0,
	CONSTRAINT PK_graffiti_Posts PRIMARY KEY ( Id ),
	CONSTRAINT IX_graffiti_Posts_CategoryId_Name UNIQUE INDEX ( CategoryId, Name ),
	CONSTRAINT FOREIGN KEY FK_graffiti_Posts_graffiti_Categories ( CategoryId )
		REFERENCES graffiti_Categories ( Id )
)
;

/*drop table if exists graffiti_VersionStore
;*/

CREATE TABLE graffiti_VersionStore(
	Id INT NOT NULL AUTO_INCREMENT,
	UniqueId char(38) NOT NULL /*CONSTRAINT [DF_graffiti_VersionStore_UniqueId]  DEFAULT (newid())*/,
	Data text CHARACTER SET utf8  NOT NULL,
	Type varchar(128) CHARACTER SET utf8  NOT NULL,
	Version INT NOT NULL,
	CreatedBy varchar(128) CHARACTER SET utf8  NOT NULL,
	CreatedOn datetime NOT NULL,
	Name varchar(255) CHARACTER SET utf8  NOT NULL,
	ItemId INT NOT NULL /*CONSTRAINT [DF_graffiti_VersionStore_ItemId] */ DEFAULT 0,
	Notes varchar(2000) CHARACTER SET utf8  NULL,
	CONSTRAINT PK_graffiti_VersionStore PRIMARY KEY ( Id )
)
;

/*drop table if exists graffiti_ObjectStore
;*/

CREATE TABLE graffiti_ObjectStore(
	Id INT NOT NULL AUTO_INCREMENT,
	Name varchar(128) CHARACTER SET utf8  NOT NULL,
	Data text CHARACTER SET utf8  NOT NULL,
	Type varchar(255) CHARACTER SET utf8  NOT NULL,
	CreatedOn datetime NOT NULL,
	ModifiedOn datetime NOT NULL,
	Content_Type varchar(128) CHARACTER SET utf8  NOT NULL,
	Version INT NOT NULL,
	UniqueId char(38) NOT NULL /*CONSTRAINT [DF_graffiti_ObjectStore_UniqueId]  DEFAULT (newid())*/,
	CONSTRAINT PK_ObjectStore PRIMARY KEY ( Id ),
	CONSTRAINT IX_graffiti_ObjectStore_Name UNIQUE INDEX ( Name )
)
;

/*drop table if exists graffiti_Comments
;*/

CREATE TABLE graffiti_Comments(
	Id INT NOT NULL AUTO_INCREMENT,
	PostId INT NOT NULL,
	Body varchar(4000) CHARACTER SET utf8  NOT NULL,
	CreatedBy varchar(128) CHARACTER SET utf8  NULL,
	Published datetime NOT NULL,
	Name varchar(128) CHARACTER SET utf8  NOT NULL,
	IsPublished boolean NOT NULL,
	Version INT NOT NULL,
	WebSite varchar(512) CHARACTER SET utf8  NULL,
	SpamScore INT NOT NULL,
	IPAddress varchar(64) CHARACTER SET utf8  NULL,
	ModifiedOn datetime NOT NULL,
	ModifiedBy varchar(128) CHARACTER SET utf8  NULL,
	Email varchar(128) CHARACTER SET utf8  NULL,
	IsDeleted boolean NOT NULL /*CONSTRAINT [DF_graffiti_Comments_IsDeleted] */ DEFAULT 0,
	UniqueId char(38) NOT NULL /*CONSTRAINT [DF_graffiti_Comments_UniqueId]  DEFAULT (newid())*/,
	UserName varchar(128) CHARACTER SET utf8  NULL,
	IsTrackback boolean NOT NULL /*CONSTRAINT [DF_graffiti_Comments_IsTrackback] */ DEFAULT 0,
	CONSTRAINT PK_graffiti_Comments PRIMARY KEY ( Id ),
	INDEX IX_graffiti_Comments_PostId ( PostId ),
	CONSTRAINT FK_graffiti_Comments_graffiti_Posts FOREIGN KEY ( PostId )
		REFERENCES graffiti_Posts ( Id )
)
;

/*drop table if exists graffiti_Logs
;*/

CREATE TABLE graffiti_Logs(
	Id INT NOT NULL AUTO_INCREMENT,
	Type INT NOT NULL,
	Title varchar(255) CHARACTER SET utf8  NOT NULL,
	Message varchar(2000) CHARACTER SET utf8  NOT NULL,
	CreatedBy varchar(128) CHARACTER SET utf8  NOT NULL,
	CreatedOn datetime NOT NULL,
	CONSTRAINT PK_graffiti_Logs PRIMARY KEY ( Id )
)
;

/*drop table if exists graffiti_UserRoles
;*/

CREATE TABLE graffiti_UserRoles(
	Id INT NOT NULL AUTO_INCREMENT,
	UserId INT NOT NULL,
	RoleName varchar(128) CHARACTER SET utf8  NOT NULL,
	CONSTRAINT PK_graffiti_UserRoles PRIMARY KEY ( Id ),
	CONSTRAINT IX_graffiti_UserRoles_RoleName_UserId UNIQUE INDEX ( RoleName, UserId ),
	CONSTRAINT FK_graffiti_UserRoles_graffiti_Users FOREIGN KEY ( UserId )
		REFERENCES graffiti_Users ( Id )
)
;

/*drop table if exists graffiti_Post_Statistics
;*/

CREATE TABLE graffiti_Post_Statistics(
	Id INT NOT NULL AUTO_INCREMENT,
	PostId INT NOT NULL,
	DateViewed datetime NOT NULL,
	CONSTRAINT PK_graffiti_Post_Statistics PRIMARY KEY ( Id ),
	CONSTRAINT FK_graffiti_Post_Statistics_graffiti_Posts FOREIGN KEY ( PostId )
		REFERENCES graffiti_Posts ( Id )
)
;

/*drop table if exists graffiti_Tags
;*/

CREATE TABLE graffiti_Tags(
	Id INT NOT NULL AUTO_INCREMENT,
	Name varchar(128) CHARACTER SET utf8  NOT NULL,
	PostId INT NOT NULL,
	CONSTRAINT PK_graffiti_Tags PRIMARY KEY ( Id ),
	INDEX IX_graffiti_Tags_PostId ( PostId ),
	CONSTRAINT IX_graffiti_Tags_Name_PostId UNIQUE INDEX ( Name, PostId ),
	CONSTRAINT FK_graffiti_Tags_graffiti_Posts FOREIGN KEY ( PostId )
		REFERENCES graffiti_Posts ( Id )
)
;

/*drop table if exists graffiti_RolePermissions
;*/

CREATE TABLE graffiti_RolePermissions(
	Id INT NOT NULL AUTO_INCREMENT,
	RoleName varchar(128) CHARACTER SET utf8  NOT NULL,
	HasRead boolean NOT NULL /*CONSTRAINT [DF_graffiti_RolePermissions_HasRead] */ DEFAULT 0,
	HasEdit boolean NOT NULL /*CONSTRAINT [DF_graffiti_RolePermissions_HasEdit] */ DEFAULT 0,
	HasPublish boolean NOT NULL /*CONSTRAINT [DF_graffiti_RolePermissions_HasPublish] */ DEFAULT 0,
	CreatedBy varchar(128) CHARACTER SET utf8  NULL,
	ModifiedOn datetime NOT NULL,
	ModifiedBy varchar(128) CHARACTER SET utf8  NULL,
	CONSTRAINT PK_graffiti_RolePermissions PRIMARY KEY ( Id ),
	INDEX IX_graffiti_RolePermissions_RoleName ( RoleName )
)
;

/*drop table if exists graffiti_RoleCategoryPermissions
;*/

CREATE TABLE graffiti_RoleCategoryPermissions(
	Id INT NOT NULL AUTO_INCREMENT,
	RoleName varchar(128) CHARACTER SET utf8  NOT NULL,
	CategoryId INT NOT NULL,
	HasRead boolean NOT NULL /*CONSTRAINT [DF_graffiti_RoleCategoryPermissions_HasRead] */ DEFAULT 0,
	HasEdit boolean NOT NULL /*CONSTRAINT [DF_graffiti_RoleCategoryPermissions_HasEdit] */ DEFAULT 0,
	HasPublish boolean NOT NULL /*CONSTRAINT [DF_graffiti_RoleCategoryPermissions_HasPublish] */ DEFAULT 0,
	CreatedBy varchar(128) CHARACTER SET utf8  NULL,
	ModifiedOn datetime NOT NULL,
	ModifiedBy varchar(128) CHARACTER SET utf8  NULL,
	CONSTRAINT PK_graffiti_RoleCategoryPermissions PRIMARY KEY ( Id ),
	INDEX IX_graffiti_RoleCategoryPermissions_RoleName_CategoryId ( RoleName, CategoryId )
)
;

drop view if exists graffiti_TagWeights
;

create view graffiti_TagWeights
AS
select
	t.Name, count(*) AS `WEIGHT`
from
	graffiti_Tags t
group by
	t.Name
;
