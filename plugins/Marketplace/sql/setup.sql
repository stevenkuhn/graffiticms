SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[marketplace_ItemStatistics](
	[PostId] [int] NOT NULL,
	[DownloadCount] [int] NOT NULL,
	[RatingSum] [int] NOT NULL,
	[RatingCount] [int] NOT NULL,
 CONSTRAINT [PK_marketplace_ItemStatistics] PRIMARY KEY CLUSTERED 
(
	[PostId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[marketplace_ItemStatistics] ADD  CONSTRAINT [DF_marketplace_ItemStatistics_DownloadCount]  DEFAULT ((0)) FOR [DownloadCount]
GO

ALTER TABLE [dbo].[marketplace_ItemStatistics] ADD  CONSTRAINT [DF_marketplace_ItemStatistics_RatingSum]  DEFAULT ((0)) FOR [RatingSum]
GO

ALTER TABLE [dbo].[marketplace_ItemStatistics] ADD  CONSTRAINT [DF_marketplace_ItemStatistics_RatingCount]  DEFAULT ((0)) FOR [RatingCount]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[marketplace_ItemStats_Get_ByPost]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[marketplace_ItemStats_Get_ByPost]
GO

CREATE PROCEDURE [dbo].[marketplace_ItemStats_Get_ByPost]
(
	@postId int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT *
	FROM dbo.marketplace_ItemStatistics
	WHERE PostId = @postId

END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[marketplace_ItemStats_Get_ByCat]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[marketplace_ItemStats_Get_ByCat]
GO

CREATE PROCEDURE [dbo].[marketplace_ItemStats_Get_ByCat]
(
	@categoryID int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT p.Id as PostId, itemstats.*
	FROM dbo.marketplace_ItemStatistics itemstats
		inner join dbo.graffiti_Posts p on itemstats.PostId = p.Id
	WHERE p.CategoryId = @categoryID

END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[marketplace_ItemStats_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[marketplace_ItemStats_Update]
GO

CREATE PROCEDURE [dbo].[marketplace_ItemStats_Update]
(
	@postId int
)
AS
BEGIN
	
	SET Transaction Isolation Level Read UNCOMMITTED

	Update itemstats
	Set itemstats.DownloadCount = itemstats.DownloadCount + 1
	From dbo.marketplace_ItemStatistics itemstats
	Where itemstats.PostId = @postId
	
	-- if post was not found, add it
	IF @@ROWCOUNT > 0
	BEGIN
		Insert Into dbo.marketplace_ItemStatistics (PostId, DownloadCount)
		Values (@postId, 1)
	END

END

GO
