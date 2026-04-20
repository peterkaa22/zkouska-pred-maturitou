namespace NotesApp.Models;

public class User
{
    public int Id { get; set; }
    
    public required string Username { get; set; }
    
    public required string PasswordHash { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // Relace - jeden uživatel má více poznámek
    public ICollection<Note> Notes { get; set; } = new List<Note>();
}
