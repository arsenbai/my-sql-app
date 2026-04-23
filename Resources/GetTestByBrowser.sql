SELECT id as Id, name as Name, status_id as StatusId, method_name as MethodName, project_id as ProjectId, session_id as SessionId, start_time as StartTime, end_time as EndTime, env as Env, browser as Browser, author_id as AuthorId 
FROM dbo.test 
WHERE browser = @Browser;