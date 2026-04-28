namespace MySqlApp.Models
{
    internal class Test
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public int? StatusId { get; set; }
        public required string MethodName { get; set; }
        public long ProjectId { set; get; }
        public long SessionId { set; get; }
        public DateTime? StartTime { set; get; }
        public DateTime? EndTime { set; get; }
        public required string Env { get; set; }
        public string? Browser { get; set; }
        public long? AuthorId { get; set; }
    }
}
