namespace Library_App.Models;

public partial class Reader
{
    public int ReaderTicket { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Telephone { get; set; }

    public DateOnly BirthDate { get; set; }

    public string? Email { get; set; }

    public decimal Fine { get; set; }

    public virtual ICollection<BookDistribution> BookDistributions { get; set; } = new List<BookDistribution>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
