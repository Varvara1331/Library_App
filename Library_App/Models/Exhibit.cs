namespace Library_App.Models;

public partial class Exhibit
{
    public int IdExhibit { get; set; }

    public DateOnly? CreationDate { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string? Subject { get; set; }

    public virtual ICollection<EventExhibit> EventExhibits { get; set; } = new List<EventExhibit>();
}
