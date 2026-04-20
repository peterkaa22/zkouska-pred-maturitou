using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NotesApp.Data;
using NotesApp.Models;

namespace NotesApp.Pages.Notes;

public class ListModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ListModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Note> Notes { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public bool ShowOnlyImportant { get; set; } = false;

    public IActionResult OnGet()
    {
        // Ověřit, zda je uživatel přihlášen
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToPage("/Login");
        }

        // Načíst poznámky uživatele
        IQueryable<Note> query = _context.Notes
            .Where(n => n.UserId == userId.Value);

        // Filtr - pouze důležité
        if (ShowOnlyImportant)
        {
            query = query.Where(n => n.IsImportant);
        }

        Notes = query
            .OrderByDescending(n => n.CreatedAt) // Nejnovější první
            .ToList();
        return Page();
    }

    public IActionResult OnPostToggleImportant(int noteId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToPage("/Login");
        }

        var note = _context.Notes.FirstOrDefault(n => n.Id == noteId && n.UserId == userId.Value);
        if (note == null)
        {
            return NotFound();
        }

        // Přepnout stav
        note.IsImportant = !note.IsImportant;
        _context.SaveChanges();

        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int noteId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToPage("/Login");
        }

        var note = _context.Notes.FirstOrDefault(n => n.Id == noteId && n.UserId == userId.Value);
        if (note == null)
        {
            return NotFound();
        }

        _context.Notes.Remove(note);
        _context.SaveChanges();

        return RedirectToPage();
    }
}
