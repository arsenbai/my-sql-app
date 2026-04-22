using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using MySqlApp.Data;
using MySqlApp.Models;
using NUnit.Framework;

// --- CONFIGURATION ---

string DB_NAME = Environment.GetEnvironmentVariable("DB_NAME");

if (string.IsNullOrWhiteSpace(DB_NAME))
{
    throw new InvalidOperationException("DB_NAME is not set.");
}


string connectionString = $"Server=CMDB-224899;Database={DB_NAME};Trusted_Connection=True;TrustServerCertificate=True;";

Console.WriteLine(">>>Connecting to database..");



// --- THE LOGIC ---
using (IDbConnection db = new SqlConnection(connectionString))
{
    try
    {
        // --- SCENARIO 1 ----

        /* PRECONDITION:
         * - Add new author in table ‘author’
         */

        // create new author record in dbo.author table
        string newAuthorLogin = "new_user_login";
        List<string> authorColumnNames = new List<string>() { "name", "login", "email" };
        List<object> newAuthorValues = new List<object>() { "new_user_name", newAuthorLogin, "new_user@email.com" };
        SqlUtilities.InsertRecord(db, "dbo.author", authorColumnNames, newAuthorValues);


        /* STEP
         * Select all the tests performed on the Chrome
         * browser and replace the author with the one
         * created in the preconditions.
         */

        // Select all the tests performed on the Chrome browser
        string browserChrome = "chrome";
        var testsWithChrome = SqlUtilities.GetTestDataByBrowser(db, browserChrome);
        Console.WriteLine($"Found {testsWithChrome.Count} tests with {browserChrome}...");


        // get ID of newly added author
        List<Author> newAuthorInList = SqlUtilities.GetAuthorDataByLogin(db, newAuthorLogin);
        Console.WriteLine(@"Number of authors with login '{0}' in db", newAuthorInList.Count);
        long newAuthorId;
        if (newAuthorInList.Count == 1)
        {
            newAuthorId = newAuthorInList[0].Id;
        }
        else
        {
            throw new ArgumentException($"DB must not have duplicate logins for authors. There are more than one unique logins for '{newAuthorInList[0].Name}'", nameof(newAuthorInList.Count));
        }

        // replace the author with the one created in the preconditions for all tests performed on the Chrome browser
        string updateSql = "UPDATE dbo.test SET author_id = @newId WHERE id = @testId";
        foreach (var itemTestWithChrome in testsWithChrome)
        {
            db.Execute(updateSql, new { newId = newAuthorId, testId = itemTestWithChrome.Id });
        }

        // Expected results: check whether Author has been replaced
        var testsListForCheck = SqlUtilities.GetTestDataByBrowser(db, browserChrome);
        bool isAuthorReplaced = true;
        foreach (var itemWithReplacedAuthor in testsListForCheck)
        {
            if (itemWithReplacedAuthor.Id == newAuthorId)
            {
                continue;
            }
            else
            {
                isAuthorReplaced = false;
                break;
            }
        }
        Assert.IsTrue(isAuthorReplaced);

        /* STEP
         * Select all tests performed on Firefox,
         * copy their contents excluding ‘ID’ and ‘browser’.
         * Add new tests with this content to the database. Browser - Safari
         */

        string browserFirefox = "firefox";
        var testsWithFirefox = SqlUtilities.GetTestDataByBrowser(db, browserFirefox);
        Console.WriteLine($"Found {testsWithFirefox.Count} tests with {browserFirefox}...");

        string insertSql = "INSERT INTO dbo.test (name, status_id, method_name, project_id, session_id, start_time, end_time, env, browser, author_id) VALUES (@Name, @StatusId, @MethodName, @ProjectId, @SessionId, @StartTime, @EndTime, @Env, @Browser, @AuthorId)";
        string newBrowserToBeAppled = "safari";
        foreach (var item in testsWithFirefox)
        {
            db.Execute(insertSql, new { Name = item.Name, StatusId = item.StatusId, MethodName = item.MethodName, ProjectId = item.ProjectId, SessionId = item.SessionId, StartTime = item.StartTime, EndTime = item.EndTime, Env = item.Env, Browser = newBrowserToBeAppled, AuthorId = item.AuthorId });
        }

        var testsWithNewlyAppliedBrowser = SqlUtilities.GetTestDataByBrowser(db, newBrowserToBeAppled);
        Console.WriteLine($"Found {testsWithNewlyAppliedBrowser.Count} tests with {newBrowserToBeAppled}...");

        // Expected results: New tests for Safari have been added, their contents match the tests for Firefox
        int sizeOfLists = testsWithNewlyAppliedBrowser.Count;

        bool isSafariTestMacthedFirefoxTest = true;
        List<bool> conditions = new List<bool>();
        for (int i = 0; i < sizeOfLists; i++)
        {
            conditions.Add(testsWithNewlyAppliedBrowser[i].Name.Equals(testsWithFirefox[i].Name));
            conditions.Add(testsWithNewlyAppliedBrowser[i].MethodName.Equals(testsWithFirefox[i].MethodName));
            conditions.Add(testsWithNewlyAppliedBrowser[i].StartTime.Equals(testsWithFirefox[i].StartTime));
            conditions.Add(testsWithNewlyAppliedBrowser[i].EndTime.Equals(testsWithFirefox[i].EndTime));
            conditions.Add(testsWithNewlyAppliedBrowser[i].Env.Equals(testsWithFirefox[i].Env));
            conditions.Add(testsWithNewlyAppliedBrowser[i].AuthorId.Equals(testsWithFirefox[i].AuthorId));
            if (conditions.All(b => b))
            {
                conditions.Clear();
                continue;
            }
            else
            {
                isSafariTestMacthedFirefoxTest = false;
                break;
            }
        }
        Assert.IsTrue(isSafariTestMacthedFirefoxTest);

        /* STEP
         * For each of the added Safari tests, change the author to Null
         */
        db.Execute($"UPDATE dbo.test SET author_id = NULL WHERE browser = @newBrowser", new { newBrowser = newBrowserToBeAppled });

        /* STEP
         * Delete from the database all Safari tests
         * whose author is null
         */
        db.Execute($"DELETE FROM dbo.test WHERE author_id IS NULL AND browser = @newBrowser", new { newBrowser = newBrowserToBeAppled });


        /* PostConditions:
         * - delete the created author, check that it has been deleted.
         */
        object objNewAuthorId = new { newAuthId = newAuthorId };

        // delete test of new author
        db.Execute("DELETE FROM dbo.test WHERE author_id = @newAuthId", objNewAuthorId);

        // delete the created author
        db.Execute("DELETE FROM dbo.author WHERE id = @newAuthId", objNewAuthorId);

        // check the created author has been deleted
        var author = db.QueryFirstOrDefault<Author>("SELECT * FROM dbo.author WHERE id = @newAuthId", objNewAuthorId);
        Assert.IsNull(author);


        // --- SCENARIO 2 ----

        /* PRECONDTIONS:
         * - Add new author to ‘author’ table.
         * - Add new project to ‘project’ table.
         */

        var columnNames = new List<string>() { "name", "login", "email" };
        string secondNewAuthorLogin = "newlogin";
        var values = new List<object>() { "newName", secondNewAuthorLogin, "new@email.com" };
        SqlUtilities.InsertRecord(db, "dbo.author", columnNames, values);

        string newProjectName = "__newProject";
        SqlUtilities.InsertRecord(db, "dbo.project", new List<string> { "name" }, new List<object> { newProjectName });


        /* STEP
         * Select all the tests performed on the Chrome browser, copy their contents, replace the author and the project with those created in preconditions, save the new tests in the database.
         * EXPECTED RESULT:
         * New tests have been added, author and project from preconditions.
         */


        var newAuthorIdNumber = db.ExecuteScalar<long>("SELECT id FROM dbo.author WHERE login = @secondNewAuthorLogin", new { secondNewAuthorLogin = secondNewAuthorLogin });
        var newProjectIdNumber = db.ExecuteScalar<long>("SELECT id FROM dbo.project WHERE name = @newProjectName", new { newProjectName = newProjectName });


        var columnNamesList = new List<string>() { "name", "status_id", "method_name", "project_id", "session_id", "start_time", "end_time", "env", "browser", "author_id" };


        Console.WriteLine("--///" + SqlUtilities.GetTestDataByBrowser(db, browserChrome));
        foreach (var item in SqlUtilities.GetTestDataByBrowser(db, browserChrome))
        {
            SqlUtilities.InsertRecord(
                db,
                "dbo.test",
                columnNamesList,
                new List<object>() { item.Name, item.StatusId, item.MethodName, newProjectIdNumber, item.SessionId, item.StartTime, item.EndTime, item.Env, item.Browser, newAuthorIdNumber }
                );
        }

        /* STEP
         * Replace ‘environment’ with all the tests created in the previous step
         * EXPECTED RESULT:
         * ‘env’ was changed
         */
        var newEnv = "_NEW_Env";
        db.Execute("UPDATE dbo.test SET env = @newEnv WHERE author_id = @newAuthorIdNumber", new { newEnv = newEnv, newAuthorIdNumber = newAuthorIdNumber });


        /*
        For all tests with the SKIPPED status, replace the status with Failed
        EXPECTED RESULT:
        ‘Status’ was changed
        */
        string changeStatusSql = "UPDATE dbo.test SET status_id = 2 WHERE status_id = 3";
        db.Execute(changeStatusSql);



    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

