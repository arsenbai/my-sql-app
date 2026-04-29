using MySqlApp.Data.Repositories;
using NUnit.Framework;
using System.Data;

namespace MySqlApp.Steps
{
    public class AuthorSteps
    {
        private IDbConnection _db { get; }
        private string _newAuthorGuid = null!;
        private string _newAuthorName = null!;
        private string _newAuthorLogin = null!;
        private string _newAuthorEmail = null!;
        internal long NewAuthorId;

        public AuthorSteps(IDbConnection db)
        {
            _db = db;
            _newAuthorGuid = $"{Guid.NewGuid():N}";
        }

        internal void CreateNewAuthor()
        {
            _newAuthorName = $"autotest_name_{_newAuthorGuid}";
            _newAuthorLogin = $"autotest_login_{_newAuthorGuid}";
            _newAuthorEmail = $"{_newAuthorLogin}@mail.com";
            NewAuthorId = AuthorRepository.InsertAuthorAndReturnAuthorId(
                _db, 
                _newAuthorName, 
                _newAuthorLogin, 
                _newAuthorEmail);
        }

        internal void DeleteNewAuthor(long newAuthorId)
        {
            AuthorRepository.DeleteAuthorById(_db, newAuthorId);
            Assert.That(
                !AuthorRepository.CheckAuthorExists(_db, newAuthorId),
                $"The created author has NOT been deleted: author_id={newAuthorId}"
                );
        }

    }
}
