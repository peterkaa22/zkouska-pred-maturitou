using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NotesApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        // Pokud je uživatel přihlášen, přesměr na seznam poznámek
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId.HasValue)
        {
            return RedirectToPage("/Notes/List");
        }

        // Pokud není přihlášen, přesměr na login
        return RedirectToPage("/Login");
    }
}
