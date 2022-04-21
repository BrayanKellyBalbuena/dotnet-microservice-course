namespace CommandService.Dtos
{
    public class CommandReadDto
    {
        public int Id { get; set; }
        public string  HowTo { get; set; } = string.Empty;
        public string CommandLine { get; set; } = string.Empty;
        public int PlatformID { get; set; }
    }
}