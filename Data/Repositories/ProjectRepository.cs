
using Dapper;
using MySqlApp.Models;
using System.Data;

namespace MySqlApp.Data.Repositories
{
    internal class ProjectRepository
    {
        internal static void InsertProject(IDbConnection db, string newProjectName)
        {
            using (var transaction = db.BeginTransaction())
            {
                string insertSql = Utils.SqlLoader.Load("InsertProject.sql");
                var rowsAffected = db.Execute(
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
