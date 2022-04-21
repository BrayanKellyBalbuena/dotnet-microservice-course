namespace CommandService.Dtos
{
    public class PlatformReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ExternalID { get; set; }
        public string Publisher { get; set; } = String.Empty;
        public string Cost { get; set; } = String.Empty;
    }
}