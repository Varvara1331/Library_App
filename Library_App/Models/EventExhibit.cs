namespace Library_App.Models;

public partial class EventExhibit
{
    public int IdEventExhibit { get; set; }

    public int IdEvent { get; set; }

    public int IdExhibit { get; set; }

    public virtual Event IdEventNavigation { get; set; } = null!;

    public virtual Exhibit IdExhibitNavigation { get; set; } = null!;
}
