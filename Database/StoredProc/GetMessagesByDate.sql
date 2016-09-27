CREATE PROCEDURE [dbo].[GetMessagesByDate]
	@maxDelayDateTime DATETIME,
	@count int = 100

AS
	SELECT TOP (@count) [Id], [Message], [DelayDateTime] 
	FROM [dbo].[Messages] WHERE DelayDateTime < @maxDelayDateTime
