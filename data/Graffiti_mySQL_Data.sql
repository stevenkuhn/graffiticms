/*
ALTER TABLE graffiti_Categories NOCHECK CONSTRAINT ALL
GO

SET IDENTITY_INSERT graffiti_Categories ON 
PRINT 'Begin inserting data in graffiti_Categories'
*/
INSERT INTO graffiti_Categories(Id, Name, FormattedName, View, PostView, LinkName, FeedUrlOverride, Body, IsDeleted, Post_Count, UniqueId, ParentId, Type, ImageUrl)
VALUES(1, 'Uncategorized', 'uncategorized', NULL, NULL, 'uncategorized', NULL, NULL,  0, 0, '{fbfd6cb6-cb5c-445f-94d0-b4adfd899dbe}', 0, 0, NULL)
;
/*
SET IDENTITY_INSERT graffiti_Categories OFF 
ALTER TABLE graffiti_Categories CHECK CONSTRAINT ALL
GO
*/

/*
ALTER TABLE graffiti_Comments NOCHECK CONSTRAINT ALL
GO

SET IDENTITY_INSERT graffiti_Comments ON 
PRINT 'Begin inserting data in graffiti_Comments'
SET IDENTITY_INSERT graffiti_Comments OFF 
ALTER TABLE graffiti_Comments CHECK CONSTRAINT ALL
GO
*/

/*
ALTER TABLE graffiti_Logs NOCHECK CONSTRAINT ALL
GO

SET IDENTITY_INSERT graffiti_Logs ON 
PRINT 'Begin inserting data in graffiti_Logs'
SET IDENTITY_INSERT graffiti_Logs OFF 
ALTER TABLE graffiti_Logs CHECK CONSTRAINT ALL
GO
*/

/*
ALTER TABLE graffiti_ObjectStore NOCHECK CONSTRAINT ALL
GO

SET IDENTITY_INSERT graffiti_ObjectStore ON 
PRINT 'Begin inserting data in graffiti_ObjectStore'
*/
INSERT INTO graffiti_ObjectStore (Id, Name, Data, Type, CreatedOn, ModifiedOn, Content_Type, Version, UniqueId)
VALUES(1, '{a000e7f7-2916-404e-988c-7d5a7eb0db9e}', '<?xml version="1.0" encoding="utf-16"?><SiteOptionWidget xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Id>a000e7f7-2916-404e-988c-7d5a7eb0db9e</Id><Title>Site Options</Title><Location>Right</Location><Order>0</Order></SiteOptionWidget>', 'Graffiti.Core.SiteOptionWidget, Graffiti.Core', '2007-08-26 22:59:25', '2007-08-26 22:59:28', 'xml/widget', 0, '{a000e7f7-2916-404e-988c-7d5a7eb0db9e}')
;
/*
SET IDENTITY_INSERT graffiti_ObjectStore OFF 
ALTER TABLE graffiti_ObjectStore CHECK CONSTRAINT ALL
GO
*/


