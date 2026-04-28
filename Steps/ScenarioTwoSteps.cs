using MySqlApp.Data.Enums;
using MySqlApp.Data.Repositories;
using MySqlApp.Models;
using NUnit.Framework;
using System.Data;

namespace MySqlApp.Steps
{
    public class ScenarioTwoSteps
    {
        public IDbConnection Db { get; }
        public long NewAuthorId { get; set; }
        public long NewProjectId { get; set; }
        public List<long> idsOfClonedTests = new List<long>();
        

        public ScenarioTwoSteps(IDbConnection db)
        {
            Db = db;
        }

        internal void CloneTestsWithNewAuthorAndNewProject(BrowserType browser, string newAuthorLogin, string newProjectName)
        {
            List<Test> originalTests = TestRepository.GetListOfTestsByBrowser(Db, browser);
            NewAuthorId = AuthorRepository.GetAuthorByLogin(Db, newAuthorLogin)!.Id;
            NewProjectId = ProjectRepository.GetProjectByName(Db, newProjectName)!.Id;

            foreach (var itemOriginalTest in originalTests)
            {
                idsOfClonedTests.Add(
                    TestRepository.InsertTestAndReturnTestId(Db,
                                        itemOriginalTest.Name,
                                        itemOriginalTest.StatusId,
                                        itemOriginalTest.MethodName,
                                        NewProjectId,
                                        itemOriginalTest.SessionId,
                                        itemOriginalTest.StartTime,
                                        itemOriginalTest.EndTime,
                                        itemOriginalTest.Env,
                                        itemOriginalTest.Browser,
                                        NewAuthorId)
                    );
            }


            bool newTestsAreAdded = true;
            bool authorAndProjectAreReplaced = true;
            foreach (var idOfClonedTest in idsOfClonedTests)
            {
                if (TestRepository.CheckTestExists(Db, idOfClonedTest))
                {
                    Test? clonedTest = TestRepository.GetTestById(Db, idOfClonedTest);
                    if (clonedTest!.AuthorId == NewAuthorId && clonedTest.ProjectId == NewProjectId)
                    {
                        continue;
                    }
                    else
                    {
                        authorAndProjectAreReplaced = false;
                        break;
                    }
                }
                else
                {
                    newTestsAreAdded = false;
                    break;
                }
            }
            Assert.That(newTestsAreAdded, "New tests have NOT been added.");
            Assert.That(authorAndProjectAreReplaced, "Author and project have NOT been replaced.");
        }

        internal void ReplaceEnvironmentForClonedTest(string newEnv)
        {
            foreach (var idOfClonedTest in idsOfClonedTests)
            {
                TestRepository.UpdateTestEnv(Db, newEnv, idOfClonedTest);
            }
        }

        internal void ReplaceStatusForAllTests(Status originalStatus, Status newStatus)
        {
            List<long> idsOfTestsWithOriginalStatus = TestRepository.GetListOfTestsByStatus(Db, originalStatus).Select(t => t.Id).ToList();
            List<bool> conditions = new List<bool>();

            foreach (var testId in idsOfTestsWithOriginalStatus)
            {
                TestRepository.UpdateTestStatus(Db, newStatus, testId);
                conditions.Add(
                    TestRepository.GetTestById(Db, testId)!.StatusId == (int) newStatus);
            }

            Assert.That(conditions.All(b => b), "Status was NOT changed.");
        }
    }
}
