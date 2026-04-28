using MySqlApp.Data.Enums;
using MySqlApp.Data.Repositories;
using MySqlApp.Steps;
using NUnit.Framework;

namespace MySqlApp.Tests
{
    [TestFixture]
    public class ScenarioTwo: BaseTest
    {
        private ScenarioTwoSteps _steps = null!;
        private string _newAuthorName = null!;
        private string _newAuthorLogin = null!;
        private string _newAuthorEmail = null!;
        private string _newProjectName = null!;
        private readonly string _newEnv = "ARSEN_BAISEUPOV";

        [SetUp]
        public void SetUp()
        {
            _steps = new ScenarioTwoSteps(db);

            string newAuthorGuid = $"{Guid.NewGuid():N}";
            _newAuthorName = $"autotest_name_{newAuthorGuid}";
            _newAuthorLogin = $"autotest_login_{newAuthorGuid}";
            _newAuthorEmail = $"{_newAuthorLogin}@mail.com";
            AuthorRepository.InsertAuthor(db, _newAuthorName, _newAuthorLogin, _newAuthorEmail);
            
            _newProjectName = $"autotest_project_name_{Guid.NewGuid():N}";
            ProjectRepository.InsertProject(db, _newProjectName);
        }

        [Test]
        public void Scenario_Two()
        {
            _steps.CloneTestsWithNewAuthorAndNewProject(BrowserType.chrome, _newAuthorLogin, _newProjectName);
            _steps.ReplaceEnvironmentForClonedTest(_newEnv);
            _steps.ReplaceStatusForAllTests(Status.SKIPPED, Status.FAILED);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var clonedTestId in _steps.idsOfClonedTests)
            {
                TestRepository.DeleteTestById(db, clonedTestId);
            }
            AuthorRepository.DeleteAuthorById(db, _steps.NewAuthorId);
            ProjectRepository.DeleteProjectById(db, _steps.NewProjectId);
        }
    }
}
