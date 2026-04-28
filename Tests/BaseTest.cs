using MySqlApp.Data.Connection;
using NUnit.Framework;
using System.Data;

namespace MySqlApp.Tests
{
    public class BaseTest
    {
        internal static ConnectionToDb connection = null!;
        internal static IDbConnection db = null!;

        [OneTimeSetUp]
        public void BaseSetUp()
        {
            connection = ConnectionToDb.Instance;
            db = connection.CreateConnection();

            if (db.State != ConnectionState.Open)
            {
                db.Open();
            }
        }

        [OneTimeTearDown]
        public void BaseTearDown()
        {
            if (db.State == ConnectionState.Open)
            {
                db.Close();
            }
        }
    }
}
