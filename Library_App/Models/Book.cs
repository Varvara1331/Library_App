namespace Library_App.Models;

public partial class Book
{
    public int IdBook { get; set; }

    public int YearOfPublication { get; set; }

    public string Publishing { get; set; } = null!;

    public string? Genre { get; set; }

    public int AgeRestrictions { get; set; }

    public bool? PermissionToIssuance { get; set; }

    public string TitleBook { get; set; } = null!;

    public int CopiesNumber { get; set; }

    public string? AuthorBook { get; set; }

    public virtual ICollection<BookDistributionBook> BookDistributionBooks { get; set; } = new List<BookDistributionBook>();

    public virtual ICollection<BookingBook> BookingBooks { get; set; } = new List<BookingBook>();
}
