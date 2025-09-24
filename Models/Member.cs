using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string FullName { get; set; } 

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public DateTime JoinDate { get; set; } = DateTime.Now; 
    }
}