using Dapper;
using MySqlApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MySqlApp.Steps
{
    internal class TestSteps
    {
        internal static long InsertTestAndReturnTestId(IDbConnection db,
                                        string name,
                                        int? statusId,
                                        string methodName,
                                        long projectId,
                                        long sessionId,
                                        DateTime? startTime,
                                        DateTime? endTime,
                                        string env,
                                        string? browser,
                                        long? authorId)
        {
            string insertSql = Utils.SqlLoader.Load("InsertTest.sql");
            long newlyAddedId = db.ExecuteScalar<long>(
            insertSql,
            new { Name = name, 
                StatusId = statusId,
                MethodName = methodName,
                ProjectId = projectId,
                SessionId = sessionId,
                StartTime = startTime,
                EndTime = endTime,
                Env = env,
                Browser = browser,
                AuthorId = authorId
            });
            return newlyAddedId;
        }

        internal static Test? GetTestById(IDbConnection db, long id)
        {
            string getTestByIdSql = Utils.SqlLoader.Load("GetTestById.sql");
            return db.QuerySingleOrDefault<Test>(
                getTestByIdSql,
                new { Id = id });
        }

        internal static List<Test> GetListOfTestsByBrowser(IDbConnection db, string browser)
        {
            string getTestByBrowserSql = Utils.SqlLoader.Load("GetTestByBrowser.sql");
            return db.Query<Test>(
                getTestByBrowserSql,
                new { Browser = browser }).ToList();
        }

        internal static void UpdateTestAuthor(IDbConnection db, long? newAuthorId, long targetTestId)
        {
            string updateTestAuthorSql = Utils.SqlLoader.Load("UpdateTestAuthor.sql");
            db.Execute(
                updateTestAuthorSql,
                new { NewAuthorId = newAuthorId, TargetTestId = targetTestId});
        }

        internal static void DeleteTestById(IDbConnection db, long targetTestId)
        {
            string deleteTestByIdSql = Utils.SqlLoader.Load("DeleteTestById.sql");
            db.Execute(
                deleteTestByIdSql,
                new { TargetTestId = targetTestId });
        }

        internal static bool CheckTestExists(IDbConnection db, long id)
        {
            Test? test = GetTestById(db, id);
            if (test is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
