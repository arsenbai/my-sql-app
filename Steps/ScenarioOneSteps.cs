using MySqlApp.Data.Enums;
using MySqlApp.Data.Repositories;
using MySqlApp.Models;
using NUnit.Framework;
using System.Data;

namespace MySqlApp.Steps
{
    public class ScenarioOneSteps
    {
        public IDbConnection Db { get; }
        public long NewAuthorId { get; set; }

        public ScenarioOneSteps(IDbConnection db)
        {
            Db = db;
        }

        internal void ReplaceAuthorForBrowserTests(BrowserType browser, string newAuthorLogin)
        {
            List<Test> testsWithOldAuthor = TestRepository.GetListOfTestsByBrowser(Db, browser);

            NewAuthorId = AuthorRepository.GetAuthorByLogin(Db, newAuthorLogin)!.Id;
            foreach (var testItemWithOldAuthor in testsWithOldAuthor)
            {
                TestRepository.UpdateTestAuthor(Db, NewAuthorId, testItemWithOldAuthor.Id);
            }

            List<Test> testsWithNewAuthor = TestRepository.GetListOfTestsByBrowser(Db, browser);
            List<bool> conditionsToCheckAuthorReplacement = new List<bool>();
            foreach (var testItem in testsWithNewAuthor)
            {
                conditionsToCheckAuthorReplacement.Add(testItem.AuthorId.Equals(NewAuthorId));
            }
            Assert.That(conditionsToCheckAuthorReplacement.All(b => b), "Author has NOT been replaced.");
        }

        internal List<long> CloneTestsToBrowser(
            BrowserType browserOfOriginalTests,
            BrowserType browserOfCopyTests)
        {
            List<long> idsNewlyAddedTests = new List<long>();
            List<Test> originalTests = TestRepository.GetListOfTestsByBrowser(Db, browserOfOriginalTests);

            foreach (var itemOriginalTest in originalTests)
            {
                idsNewlyAddedTests.Add(
                    TestRepository.InsertTestAndReturnTestId(Db,
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
                    TestRepository.GetTestById(Db, idItem)!
                    );
            }

            List<bool> conditions = new List<bool>();
            conditions.Add(
                newlyAddedTests.Select(t => t.Name).ToHashSet().SetEquals(originalTests.Select(t => t.Name))
                );
            conditions.Add(
                newlyAddedTests.Select(t => t.StatusId).ToHashSet().SetEquals(originalTests.Select(t => t.StatusId))
            );
            conditions.Add(
                newlyAddedTests.Select(t => t.MethodName).ToHashSet().SetEquals(originalTests.Select(t => t.MethodName))
            );
            conditions.Add(
                newlyAddedTests.Select(t => t.ProjectId).ToHashSet().SetEquals(originalTests.Select(t => t.ProjectId))
            );
            conditions.Add(
                newlyAddedTests.Select(t => t.SessionId).ToHashSet().SetEquals(originalTests.Select(t => t.SessionId))
            );
            conditions.Add(
                newlyAddedTests.Select(t => t.StartTime).ToHashSet().SetEquals(originalTests.Select(t => t.StartTime))
            );
            conditions.Add(
                newlyAddedTests.Select(t => t.EndTime).ToHashSet().SetEquals(originalTests.Select(t => t.EndTime))
            );
            conditions.Add(
                newlyAddedTests.Select(t => t.Env).ToHashSet().SetEquals(originalTests.Select(t => t.Env))
            );
            conditions.Add(
                newlyAddedTests.Select(t => t.AuthorId).ToHashSet().SetEquals(originalTests.Select(t => t.AuthorId))
            );

            Assert.That(conditions.All(b => b),
                $"New tests for {browserOfCopyTests.ToString()} have NOT been added AND/OR their contents DO NOT match the tests for {browserOfOriginalTests.ToString()}");

            return idsNewlyAddedTests;
        }

        internal void SetAuthorNull(List<long> idsNewlyAddedTests)
        {
            foreach (var idOfNewTest in idsNewlyAddedTests)
            {
                TestRepository.UpdateTestAuthor(Db, null, idOfNewTest);
            }

            bool authorIsChanged = true;
            foreach (var idOfNewTest in idsNewlyAddedTests)
            {
                if (TestRepository.GetTestById(Db, idOfNewTest)!.AuthorId is not null)
                {
                    authorIsChanged = false;
                    break;
                }
            }
            Assert.That(authorIsChanged, "Author has NOT been changed to null for the newly added tests.");
        }

        internal void DeleteBrowserTestsWhereAuthorIsNull(BrowserType targetBrowser)
        {
            List<Test> filteredTests = TestRepository.GetListOfTestsByBrowser(Db, targetBrowser);
            foreach (var targetTest in filteredTests)
            {
                if (targetTest.AuthorId is null)
                {
                    TestRepository.DeleteTestById(Db, targetTest.Id);
                }
            }

            bool targetTestsAreDeleted = true;
            foreach (var testToCheck in filteredTests)
            {
                if (TestRepository.CheckTestExists(Db, testToCheck.Id))
                {
                    targetTestsAreDeleted = false;
                    break;
                }
            }
            Assert.That(targetTestsAreDeleted, $"{targetBrowser.ToString()} tests where author is null have NOT been deleted.");
        }

        internal void DeleteNewAuthor()
        {
            AuthorRepository.DeleteAuthorById(Db, NewAuthorId);
            Assert.That(
                !AuthorRepository.CheckAuthorExists(Db, NewAuthorId),
                $"the created author has NOT been deleted: author_id={NewAuthorId}"
                );
        }


    }
}
