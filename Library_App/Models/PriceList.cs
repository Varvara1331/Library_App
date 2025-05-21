namespace Library_App.Models;

public partial class PriceList
{
    public string NameService { get; set; } = null!;

    public decimal? Price { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
