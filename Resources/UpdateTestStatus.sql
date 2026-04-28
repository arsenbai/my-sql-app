UPDATE dbo.test 
SET status_id = @NewStatusId
WHERE id = @TargetTestId;