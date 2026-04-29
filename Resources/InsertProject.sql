INSERT INTO dbo.project 
(name) 
OUTPUT INSERTED.id
VALUES (@Name);