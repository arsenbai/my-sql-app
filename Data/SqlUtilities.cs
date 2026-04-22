using Dapper;
using MySqlApp.Models;
using System.Data;

namespace MySqlApp.Data
{
    // --- THE SQL UTILITIES ---
    public static class SqlUtilities
    {
        internal static List<Author> GetAuthorDataByLogin(IDbConnection db, 
                                                        string login)
        {
            var sql = @"SELECT id as Id, name as Name, login as Login, email as Email FROM dbo.author WHERE login = @Login";
            return db.Query<Author>(sql, new { Login = login }).ToList();
        }
        internal static List<Test> GetTestDataByBrowser(IDbConnection db,
                                                      string browser)
        {
            var sql = @"SELECT id as Id, name as Name, status_id as StatusId, method_name as MethodName, project_id as ProjectId, session_id as SessionId, start_time as StartTime, end_time as EndTime, env as Env, browser as Browser, author_id as AuthorId FROM dbo.test WHERE browser = @Browser";
            var listResult = db.Query<Test>(sql, new { Browser = browser }).ToList();
            return listResult;
        }

        internal static List<Test> GetTestDataByAuthorId(IDbConnection db,
                                              long authorId)
        {
            var sql = @"SELECT id as Id, name as Name, status_id as StatusId, method_name as MethodName, project_id as ProjectId, session_id as SessionId, start_time as StartTime, end_time as EndTime, env as Env, browser as Browser, author_id as AuthorId FROM dbo.test WHERE author_id = @AuthorId";
            var listResult = db.Query<Test>(sql, new { AuthorId = authorId }).ToList();
            return listResult;
        }

        public static int InsertRecord(IDbConnection db,
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
}
