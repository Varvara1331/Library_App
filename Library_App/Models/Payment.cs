namespace Library_App.Models;

public partial class Payment
{
    public int IdPayment { get; set; }

    public int ReaderTicket { get; set; }

    public DateOnly PaymentDate { get; set; }

    public decimal Cost { get; set; }

    public string? NameService { get; set; }

    public virtual PriceList? NameServiceNavigation { get; set; }

    public virtual Reader ReaderTicketNavigation { get; set; } = null!;
}
