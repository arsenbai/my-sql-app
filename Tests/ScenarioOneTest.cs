using MySqlApp.Data.Connection;
using MySqlApp.Data.Repositories;
using MySqlApp.Models;
using MySqlApp.Steps;
using NUnit.Framework;
using System.Data;
using MySqlApp.Utils;

namespace MySqlApp.Tests
{
    // SCENARIO 1
    [TestFixture]
    public class ScenarioOneTest
    {
        // db
        private ConnectionToDb _connection = null!;
        private IDbConnection _db = null!;

        // new author
        private string _newAuthorName = null!;
        private string _newAuthorLogin = null!;
        private string _newAuthorEmail = null!;

        // browsers
        private readonly string _browserChrome = "chrome";
        private readonly string _browserFirefox = "firefox";
        private readonly string _browserSafari = "safari";

        // lists for newly added tests
        private List<Test> _newlyAddedTests = new List<Test>();
        private List<long> _idsNewlyAddedTests = new List<long>();



        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // create Db connection
            _connection = ConnectionToDb.Instance;
            _db = _connection.CreateConnection();

            if (_db.State != ConnectionState.Open)
            { 
                _db.Open();
            }


            // PRECONDITION: create new author record in dbo.author table
            string newAuthorGuid = $"{Guid.NewGuid():N}";
            _newAuthorName = $"autotest_name_{newAuthorGuid}";
            _newAuthorLogin = $"autotest_login_{newAuthorGuid}";
            _newAuthorEmail = $"{_newAuthorLogin}@mail.com";
            AuthorRepository.InsertAuthor(_db, _newAuthorName, _newAuthorLogin, _newAuthorEmail);
        }

        [Test]
        public void Scenario_One() 
        {
            ScenarioOneSteps.SelectTestsPerformedOnChrome_ThenReplaceAuthorWithCreatedInPrecondition(
                _db, 
                _newAuthorLogin, 
                _browserChrome);
            ScenarioOneSteps.SelectTestsPerformedOnFirefox_ThenCopyContentsWithSafariBrowser_AddNewTestsToDb(
                _db, 
                _browserFirefox, 
                _browserSafari,
                _idsNewlyAddedTests,
                _newlyAddedTests);
            ScenarioOneSteps.ChangeAuthorToNullForAllNewlyAddedSafariTest(_db, _idsNewlyAddedTests);
            ScenarioOneSteps.DeleteAllSafariTestWithAuthorNull(_db, _browserSafari);
        }



        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // POSTCONDITION delete the created author, check that it has been deleted
            AuthorRepository.DeleteAuthorById(_db, SharedTestState.NewAuthorId);
            Assert.That(
                !AuthorRepository.CheckAuthorExists(_db, SharedTestState.NewAuthorId),
                $"the created author has NOT been deleted: author_id={SharedTestState.NewAuthorId}"
                );
            if (_db.State == ConnectionState.Open)
            {
                _db.Close();
            }
        }
    }
}
