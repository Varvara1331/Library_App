namespace Library_App.Models;

public partial class User
{
    public string LoginUser { get; set; } = null!;

    public string? PasswordUser { get; set; }

    public string NameUser { get; set; } = null!;

    public int IdRole { get; set; }

    public virtual Role IdRoleNavigation { get; set; } = null!;
}
