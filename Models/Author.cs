using System;
using System.Collections.Generic;
using System.Text;

namespace MySqlApp.Models
{
    // --- THE MODEL ---
    // This class matches the 'dbo.author' table
    internal class Author
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Login { get; set; }
        public required string Email { get; set; }
    }
}
