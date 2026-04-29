using MySqlApp.Data.Enums;
using MySqlApp.Steps;
using NUnit.Framework;

namespace MySqlApp.Tests
{
    [TestFixture]
    public class ScenarioTwo: BaseTest
    {
        private AuthorSteps _authorSteps = null!;
        private TestSteps _testSteps = null!;
        private ProjectSteps _projectSteps = null!;
        

        private List<long>? idsOfClonedTests;

        [SetUp]
        public void SetUp()
        {
            _authorSteps = new AuthorSteps(db);
            _testSteps = new TestSteps(db);
            _projectSteps = new ProjectSteps(db);

            _authorSteps.CreateNewAuthor();
            _projectSteps.CreateNewProject();
        }

        [Test]
        public void ScenarioTwoTest()
        {
            idsOfClonedTests = _testSteps.CloneTestsWithNewAuthorAndNewProject(BrowserType.chrome, _authorSteps.NewAuthorId, _projectSteps.NewProjectId);
            _testSteps.ReplaceEnvironmentForClonedTest(idsOfClonedTests);
            _testSteps.ReplaceStatusForAllTests(Status.SKIPPED, Status.FAILED);
        }

        [TearDown]
        public void TearDown()
        {
            _testSteps.DeleteTestsByIds(idsOfClonedTests!);
            _authorSteps.DeleteNewAuthor(_authorSteps.NewAuthorId);
            _projectSteps.DeleteNewProject(_projectSteps.NewProjectId);

        }
    }
}
