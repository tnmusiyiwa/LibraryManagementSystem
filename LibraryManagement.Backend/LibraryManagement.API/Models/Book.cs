using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.API.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author { get; set; }

        [Required]
        public int PublicationYear { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(20)]
        public string? ISBN { get; set; }

        public bool IsAvailable { get; set; } = true;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string? CoverImage { get; set; }
    }
}
