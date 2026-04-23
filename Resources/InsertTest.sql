INSERT INTO dbo.test 
(name, status_id, method_name, project_id, session_id, start_time, end_time, env, browser, author_id)
OUTPUT INSERTED.id
VALUES (@Name, @StatusId, @MethodName, @ProjectId, @SessionId, @StartTime, @EndTime, @Env, @Browser, @AuthorId);