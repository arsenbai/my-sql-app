using MySqlApp.Data;
using MySqlApp.Data.Connection;
using MySqlApp.Models;
using MySqlApp.Steps;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata;
using System.Text;

namespace MySqlApp.Tests
{
    // SCENARIO 1
    [TestFixture]
    public class ScenarioOneTest
    {
        private ConnectionToDb _connection = null!;
        private IDbConnection _db = null!;
        private long _newAuthorId;
        private string _newAuthorName = null!;
        private string _newAuthorLogin = null!;
        private string _newAuthorEmail = null!;
        private readonly string _browserChrome = "chrome";
        private readonly string _browserFirefox = "firefox";
        private readonly string _browserSafari = "safari";
        private List<Test> newlyAddedTests = new List<Test>();
        private List<long> idsNewlyAddedTests = new List<long>();


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // create Db connection
            _connection = Data.Connection.ConnectionToDb.Instance;
            _db = _connection.CreateConnection();
            _db.Open();

            // PRECONDITION: create new author record in dbo.author table
            string newAuthorGuid = $"{Guid.NewGuid():N}";
            _newAuthorName = $"autotest_name_{newAuthorGuid}";
            _newAuthorLogin = $"autotest_login_{newAuthorGuid}";
            _newAuthorEmail = $"{_newAuthorLogin}@mail.com";
            AuthorSteps.InsertAuthor(_db, _newAuthorName, _newAuthorLogin, _newAuthorEmail);
        }

        [Test]
        /* STEP
         * Select all the tests performed on the Chrome browser 
         * and replace the author with the one created in the preconditions.
         */
        public void SelectTestsPerformedOnChrome_ThenReplaceAuthorWithCreatedInPrecondition()
        {
            // Select all the tests performed on the Chrome browser 
            List<Test> testsWithOldAuthor = TestSteps.GetListOfTestsByBrowser(_db, _browserChrome);

            // Replace the author with the one created in the preconditions.
            _newAuthorId = AuthorSteps.GetAuthorByLogin(_db, _newAuthorLogin)!.Id;
            foreach (var testItemWithOldAuthor in testsWithOldAuthor)
            {
                TestSteps.UpdateTestAuthor(_db, _newAuthorId, testItemWithOldAuthor.Id);
            }

            // Check EXPECTED RESULT - Author has been replaced
            List<Test> testsWithExpectedNewAuthor = TestSteps.GetListOfTestsByBrowser(_db, _browserChrome);
            List<bool> conditionsToCheckAuthorReplacement = new List<bool>();
            foreach (var testItemWithExpectedNewAuthor in testsWithExpectedNewAuthor)
            {
                conditionsToCheckAuthorReplacement.Add(testItemWithExpectedNewAuthor.AuthorId.Equals(_newAuthorId));
            }
            Assert.That(conditionsToCheckAuthorReplacement.All(b => b), "Author has NOT been replaced.");
        }

        [Test]
        /* STEP
         * Select all tests performed on Firefox, 
         * copy their contents excluding ‘ID’ and ‘browser’.
         * Add new tests with this content to the database. Browser - Safari
         */
        public void SelectTestsPerformedOnFirefox_ThenCopyContentsWithSafariBrowser_AddNewTestsToDb()
        {
            // Select all the tests performed on the Firefox browser 
            List<Test> testsWithFirefox = TestSteps.GetListOfTestsByBrowser(_db, _browserFirefox);

            // copy their contents excluding ‘ID’ and ‘browser’.
            // Add new tests with this content to the database. Browser - Safari
            foreach (var item in testsWithFirefox)
            {
                idsNewlyAddedTests.Add(
                    TestSteps.InsertTestAndReturnTestId(_db,
                                        item.Name, 
                                        item.StatusId, 
                                        item.MethodName, 
                                        item.ProjectId, 
                                        item.SessionId, 
                                        item.StartTime, 
                                        item.EndTime, 
                                        item.Env, 
                                        _browserSafari, 
                                        item.AuthorId
                                        )
                    );
            }

            // Check EXPECTED RESULT - New tests for Safari have been added, their contents match the tests for Firefox

            foreach (var _idItem in idsNewlyAddedTests)
            {
                newlyAddedTests.Add(
                    TestSteps.GetTestById(_db, _idItem)
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

        [Test]
        /* STEP
         * For each of the added Safari tests, change the author to Null
         */
        public void ChangeAuthorToNullForAllNewlyAddedSafariTest()
        {
            foreach (var _idItem in idsNewlyAddedTests)
            {
                TestSteps.UpdateTestAuthor(_db, null, _idItem);
            }

            // Check EXPECTED RESULT - Author has been changed
            bool authorIsChanged = true;
            foreach (var _idItem in idsNewlyAddedTests)
            {
                if (TestSteps.GetTestById(_db, _idItem).AuthorId is not null)
                {
                    authorIsChanged = false;
                    break;
                }
            }
            Assert.That(authorIsChanged, "Author has NOT been changed to NULL for the newly added Safari tests.");
        }

        [Test]
        /* STEP
         * Delete from the database all Safari tests whose author is null
         */
        public void DeleteAllSafariTestWithAuthorNull()
        {
            List<Test> safariTests = TestSteps.GetListOfTestsByBrowser(_db, _browserSafari);
            foreach (var testItem in safariTests)
            {
                if (testItem.AuthorId is null)
                {
                    TestSteps.DeleteTestById(_db, testItem.Id);
                }
            }

            // Check EXPECTED RESULT - Safari tests whose author is null have been deleted
            bool safariTestsWithAuthorNullIsDeleted = true;
            foreach (var itemToCheck in safariTests)
            {
                if (TestSteps.CheckTestExists(_db, itemToCheck.Id))
                {
                    safariTestsWithAuthorNullIsDeleted = false;
                    break;
                }
            }
            Assert.That(safariTestsWithAuthorNullIsDeleted, "Safari tests whose author is null have NOT been deleted.");
        }


        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            //    // POSTCONDITION delete the created author, check that it has been deleted
            //    AuthorSteps.DeleteAuthorById(_db, _newAuthorId);
            //    Assert.That(
            //        !AuthorSteps.CheckAuthorExists(_db, _newAuthorId),
            //        $"the created author has NOT been deleted: author_id={_newAuthorId}"
            //        );
            _db.Close();
        }
    }
}
