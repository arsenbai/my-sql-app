using Dapper;
using MySqlApp.Data.Enums;
using MySqlApp.Models;
using System.Data;

namespace MySqlApp.Data.Repositories
{
    internal class TestRepository
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
            long newlyAddedId;
            using (var transaction = db.BeginTransaction())
            { 
                string insertSql = Utils.SqlLoader.Load("InsertTest.sql");
                newlyAddedId = db.ExecuteScalar<long>(
                    insertSql,
                    param: new { Name = name, 
                        StatusId = statusId,
                        MethodName = methodName,
                        ProjectId = projectId,
                        SessionId = sessionId,
                        StartTime = startTime,
                        EndTime = endTime,
                        Env = env,
                        Browser = browser,
                        AuthorId = authorId
                    },
                    transaction: transaction);
                try
                {
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return newlyAddedId;
        }

        internal static Test? GetTestById(IDbConnection db, long id)
        {
            string getTestByIdSql = Utils.SqlLoader.Load("GetTestById.sql");
            return db.QuerySingleOrDefault<Test>(
                getTestByIdSql,
                param: new { Id = id });
        }

        internal static List<Test> GetListOfTestsByStatus(IDbConnection db, Status status)
        {
            string getTestByStatusSql = Utils.SqlLoader.Load("GetTestByStatus.sql");
            return db.Query<Test>(
                getTestByStatusSql,
                new { StatusId = (int) status}).ToList();
        }

        internal static List<Test> GetListOfTestsByBrowser(IDbConnection db, BrowserType browser)
        {
            string getTestByBrowserSql = Utils.SqlLoader.Load("GetTestByBrowser.sql");
            return db.Query<Test>(
                getTestByBrowserSql,
                new { Browser = browser.ToString() }).ToList();
        }

        internal static void UpdateTestAuthor(IDbConnection db, long? newAuthorId, long targetTestId)
        {
            using (var transaction = db.BeginTransaction())
            { 
                string updateTestAuthorSql = Utils.SqlLoader.Load("UpdateTestAuthor.sql");
                db.Execute(
                    updateTestAuthorSql,
                    param: new { NewAuthorId = newAuthorId, TargetTestId = targetTestId},
                    transaction: transaction);
                try
                {
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        internal static void UpdateTestProject(IDbConnection db, long? newProjectId, long targetTestId)
        {
            using (var transaction = db.BeginTransaction())
            {
                string updateTestProjectSql = Utils.SqlLoader.Load("UpdateTestProject.sql");
                db.Execute(
                    updateTestProjectSql,
                    param: new { NewProjectId = newProjectId, TargetTestId = targetTestId},
                    transaction: transaction);
                try
                {
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        internal static void UpdateTestEnv(IDbConnection db, string env, long targetTestId)
        {
            using (var transaction = db.BeginTransaction())
            {
                string updateTestEnvSql = Utils.SqlLoader.Load("UpdateTestEnv.sql");
                db.Execute(
                    updateTestEnvSql,
                    param: new { Env = env, TargetTestId = targetTestId},
                    transaction: transaction);
                try
                {
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        internal static void UpdateTestStatus(IDbConnection db, Status newStatus, long targetTestId)
        {
            using (var transaction = db.BeginTransaction())
            {
                string updateTestStatus = Utils.SqlLoader.Load("UpdateTestStatus.sql");
                db.Execute(
                    updateTestStatus,
                    param: new { NewStatusId = (int) newStatus, TargetTestId = targetTestId},
                    transaction: transaction);
                try
                {
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        internal static void DeleteTestById(IDbConnection db, long targetTestId)
        {
            using (var transaction = db.BeginTransaction()) 
            { 
                string deleteTestByIdSql = Utils.SqlLoader.Load("DeleteTestById.sql");
                db.Execute(
                    deleteTestByIdSql,
                    new { TargetTestId = targetTestId },
                    transaction: transaction);
                try
                {
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
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
