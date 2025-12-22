using System.ComponentModel.DataAnnotations;
namespace SyncoSyntax.Models
{
    public class Category
    {
        [Key]
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public int Id { get; set; }

        public string  Name { get; set; }

        public string? Description { get; set; }


        public ICollection<Post> Posts { get; set; }


    }
}
