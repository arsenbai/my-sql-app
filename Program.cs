using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

// --- 1. CONFIGURATION ---
string connectionString = @"Server=ARSENROG;Database=TestDb;Trusted_Connection=True;TrustServerCertificate=True;";

Console.WriteLine(">>>Connecting to database..");

// --- 2. THE MODEL ---
// This class matches the 'dbo.test' table


// --- 3. THE LOGIC ---
using (IDbConnection db = new SqlConnection(connectionString))
{
    try
    {
        // --- SCENARIO 1 ----
        // Fetching tests with Chrome browser

        // string browserChrome = "chrome";
        // var testsWithChrome = SqlUtilities.GetTestDataByBrowser(db, browserChrome);
        // Console.WriteLine($"Found {testsWithChrome.Count} tests with {browserChrome}...");

        /*
        REPLACE CODE
        long newAuthorId = 32;
        */

        // string updateSql = "UPDATE dbo.test SET author_id = @newId WHERE id = @testId";

        // foreach (var t in testsWithChrome)
        // {
        //     db.Execute(updateSql, new { newId = newAuthorId, testId = t.Id });
        // }


        // Select all tests performed on Firefox, 
        // copy their contents excluding ‘ID’ and ‘browser’. 
        // Add new tests with this content to the database. Browser - Safari


        // string browserFirefox = "firefox";
        // var testsWithFirefox = SqlUtilities.GetTestDataByBrowser(db, browserFirefox);
        // Console.WriteLine($"Found {testsWithFirefox.Count} tests with {browserFirefox}...");
        // string insertSql = "INSERT INTO dbo.test (name, status_id, method_name, project_id, session_id, start_time, end_time, env, browser, author_id) VALUES (@Name, @StatusId, @MethodName, @ProjectId, @SessionId, @StartTime, @EndTime, @Env, @Browser, @AuthorId)";
        // foreach (var item in testsWithFirefox)
        // {
        //     db.Execute(insertSql, new { Name = item.Name, StatusId = item.StatusId, MethodName = item.MethodName, ProjectId = item.ProjectId, SessionId = item.SessionId, StartTime = item.StartTime, EndTime = item.EndTime, Env = item.Env, Browser = "safari", AuthorId = item.AuthorId });
        // }

        // string browserSafari = "safari";
        // var testsWithSafari = SqlUtilities.GetTestDataByBrowser(db, browserSafari);
        // Console.WriteLine($"Found {testsWithSafari.Count} tests with {browserSafari}...");

        // string deleteFromTestSql = "DELETE FROM dbo.test WHERE id = @Id";
        // foreach (var item in testsWithSafari)
        // {
        //     if (item.AuthorId == null)
        //     {
        //         db.Execute(deleteFromTestSql, new { Id = item.Id });
        //     }
        // }


        // // --- DELETE NEWLY ADDED AUTHOR AND HIS TESTS---
        // string deleteFromTestByNewAuthor = "DELETE FROM dbo.test WHERE author_id = 32;";
        // db.Execute(deleteFromTestByNewAuthor);
        // string deleteFromAuthorSql = "DELETE FROM dbo.author WHERE name = 'ArsenB'";
        // db.Execute(deleteFromAuthorSql);

        // --- SCENARIO 2 ----

        /*
        PRECONDTIONS:
        - Add new author to ‘author’ table.
        - Add new project to ‘project’ table.
        */

        // var columnNames = new List<string>() { "name", "login", "email" };
        // string newAuthorLogin = "newlogin";
        // var values = new List<object>() { "newName", newAuthorLogin, "new@email.com" };
        // SqlUtilities.InsertRecord(db, "dbo.author", columnNames, values);

        // string newProjectName = "__newProject";
        // SqlUtilities.InsertRecord(db, "dbo.project", new List<string> { "name" }, new List<object> { newProjectName });


        /*
        Select all the tests performed on the Chrome browser, copy their contents, replace the author and the project with those created in preconditions, save the new tests in the database.
        EXPECTED RESULT:
        New tests have been added, author and project from preconditions.
        */
        /*
        Replace ‘environment’ with all the tests created in the previous step
        EXPECTED RESULT:
        ‘env’ was changed
        */

        // var newAuthorIdNumber = db.ExecuteScalar<int>("SELECT id FROM dbo.author WHERE login = @newAuthorLogin", new { newAuthorLogin = newAuthorLogin });
        // var newProjectIdNumber = db.ExecuteScalar<int>("SELECT id FROM dbo.project WHERE name = @newProjectName", new { newProjectName = newProjectName });


        // var columnNamesList = new List<string>() { "name", "status_id", "method_name", "project_id", "session_id", "start_time", "end_time", "env", "browser", "author_id" };


        // foreach (var item in testsWithChrome)
        // {
        //     SqlUtilities.InsertRecord(
        //         db,
        //         "dbo.test",
        //         columnNamesList,
        //         new List<object>() { item.Name, item.StatusId, item.MethodName, newProjectIdNumber, item.SessionId, item.StartTime, item.EndTime, "_NEW_Env", item.Browser, newAuthorIdNumber }
        //         );
        // }


        /*
        For all tests with the SKIPPED status, replace the status with Failed
        EXPECTED RESULT:
        ‘Status’ was changed
        */
        // string changeStatusSql = "UPDATE dbo.test SET status_id = 2 WHERE status_id = 3";
        // db.Execute(changeStatusSql);



    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}


public class Test
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public int? StatusId { get; set; }
    public required string MethodName { get; set; }
    public long ProjectId { set; get; }
    public long SessionId { set; get; }
    public DateTime? StartTime { set; get; }
    public DateTime? EndTime { set; get; }
    public required string Env { get; set; }
    public string? Browser { get; set; }
    public long? AuthorId { get; set; }
}

// public class Project
// {
//     public long Id { get; set; }
//     public required string Name { get; set; }
// }

public static class SqlUtilities
{
    public static List<Test> GetTestDataByBrowser(IDbConnection db, string browser)
    {
        var sql = @"SELECT id as Id, name as Name, status_id as StatusId, method_name as MethodName, project_id as ProjectId, session_id as SessionId, start_time as StartTime, end_time as EndTime, env as Env, browser as Browser, author_id as AuthorId FROM dbo.test WHERE browser = @Browser";
        return db.Query<Test>(sql, new { Browser = browser }).ToList();
    }

    public static int InsertRecord(
        IDbConnection db,
        string tableName,
        List<string> columnNames,
        List<object> values)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name is required.");

        if (columnNames == null || values == null)
            throw new ArgumentNullException("Columns and values cannot be null.");

        if (columnNames.Count == 0)
            throw new ArgumentException("At least one column is required.");

        if (columnNames.Count != values.Count)
            throw new ArgumentException("Columns count must match values count.");

        // Build columns: name, status_id, browser
        string columns = string.Join(", ", columnNames);

        // Build params: @p0, @p1, @p2
        string parameters = string.Join(", ",
            columnNames.Select((x, i) => $"@p{i}"));

        string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

        var param = new DynamicParameters();

        for (int i = 0; i < values.Count; i++)
        {
            param.Add($"p{i}", values[i]);
        }

        return db.Execute(sql, param);
    }
}