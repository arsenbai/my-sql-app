SELECT id as Id, name as Name, login as Login, email as Email 
FROM dbo.author 
WHERE login = @Login;