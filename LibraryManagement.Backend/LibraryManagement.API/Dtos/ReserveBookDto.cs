namespace LibraryManagement.API.Dtos
{
    public class ReserveBookDto
    {
        public int BookId { get; set; }
        public bool NotifyWhenAvailable { get; set; } = false;
    }
}
