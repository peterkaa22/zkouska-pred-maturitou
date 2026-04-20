namespace NotesApp.Models;

public class Note
{
    public int Id { get; set; }
    
    public required string Title { get; set; }
    
    public required string Content { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public bool IsImportant { get; set; } = false;
    
    // Cizí klíč - kterému uživateli patří poznámka
    public int UserId { get; set; }
    
    // Relace
    public User? User { get; set; }
}
