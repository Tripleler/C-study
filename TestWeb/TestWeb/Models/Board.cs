using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWeb.Models
{
    public class BoardBase
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

        [Required]
        public int? Deleted {  get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? EditDate { get; set; }
    }

    public class Board: BoardBase
    {
        public string? FileNames { get; set; }

        public string? FileIds { get; set; }

        public ICollection<Comment>? Comments { get; set; }

        public Board()
        {
            Comments = new List<Comment>();
        }
    }

    public class BoardSend: BoardBase
    {
        public List<IFormFile>? Files { get; set; }
    }

    public class Comment
    {
        [Key]
        public int idx { get; set; }

        public int BoardNo { get; set; }

        public string? Content { get; set; }

        public string? Writter { get; set; }

        public int Class { get; set; }

        public int GroupNum { get; set; }

        public DateTime? CreateDate { get; set; }

        [ForeignKey("BoardNo")]
        public Board? Board { get; set; }
    }
}
