using MySqlApp.Data.Enums;
using MySqlApp.Data.Repositories;
using MySqlApp.Models;
using NUnit.Framework;
using System.Data;

namespace MySqlApp.Steps
{
    public class TestSteps
    {
        private IDbConnection _db { get; }
        private readonly string _newEnv = "ARSEN_BAISEUPOV";

        public TestSteps(IDbConnection db)
        {
            _db = db;
        }

        private void AssertAllClonedTestsWithNewBrowserHaveIdenticalContentsWithOriginalTests(List<Test> clonedTests, List<Test> originalTests)
        {

            Assert.Multiple(() =>
            {
                Assert.That(
                    clonedTests.Select(t => t.Name).ToHashSet().SetEquals(originalTests.Select(t => t.Name)),
                    "Test Names DO NOT match");
                Assert.That(
                    clonedTests.Select(t => t.StatusId).ToHashSet().SetEquals(originalTests.Select(t => t.StatusId)),
                    "Test StatusIds DO NOT match");
                Assert.That(
                    clonedTests.Select(t => t.MethodName).ToHashSet().SetEquals(originalTests.Select(t => t.MethodName)),
                    "Test MethodNames DO NOT match");
                Assert.That(
                    clonedTests.Select(t => t.ProjectId).ToHashSet().SetEquals(originalTests.Select(t => t.ProjectId)),
                    "Test ProjectIds DO NOT match");
                Assert.That(
                    clonedTests.Select(t => t.SessionId).ToHashSet().SetEquals(originalTests.Select(t => t.SessionId)),
                    "Test SessionIds DO NOT match");
                Assert.That(
                    clonedTests.Select(t => t.StartTime).ToHashSet().SetEquals(originalTests.Select(t => t.StartTime)),
                    "Test StartTimes DO NOT match");
                Assert.That(
                    clonedTests.Select(t => t.EndTime).ToHashSet().SetEquals(originalTests.Select(t => t.EndTime)),
                    "Test EndTimes DO NOT match");
                Assert.That(
                    clonedTests.Select(t => t.Env).ToHashSet().SetEquals(originalTests.Select(t => t.Env)),
                    "Test Envs DO NOT match");
                Assert.That(
                    clonedTests.Select(t => t.AuthorId).ToHashSet().SetEquals(originalTests.Select(t => t.AuthorId)),
                    "Test AuthorIds DO NOT match");
            });
        }

        internal void ReplaceAuthorForBrowserTests(BrowserType browser, long newAuthorId)
        {
            List<Test> testsWithOldAuthor = TestRepository.GetListOfTestsByBrowser(_db, browser);

            foreach (var testItemWithOldAuthor in testsWithOldAuthor)
            {
                TestRepository.UpdateTestAuthor(_db, newAuthorId, testItemWithOldAuthor.Id);
            }

            List<Test> testsWithNewAuthor = TestRepository.GetListOfTestsByBrowser(_db, browser);
            List<bool> conditionsToCheckAuthorReplacement = new List<bool>();
            foreach (var testItem in testsWithNewAuthor)
            {
                conditionsToCheckAuthorReplacement.Add(testItem.AuthorId.Equals(newAuthorId));
            }
            Assert.That(conditionsToCheckAuthorReplacement.All(b => b), "Author has NOT been replaced.");
        }

        internal List<long> CloneTestsWithNewAuthorAndNewProject(
                                                    BrowserType browser,
                                                    long newAuthorId,
                                                    long newProjectId)
        {
            List<long> idsOfClonedTests = new List<long>();
            List<Test> originalTests = TestRepository.GetListOfTestsByBrowser(_db, browser);

            foreach (var itemOriginalTest in originalTests)
            {
                idsOfClonedTests.Add(
                    TestRepository.InsertTestAndReturnTestId(
                        _db,
                        itemOriginalTest.Name,
                        itemOriginalTest.StatusId,
                        itemOriginalTest.MethodName,
                        newProjectId,
                        itemOriginalTest.SessionId,
                        itemOriginalTest.StartTime,
                        itemOriginalTest.EndTime,
                        itemOriginalTest.Env,
                        itemOriginalTest.Browser,
                        newAuthorId)
                    );
            }


            bool newTestsAreAdded = true;
            bool authorAndProjectAreReplaced = true;
            foreach (var idOfClonedTest in idsOfClonedTests)
            {
                if (TestRepository.CheckTestExists(_db, idOfClonedTest))
                {
                    Test? clonedTest = TestRepository.GetTestById(_db, idOfClonedTest);
                    if (clonedTest!.AuthorId == newAuthorId && clonedTest.ProjectId == newProjectId)
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
            return idsOfClonedTests;
        }

        internal void ReplaceEnvironmentForClonedTest(List<long> idsOfClonedTests)
        {
            foreach (var idOfClonedTest in idsOfClonedTests)
            {
                TestRepository.UpdateTestEnv(_db, _newEnv, idOfClonedTest);
            }
        }

        internal void ReplaceStatusForAllTests(Status originalStatus, Status newStatus)
        {
            List<long> idsOfTestsWithOriginalStatus = TestRepository.GetListOfTestsByStatus(_db, originalStatus).Select(t => t.Id).ToList();
            List<bool> conditions = new List<bool>();

            foreach (var testId in idsOfTestsWithOriginalStatus)
            {
                TestRepository.UpdateTestStatus(_db, newStatus, testId);
                conditions.Add(
                    TestRepository.GetTestById(_db, testId)!.StatusId == (int)newStatus);
            }

            Assert.That(conditions.All(b => b), "Status was NOT changed.");
        }

        internal List<long> CloneTestsToBrowser(
            BrowserType browserOfOriginalTests,
            BrowserType browserOfCopyTests)
        {
            List<long> idsNewlyAddedTests = new List<long>();
            List<Test> originalTests = TestRepository.GetListOfTestsByBrowser(_db, browserOfOriginalTests);

            foreach (var itemOriginalTest in originalTests)
            {
                idsNewlyAddedTests.Add(
                    TestRepository.InsertTestAndReturnTestId(_db,
                                        itemOriginalTest.Name,
                                        itemOriginalTest.StatusId,
                                        itemOriginalTest.MethodName,
                                        itemOriginalTest.ProjectId,
                                        itemOriginalTest.SessionId,
                                        itemOriginalTest.StartTime,
                                        itemOriginalTest.EndTime,
                                        itemOriginalTest.Env,
                                        browserOfCopyTests.ToString(),
                                        itemOriginalTest.AuthorId)
                    );
            }

            List<Test> newlyAddedTests = new List<Test>();

            foreach (var idItem in idsNewlyAddedTests)
            {
                newlyAddedTests.Add(
                    TestRepository.GetTestById(_db, idItem)!
                    );
            }

            AssertAllClonedTestsWithNewBrowserHaveIdenticalContentsWithOriginalTests(newlyAddedTests, originalTests);

            return idsNewlyAddedTests;
        }

        internal void SetAuthorNull(List<long> idsNewlyAddedTests)
        {
            foreach (var idOfNewTest in idsNewlyAddedTests)
            {
                TestRepository.UpdateTestAuthor(_db, null, idOfNewTest);
            }

            bool authorIsChanged = true;
            foreach (var idOfNewTest in idsNewlyAddedTests)
            {
                if (TestRepository.GetTestById(_db, idOfNewTest)!.AuthorId is not null)
                {
                    authorIsChanged = false;
                    break;
                }
            }
            Assert.That(authorIsChanged, "Author has NOT been changed to null for the newly added tests.");
        }

        internal void DeleteBrowserTestsWhereAuthorIsNull(BrowserType targetBrowser)
        {
            List<Test> filteredTests = TestRepository.GetListOfTestsByBrowser(_db, targetBrowser);
            foreach (var targetTest in filteredTests)
            {
                if (targetTest.AuthorId is null)
                {
                    TestRepository.DeleteTestById(_db, targetTest.Id);
                }
            }

            bool targetTestsAreDeleted = true;
            foreach (var testToCheck in filteredTests)
            {
                if (TestRepository.CheckTestExists(_db, testToCheck.Id))
                {
                    targetTestsAreDeleted = false;
                    break;
                }
            }
            Assert.That(targetTestsAreDeleted, $"{targetBrowser.ToString()} tests where author is null have NOT been deleted.");
        }

        internal void DeleteTestsByIds(List<long> idsOfTests)
        {
            foreach (var testId in idsOfTests)
            {
                TestRepository.DeleteTestById(_db, testId);
            }
        }

    }
}
