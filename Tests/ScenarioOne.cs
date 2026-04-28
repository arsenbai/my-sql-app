using MySqlApp.Data.Enums;
using MySqlApp.Data.Repositories;
using MySqlApp.Models;
using MySqlApp.Steps;
using NUnit.Framework;

namespace MySqlApp.Tests
{
    [TestFixture]
    public class ScenarioOne : BaseTest
    {
        private ScenarioOneSteps _steps = null!;
        private string _newAuthorName = null!;
        private string _newAuthorLogin = null!;
        private string _newAuthorEmail = null!;
        private List<long>? _idsNewlyAddedTests;


        [SetUp]
        public void SetUp()
        {
            _steps = new ScenarioOneSteps(db);

            string newAuthorGuid = $"{Guid.NewGuid():N}";
            _newAuthorName = $"autotest_name_{newAuthorGuid}";
            _newAuthorLogin = $"autotest_login_{newAuthorGuid}";
            _newAuthorEmail = $"{_newAuthorLogin}@mail.com";
            AuthorRepository.InsertAuthor(db, _newAuthorName, _newAuthorLogin, _newAuthorEmail);
        }

        [Test]
        public void Scenario_One() 
        {
            _steps.ReplaceAuthorForBrowserTests(BrowserType.chrome, _newAuthorLogin);
            _idsNewlyAddedTests = _steps.CloneTestsToBrowser(BrowserType.firefox, BrowserType.safari);
            _steps.SetAuthorNull(_idsNewlyAddedTests);
            _steps.DeleteBrowserTestsWhereAuthorIsNull(BrowserType.safari);
        }



        [TearDown]
        public void TearDown()
        {
            _steps.DeleteNewAuthor();
        }

    }
}
