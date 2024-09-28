using System;

namespace LibraryManagement.API.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? BookId { get; set; }
        public Book? Book { get; set; }
        public string Message { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public ApplicationUser User { get; set; }
    }
}