CREATE TABLE [dbo].[Messages]
(
	[Id] INT NOT NULL IDENTITY PRIMARY KEY, 
	[Message] NVARCHAR(200) NOT NULL, 
	[DelayDateTime] DATETIME NOT NULL
)
GO

CREATE NONCLUSTERED INDEX IX_Messages_DelayDateTime
 ON [dbo].[Messages] ([DelayDateTime]);   
GO
