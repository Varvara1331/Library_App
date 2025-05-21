namespace Library_App.Models;

public partial class Booking
{
    public int IdBooking { get; set; }

    public int ReaderTicket { get; set; }

    public DateOnly BookingDate { get; set; }

    public virtual ICollection<BookingBook> BookingBooks { get; set; } = new List<BookingBook>();

    public virtual Reader ReaderTicketNavigation { get; set; } = null!;
}
