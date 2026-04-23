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
            string insertSql = Utils.SqlLoader.Load("InsertAuthor.sql");
            db.Execute(
                insertSql,
                new { Name = newAuthorName, Login = newAuthorLogin, Email = newAuthorEmail});
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
            string deleteAuthorByIdSql = Utils.SqlLoader.Load("DeleteAuthorById.sql");
            db.Execute(
                deleteAuthorByIdSql,
                new { TargetAuthorId = targetAuthorId });
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
