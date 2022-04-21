namespace PlatformService.Dtos
{
    public class PlatformUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Cost { get; set; }

        public PlatformUpdateDto()
        {
            Name = String.Empty;
            Publisher = String.Empty;
            Cost = String.Empty;
        }
    }
}