using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.API.Models
{
    public class BorrowedBook
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }
        [Required]
        public BorrowedBookStatus Status { get; set; } = BorrowedBookStatus.Borrowed;
    }
}