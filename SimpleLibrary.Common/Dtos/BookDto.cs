namespace SimpleLibrary.Common.Dtos;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime AvailableFrom { get; set; }
}