using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public required string Title { get; set; } 

        [Required]
        public required string Author { get; set; }

        public int? PublishedYear { get; set; } 

        [Required]
        public required decimal Price { get; set; }

        public string? Genre { get; set; } 
    }
}