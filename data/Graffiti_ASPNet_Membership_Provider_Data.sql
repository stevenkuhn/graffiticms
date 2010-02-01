ALTER TABLE [aspnet_Applications] NOCHECK CONSTRAINT ALL
GO

PRINT 'Begin inserting data in aspnet_Applications'
INSERT INTO [aspnet_Applications] ([ApplicationName], [LoweredApplicationName], [ApplicationId], [Description])
VALUES('/graffiti', '/graffiti', '39f9ed8e-0842-4912-a14a-19624b227bea', NULL)
ALTER TABLE [aspnet_Applications] CHECK CONSTRAINT ALL
GO



ALTER TABLE [aspnet_Paths] NOCHECK CONSTRAINT ALL
GO

PRINT 'Begin inserting data in aspnet_Paths'
ALTER TABLE [aspnet_Paths] CHECK CONSTRAINT ALL
GO



ALTER TABLE [aspnet_PersonalizationAllUsers] NOCHECK CONSTRAINT ALL
GO

PRINT 'Begin inserting data in aspnet_PersonalizationAllUsers'
ALTER TABLE [aspnet_PersonalizationAllUsers] CHECK CONSTRAINT ALL
GO



ALTER TABLE [aspnet_Roles] NOCHECK CONSTRAINT ALL
GO

PRINT 'Begin inserting data in aspnet_Roles'
INSERT INTO [aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description])
VALUES('39f9ed8e-0842-4912-a14a-19624b227bea', '34969392-34a0-4d79-9a00-fcf4dff45b60', 'gAdmin', 'gadmin', NULL)
ALTER TABLE [aspnet_Roles] CHECK CONSTRAINT ALL
GO



ALTER TABLE [aspnet_SchemaVersions] NOCHECK CONSTRAINT ALL
GO

PRINT 'Begin inserting data in aspnet_SchemaVersions'
INSERT INTO [aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion])
VALUES('common', '1', 1)
INSERT INTO [aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion])
VALUES('health monitoring', '1', 1)
INSERT INTO [aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion])
VALUES('membership', '1', 1)
INSERT INTO [aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion])
VALUES('personalization', '1', 1)
INSERT INTO [aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion])
VALUES('profile', '1', 1)
INSERT INTO [aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion])
VALUES('role manager', '1', 1)
ALTER TABLE [aspnet_SchemaVersions] CHECK CONSTRAINT ALL
GO



ALTER TABLE [aspnet_Users] NOCHECK CONSTRAINT ALL
GO

PRINT 'Begin inserting data in aspnet_Users'
INSERT INTO [aspnet_Users] ([ApplicationId], [UserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate])
VALUES('39f9ed8e-0842-4912-a14a-19624b227bea', '471bedfe-62be-4755-b7d0-f3109bdc9a95', 'admin', 'admin', NULL,  0, '2007-08-27 01:36:19')
ALTER TABLE [aspnet_Users] CHECK CONSTRAINT ALL
GO



ALTER TABLE [aspnet_UsersInRoles] NOCHECK CONSTRAINT ALL
GO

PRINT 'Begin inserting data in aspnet_UsersInRoles'
INSERT INTO [aspnet_UsersInRoles] ([UserId], [RoleId])
VALUES('471bedfe-62be-4755-b7d0-f3109bdc9a95', '34969392-34a0-4d79-9a00-fcf4dff45b60')
ALTER TABLE [aspnet_UsersInRoles] CHECK CONSTRAINT ALL
GO



ALTER TABLE [aspnet_WebEvent_Events] NOCHECK CONSTRAINT ALL
GO

PRINT 'Begin inserting data in aspnet_WebEvent_Events'
ALTER TABLE [aspnet_WebEvent_Events] CHECK CONSTRAINT ALL
GO



ALTER TABLE [aspnet_Membership] NOCHECK CONSTRAINT ALL
GO

PRINT 'Begin inserting data in aspnet_Membership'
INSERT INTO [aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment])
VALUES('39f9ed8e-0842-4912-a14a-19624b227bea', '471bedfe-62be-4755-b7d0-f3109bdc9a95', 'HdIu3VUDqDxvARwbg0NhQyQoJ68=', 1, '8IosIPwJ6C2D4JLN0NzKwA==', NULL, 'user@graffiticms.com', 'user@graffiticms.com', NULL, NULL, 1,  0, '2007-08-03 14:58:26', '2007-08-27 01:27:53', '2007-08-03 14:58:26', '1754-01-01 00:00:00', 0, '1754-01-01 00:00:00', 0, '1754-01-01 00:00:00', NULL)
ALTER TABLE [aspnet_Membership] CHECK CONSTRAINT ALL
GO



ALTER TABLE [aspnet_PersonalizationPerUser] NOCHECK CONSTRAINT ALL
GO

PRINT 'Begin inserting data in aspnet_PersonalizationPerUser'
ALTER TABLE [aspnet_PersonalizationPerUser] CHECK CONSTRAINT ALL
GO



ALTER TABLE [aspnet_Profile] NOCHECK CONSTRAINT ALL
GO

PRINT 'Begin inserting data in aspnet_Profile'
INSERT INTO [aspnet_Profile] ([UserId], [PropertyNames], [PropertyValuesString], [PropertyValuesBinary], [LastUpdatedDate])
VALUES('471bedfe-62be-4755-b7d0-f3109bdc9a95', 'bio:S:0:0:properName:S:0:9:webSite:S:9:20:', 'The Adminhttp://graffiti.com/', 0x, '2007-08-27 01:29:37')
ALTER TABLE [aspnet_Profile] CHECK CONSTRAINT ALL
GO



