using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NotesApp.Data;
using NotesApp.Models;
using NotesApp.Services;

namespace NotesApp.Pages;

public class RegisterModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;

    public RegisterModel(ApplicationDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string ConfirmPassword { get; set; } = string.Empty;

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

    public async Task<IActionResult> OnPostAsync()
    {
        // Validace
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Uživatelské jméno a heslo jsou povinné!";
            return Page();
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Hesla se neshodují!";
            return Page();
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Heslo musí mít alespoň 6 znaků!";
            return Page();
        }

        // Ověřit, že uživatelské jméno je unikátní
        var existingUser = _context.Users.FirstOrDefault(u => u.Username == Username);
        if (existingUser != null)
        {
            ErrorMessage = "Toto uživatelské jméno už existuje!";
            return Page();
        }

        // Vytvořit nového uživatele
        var user = new User
        {
            Username = Username,
            PasswordHash = _authService.HashPassword(Password),
            CreatedAt = DateTime.Now
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Přihlásit uživatele a nastavit session
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Username", user.Username);

        return RedirectToPage("/Notes/List");
    }
}
