using System.ComponentModel.DataAnnotations;

namespace TestWeb.Models
{
    public class Board_view
    {
        [Key]
        public int BoardNo { get; set; }

        [Required]
        public string? BoardTitle { get; set; }

        [Required]
        public string? BoardContent { get; set; }

        [Required]
        public string? BoardWritter { get; set; }

        [Required]
        public int? BoardViewCount { get; set; }
    }
}
