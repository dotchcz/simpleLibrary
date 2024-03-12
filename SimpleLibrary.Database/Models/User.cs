namespace SimpleLibrary.Database.Models;

public class User: IId
{
    public string Username { get; set; }
    public string Email { get; set; }
}