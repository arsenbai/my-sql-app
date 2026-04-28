UPDATE dbo.test 
SET project_id = @NewProjectId 
WHERE id = @TargetTestId;