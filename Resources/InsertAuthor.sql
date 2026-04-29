INSERT INTO dbo.author 
(name, login, email) 
OUTPUT INSERTED.id
VALUES (@Name, @Login, @Email);