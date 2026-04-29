using MySqlApp.Data.Enums;
using MySqlApp.Models;
using MySqlApp.Steps;
using NUnit.Framework;

namespace MySqlApp.Tests
{
    [TestFixture]
    public class ScenarioOne : BaseTest
    {
        private AuthorSteps _authorSteps = null!;
        private TestSteps _testSteps = null!;

        
        private List<long>? _idsNewlyAddedTests;
        


        [SetUp]
        public void SetUp()
        {
            _authorSteps = new AuthorSteps(db);
            _testSteps = new TestSteps(db);
            _authorSteps.CreateNewAuthor();
        }

        [Test]
        public void ScenarioOneTest() 
        {
            _testSteps.ReplaceAuthorForBrowserTests(BrowserType.chrome, _authorSteps.NewAuthorId);
            _idsNewlyAddedTests = _testSteps.CloneTestsToBrowser(BrowserType.firefox, BrowserType.safari);
            _testSteps.SetAuthorNull(_idsNewlyAddedTests);
            _testSteps.DeleteBrowserTestsWhereAuthorIsNull(BrowserType.safari);
        }



        [TearDown]
        public void TearDown()
        {
            _authorSteps.DeleteNewAuthor(_authorSteps.NewAuthorId);
        }

    }
}
