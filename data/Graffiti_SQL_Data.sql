ALTER TABLE [dbo].[graffiti_Categories] NOCHECK CONSTRAINT ALL
GO
SET IDENTITY_INSERT [dbo].[graffiti_Categories] ON 
PRINT 'Begin inserting data in graffiti_Categories'
INSERT INTO [dbo].[graffiti_Categories] ([Id], [Name], [FormattedName], [View], [PostView], [LinkName], [FeedUrlOverride], [Body], [IsDeleted], [Post_Count], [UniqueId], [ParentId], [Type], [ImageUrl])
VALUES(1, N'Uncategorized', N'uncategorized', NULL, NULL, N'uncategorized', NULL, NULL,  0, 0, N'fbfd6cb6-cb5c-445f-94d0-b4adfd899dbe', 0, 0, NULL)
SET IDENTITY_INSERT [dbo].[graffiti_Categories] OFF 
ALTER TABLE [dbo].[graffiti_Categories] CHECK CONSTRAINT ALL
GO


ALTER TABLE [dbo].[graffiti_Comments] NOCHECK CONSTRAINT ALL
GO
SET IDENTITY_INSERT [dbo].[graffiti_Comments] ON 
PRINT 'Begin inserting data in graffiti_Comments'
SET IDENTITY_INSERT [dbo].[graffiti_Comments] OFF 
ALTER TABLE [dbo].[graffiti_Comments] CHECK CONSTRAINT ALL
GO


ALTER TABLE [dbo].[graffiti_Logs] NOCHECK CONSTRAINT ALL
GO
SET IDENTITY_INSERT [dbo].[graffiti_Logs] ON 
PRINT 'Begin inserting data in graffiti_Logs'
SET IDENTITY_INSERT [dbo].[graffiti_Logs] OFF 
ALTER TABLE [dbo].[graffiti_Logs] CHECK CONSTRAINT ALL
GO


ALTER TABLE [dbo].[graffiti_ObjectStore] NOCHECK CONSTRAINT ALL
GO
SET IDENTITY_INSERT [dbo].[graffiti_ObjectStore] ON 
PRINT 'Begin inserting data in graffiti_ObjectStore'
INSERT INTO [dbo].[graffiti_ObjectStore] ([Id], [Name], [Data], [Type], [CreatedOn], [ModifiedOn], [Content_Type], [Version], [UniqueId])
VALUES(1, N'a000e7f7-2916-404e-988c-7d5a7eb0db9e', N'<?xml version="1.0" encoding="utf-16"?>
<SiteOptionWidget xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Id>a000e7f7-2916-404e-988c-7d5a7eb0db9e</Id>
  <Title>Site Options</Title>
  <Location>Right</Location>
  <Order>0</Order>
</SiteOptionWidget>', N'Graffiti.Core.SiteOptionWidget, Graffiti.Core', convert(datetime, '2007-08-26 22:59:25', 120), convert(datetime, '2007-08-26 22:59:28', 120), N'xml/widget', 0, N'a000e7f7-2916-404e-988c-7d5a7eb0db9e')
SET IDENTITY_INSERT [dbo].[graffiti_ObjectStore] OFF 
ALTER TABLE [dbo].[graffiti_ObjectStore] CHECK CONSTRAINT ALL
GO

