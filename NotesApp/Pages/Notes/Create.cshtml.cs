using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NotesApp.Data;
using NotesApp.Models;

namespace NotesApp.Pages.Notes;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Title { get; set; } = string.Empty;

    [BindProperty]
    public string NoteContent { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        // Ověřit, zda je uživatel přihlášen
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToPage("/Login");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToPage("/Login");
        }

        // Validace
        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(NoteContent))
        {
            ErrorMessage = "Nadpis a obsah jsou povinné!";
            return Page();
        }

        if (Title.Length > 500)
        {
            ErrorMessage = "Nadpis je příliš dlouhý (max 500 znaků)!";
            return Page();
        }

        // Vytvořit novou poznámku
        var note = new Note
        {
            Title = Title.Trim(),
            Content = NoteContent.Trim(),
            UserId = userId.Value,
            CreatedAt = DateTime.Now,
            IsImportant = false
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Notes/List");
    }
}
