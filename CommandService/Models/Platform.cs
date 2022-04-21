using System.ComponentModel.DataAnnotations;

namespace CommandService.Models 
{
    public class Platform
    {
        public int Id { get; set; }
        [Required]
        public int  ExternalID { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Publisher { get; set; } = String.Empty;
        public string Cost { get; set; } = String.Empty;
        public virtual ICollection<Command> Commands { get; set; } = new List<Command>();
    }
}