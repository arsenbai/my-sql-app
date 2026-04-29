
using Dapper;
using MySqlApp.Models;
using System.Data;

namespace MySqlApp.Data.Repositories
{
    internal class ProjectRepository
    {
        internal static long InsertProjectAndReturnProjectId(IDbConnection db, string newProjectName)
        {
            long newlyAddedId;
            using (var transaction = db.BeginTransaction())
            {
                string insertSql = Utils.SqlLoader.Load("InsertProject.sql");
                newlyAddedId = db.ExecuteScalar<long>(
                    insertSql,
                    param: new { Name = newProjectName },
                    transaction: transaction);
                try
                {
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return newlyAddedId;
        }

        internal static Project? GetProjectByName(IDbConnection db, string name)
        {
            string getProjectByName = Utils.SqlLoader.Load("GetProjectByName.sql");
            return db.QueryFirstOrDefault<Project>(
                getProjectByName,
                param: new { Name = name });
        }

        internal static void DeleteProjectById(IDbConnection db, long targetProjectId)
        {
            using (var transaction = db.BeginTransaction())
            {
                string deleteProjectByIdSql = Utils.SqlLoader.Load("DeleteProjectById.sql");
                db.Execute(
                    deleteProjectByIdSql,
                    param: new { TargetProjectId = targetProjectId },
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
    }
}
