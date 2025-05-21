namespace Library_App.Models;

public partial class BookDistribution
{
    public int IdBookDistribution { get; set; }

    public int ReaderTicket { get; set; }

    public DateOnly ReturnDate { get; set; }

    public DateOnly IssuanceDate { get; set; }

    public virtual ICollection<BookDistributionBook> BookDistributionBooks { get; set; } = new List<BookDistributionBook>();

    public virtual Reader ReaderTicketNavigation { get; set; } = null!;
}
