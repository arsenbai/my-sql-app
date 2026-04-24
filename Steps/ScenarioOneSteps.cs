using MySqlApp.Data.Repositories;
using MySqlApp.Models;
using NUnit.Framework;
using System.Data;

namespace MySqlApp.Steps
{
    internal class ScenarioOneSteps
    {





        /* STEP
         * Select all the tests performed on the Chrome browser 
         * and replace the author with the one created in the preconditions.
         */
        internal static void SelectTestsPerformedOnChrome_ThenReplaceAuthorWithCreatedInPrecondition(IDbConnection db, string newAuthorLogin, long newAuthorId, string browserChrome)
        {
            // Select all the tests performed on the Chrome browser 
            List<Test> testsWithOldAuthor = TestRepository.GetListOfTestsByBrowser(db, browserChrome);

            // Replace the author with the one created in the preconditions.
            newAuthorId = AuthorRepository.GetAuthorByLogin(db, newAuthorLogin)!.Id;
            foreach (var testItemWithOldAuthor in testsWithOldAuthor)
            {
                TestRepository.UpdateTestAuthor(db, newAuthorId, testItemWithOldAuthor.Id);
            }

            // Check EXPECTED RESULT - Author has been replaced
            List<Test> testsWithExpectedNewAuthor = TestRepository.GetListOfTestsByBrowser(db, browserChrome);
            List<bool> conditionsToCheckAuthorReplacement = new List<bool>();
            foreach (var testItemWithExpectedNewAuthor in testsWithExpectedNewAuthor)
            {
                conditionsToCheckAuthorReplacement.Add(testItemWithExpectedNewAuthor.AuthorId.Equals(newAuthorId));
            }
            Assert.That(conditionsToCheckAuthorReplacement.All(b => b), "Author has NOT been replaced.");
        }

        /* STEP
         * Select all tests performed on Firefox, 
         * copy their contents excluding ‘ID’ and ‘browser’.
         * Add new tests with this content to the database. Browser - Safari
         */
        internal static void SelectTestsPerformedOnFirefox_ThenCopyContentsWithSafariBrowser_AddNewTestsToDb(
            IDbConnection db, 
            string browserFirefox, 
            string browserSafari, 
            List<long> idsNewlyAddedTests, 
            List<Test> newlyAddedTests)
        {
            // Select all the tests performed on the Firefox browser 
            List<Test> testsWithFirefox = TestRepository.GetListOfTestsByBrowser(db, browserFirefox);

            // copy their contents excluding ‘ID’ and ‘browser’.
            // Add new tests with this content to the database. Browser - Safari
            foreach (var item in testsWithFirefox)
            {
                idsNewlyAddedTests.Add(
                    TestRepository.InsertTestAndReturnTestId(db,
                                        item.Name,
                                        item.StatusId,
                                        item.MethodName,
                                        item.ProjectId,
                                        item.SessionId,
                                        item.StartTime,
                                        item.EndTime,
                                        item.Env,
                                        browserSafari,
                                        item.AuthorId
                                        )
                    );
            }

            // Check EXPECTED RESULT - New tests for Safari have been added, their contents match the tests for Firefox

            foreach (var idItem in idsNewlyAddedTests)
            {
                newlyAddedTests.Add(
                    TestRepository.GetTestById(db, idItem)!
                    );
            }

            List<bool> conditionsForNewSafariTests = new List<bool>();
            conditionsForNewSafariTests.Add(
                newlyAddedTests.Select(t => t.Name).ToHashSet().SetEquals(testsWithFirefox.Select(t => t.Name))
                );
            conditionsForNewSafariTests.Add(
                newlyAddedTests.Select(t => t.StatusId).ToHashSet().SetEquals(testsWithFirefox.Select(t => t.StatusId))
            );
            conditionsForNewSafariTests.Add(
                newlyAddedTests.Select(t => t.MethodName).ToHashSet().SetEquals(testsWithFirefox.Select(t => t.MethodName))
            );
            conditionsForNewSafariTests.Add(
                newlyAddedTests.Select(t => t.ProjectId).ToHashSet().SetEquals(testsWithFirefox.Select(t => t.ProjectId))
            );
            conditionsForNewSafariTests.Add(
                newlyAddedTests.Select(t => t.SessionId).ToHashSet().SetEquals(testsWithFirefox.Select(t => t.SessionId))
            );
            conditionsForNewSafariTests.Add(
                newlyAddedTests.Select(t => t.StartTime).ToHashSet().SetEquals(testsWithFirefox.Select(t => t.StartTime))
            );
            conditionsForNewSafariTests.Add(
                newlyAddedTests.Select(t => t.EndTime).ToHashSet().SetEquals(testsWithFirefox.Select(t => t.EndTime))
            );
            conditionsForNewSafariTests.Add(
                newlyAddedTests.Select(t => t.Env).ToHashSet().SetEquals(testsWithFirefox.Select(t => t.Env))
            );
            conditionsForNewSafariTests.Add(
                newlyAddedTests.Select(t => t.AuthorId).ToHashSet().SetEquals(testsWithFirefox.Select(t => t.AuthorId))
            );

            Assert.That(conditionsForNewSafariTests.All(b => b),
                "New tests for Safari have NOT been added AND/OR their contents DO NOT match the tests for Firefox");
        }

        /* STEP
         * For each of the added Safari tests, change the author to Null
         */
        internal static void ChangeAuthorToNullForAllNewlyAddedSafariTest(IDbConnection db, List<long> idsNewlyAddedTests)
        {
            foreach (var _idItem in idsNewlyAddedTests)
            {
                TestRepository.UpdateTestAuthor(db, null, _idItem);
            }

            // Check EXPECTED RESULT - Author has been changed
            bool authorIsChanged = true;
            foreach (var _idItem in idsNewlyAddedTests)
            {
                if (TestRepository.GetTestById(db, _idItem)!.AuthorId is not null)
                {
                    authorIsChanged = false;
                    break;
                }
            }
            Assert.That(authorIsChanged, "Author has NOT been changed to NULL for the newly added Safari tests.");
        }

        /* STEP
         * Delete from the database all Safari tests whose author is null
         */
        internal static void DeleteAllSafariTestWithAuthorNull(IDbConnection db, string browserSafari)
        {
            List<Test> safariTests = TestRepository.GetListOfTestsByBrowser(db, browserSafari);
            foreach (var testItem in safariTests)
            {
                if (testItem.AuthorId is null)
                {
                    TestRepository.DeleteTestById(db, testItem.Id);
                }
            }

            // Check EXPECTED RESULT - Safari tests whose author is null have been deleted
            bool safariTestsWithAuthorNullIsDeleted = true;
            foreach (var itemToCheck in safariTests)
            {
                if (TestRepository.CheckTestExists(db, itemToCheck.Id))
                {
                    safariTestsWithAuthorNullIsDeleted = false;
                    break;
                }
            }
            Assert.That(safariTestsWithAuthorNullIsDeleted, "Safari tests whose author is null have NOT been deleted.");
        }


    }
}
