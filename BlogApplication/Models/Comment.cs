using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncoSyntax.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        [MaxLength(100, ErrorMessage = "UserName cannot exceed 100 characters")]
        public string UserName { get; set; } = string.Empty;

        // ✅ FIXED
        public DateTime CommentDate { get; set; } = DateTime.Now;

        [Required]
        public string Content { get; set; } = string.Empty;

        [ForeignKey("Post")]
        public int PostId { get; set; }

        public Post Post { get; set; } = null!;
    }
}
