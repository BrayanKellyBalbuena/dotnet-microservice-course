namespace PlatformService.Dtos 
{
    public class PlatformCreateDto
    {
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Cost { get; set; }

        public PlatformCreateDto()
        {
            Name = String.Empty;
            Publisher = String.Empty;
            Cost = String.Empty;
        }
    }
}