namespace Library_App.Models;

public partial class BookingBook
{
    public int IdBookingBook { get; set; }

    public int IdBooking { get; set; }

    public int IdBook { get; set; }

    public virtual Book IdBookNavigation { get; set; } = null!;

    public virtual Booking IdBookingNavigation { get; set; } = null!;
}
