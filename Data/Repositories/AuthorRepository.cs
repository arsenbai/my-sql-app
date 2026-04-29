using Dapper;
using MySqlApp.Models;
using System.Data;

namespace MySqlApp.Data.Repositories
{
    internal class AuthorRepository
    {
        internal static long InsertAuthorAndReturnAuthorId(IDbConnection db, string newAuthorName, string newAuthorLogin, string newAuthorEmail)
        {
            long newlyAddedId;
            using (var transaction = db.BeginTransaction())
            {
                string insertSql = Utils.SqlLoader.Load("InsertAuthor.sql");
                newlyAddedId = db.ExecuteScalar<long>(
                    insertSql,
                    param: new { Name = newAuthorName, Login = newAuthorLogin, Email = newAuthorEmail},
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

        internal static Author? GetAuthorById(IDbConnection db, long id)
        {
                string getAuthorByIdSql = Utils.SqlLoader.Load("GetAuthorById.sql");
            return db.QuerySingleOrDefault<Author>(
                getAuthorByIdSql,
                param: new { Id = id});
        }

        internal static Author? GetAuthorByLogin(IDbConnection db, string login)
        {
            string getAuthorByLoginSql = Utils.SqlLoader.Load("GetAuthorByLogin.sql");
            return db.QueryFirstOrDefault<Author>(
                getAuthorByLoginSql,
                param: new { Login = login });
        }

        internal static void DeleteAuthorById(IDbConnection db, long targetAuthorId)
        {
            using (var transaction = db.BeginTransaction())
            { 
                string deleteAuthorByIdSql = Utils.SqlLoader.Load("DeleteAuthorById.sql");
                db.Execute(
                    deleteAuthorByIdSql,
                    param: new { TargetAuthorId = targetAuthorId },
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

        internal static bool CheckAuthorExists(IDbConnection db, long id)
        {
            Author? author = GetAuthorById(db, id);
            if (author is not null)
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
