using System.ComponentModel.DataAnnotations;

namespace CommandService.Models 
{
    public class Command
    {
        public int Id { get; set; }
        [Required]
        public string  HowTo { get; set; } = string.Empty;
        [Required]
        public string CommandLine { get; set; } = string.Empty;
        [Required]
        public int PlatformID { get; set; }
        public virtual Platform? Platform { get; set; }
    }
}