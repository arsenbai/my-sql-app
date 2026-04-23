using Dapper;
using Microsoft.Data.SqlClient;
using MySqlApp.Data;
using MySqlApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MySqlApp.Steps
{
    internal class AuthorSteps
    {
        internal static void InsertAuthor(IDbConnection db, string newAuthorName, string newAuthorLogin, string newAuthorEmail)
        {
            using (var transaction = db.BeginTransaction())
            { 
                string insertSql = Utils.SqlLoader.Load("InsertAuthor.sql");
                var rowsAffected = db.Execute(
                    insertSql,
                    new { Name = newAuthorName, Login = newAuthorLogin, Email = newAuthorEmail},
                    transaction: transaction);
                transaction.Commit();
                Console.WriteLine($"{rowsAffected} rows inserted.");
            }
        }

        internal static Author? GetAuthorById(IDbConnection db, long id)
        {
            string getAuthorByIdSql = Utils.SqlLoader.Load("GetAuthorById.sql");
            return db.QuerySingleOrDefault<Author>(
                getAuthorByIdSql,
                new { Id = id});
        }

        internal static Author? GetAuthorByLogin(IDbConnection db, string login)
        {
            string getAuthorByLoginSql = Utils.SqlLoader.Load("GetAuthorByLogin.sql");
            return db.QueryFirstOrDefault<Author>(
                getAuthorByLoginSql, 
                new { Login = login });
        }

        internal static void DeleteAuthorById(IDbConnection db, long targetAuthorId)
        {
            using (var transaction = db.BeginTransaction())
            { 
                string deleteAuthorByIdSql = Utils.SqlLoader.Load("DeleteAuthorById.sql");
                db.Execute(
                    deleteAuthorByIdSql,
                    new { TargetAuthorId = targetAuthorId },
                    transaction: transaction);
                transaction.Commit();
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
