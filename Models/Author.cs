
namespace MySqlApp.Models
{
    internal class Author
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Login { get; set; }
        public required string Email { get; set; }
    }
}
