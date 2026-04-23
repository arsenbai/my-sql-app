UPDATE dbo.test 
SET author_id = @NewAuthorId 
WHERE id = @TargetTestId;