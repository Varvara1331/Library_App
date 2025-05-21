namespace Library_App.Models;

public partial class BookDistributionBook
{
    public int IdBookDistributionBook { get; set; }

    public int? IdBookDistribution { get; set; }

    public int IdBook { get; set; }

    public bool? Refund { get; set; }

    public virtual BookDistribution? IdBookDistributionNavigation { get; set; }

    public virtual Book IdBookNavigation { get; set; } = null!;
}
