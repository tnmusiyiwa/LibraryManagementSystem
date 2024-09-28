using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.API.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        public DateTime ReservationDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }
        public bool IsCanceled { get; set; } = false;
    }
}