CREATE PROCEDURE [dbo].[DeleteMessagesById] 
	@ids AS [dbo].[MessageIdsType] READONLY
AS
	BEGIN
		SET NOCOUNT ON;

		DELETE [dbo].[Messages] WHERE Id IN (SELECT Id from @ids);
	END
GO