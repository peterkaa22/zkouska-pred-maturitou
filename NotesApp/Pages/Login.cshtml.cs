using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NotesApp.Data;
using NotesApp.Services;

namespace NotesApp.Pages;

public class LoginModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;

    public LoginModel(ApplicationDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        // Pokud je uživatel již přihlášen, přesměrujeme ho
        if (HttpContext.Session.GetInt32("UserId").HasValue)
        {
            return RedirectToPage("/Notes/List");
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        // Validace
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Uživatelské jméno a heslo jsou povinné!";
            return Page();
        }

        // Najít uživatele
        var user = _context.Users.FirstOrDefault(u => u.Username == Username);
        if (user == null)
        {
            ErrorMessage = "Nesprávné uživatelské jméno nebo heslo!";
            return Page();
        }

        // Ověřit heslo
        if (!_authService.VerifyPassword(Password, user.PasswordHash))
        {
            ErrorMessage = "Nesprávné uživatelské jméno nebo heslo!";
            return Page();
        }

        // Nastavit session
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Username", user.Username);

        return RedirectToPage("/Notes/List");
    }
}
