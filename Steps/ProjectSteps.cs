using MySqlApp.Data.Repositories;
using System.Data;

namespace MySqlApp.Steps
{
    public class ProjectSteps
    {
        private string _newProjectName = null!;
        internal long NewProjectId;

        private IDbConnection _db { get;  }

        public ProjectSteps(IDbConnection db)
        {
            _db = db;
        }

        internal void CreateNewProject()
        {
            _newProjectName = $"autotest_project_name_{Guid.NewGuid():N}";
            NewProjectId = ProjectRepository.InsertProjectAndReturnProjectId(_db, _newProjectName);
        }

        internal void DeleteNewProject(long newProjectId)
        {
            ProjectRepository.DeleteProjectById(_db, newProjectId);
        }
    }
}
