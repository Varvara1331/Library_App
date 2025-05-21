namespace Library_App.Models;

public partial class Event
{
    public int IdEvent { get; set; }

    public DateTime EventDate { get; set; }

    public string EventName { get; set; } = null!;

    public string EventLocation { get; set; } = null!;

    public string? EventType { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<EventExhibit> EventExhibits { get; set; } = new List<EventExhibit>();
}
