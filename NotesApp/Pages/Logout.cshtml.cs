using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NotesApp.Pages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        // Vymazat session
        HttpContext.Session.Clear();

        // Přesměrovat na login
        return RedirectToPage("/Login");
    }
}
