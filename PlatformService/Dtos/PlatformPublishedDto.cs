namespace PlatformService.Dtos
{
    public class PlatformPublishedDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Event { get; set; } = String.Empty;
         public string Publisher { get; set; } = String.Empty;
        public string Cost { get; set; } = String.Empty;
    }
}