using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NotesApp.Data;
using NotesApp.Services;

namespace NotesApp.Pages.Account;

public class DeleteAccountModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;

    public DeleteAccountModel(ApplicationDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
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

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Pro zrušení účtu musíte zadat heslo.";
            return Page();
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
        if (user == null)
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        if (!_authService.VerifyPassword(Password, user.PasswordHash))
        {
            ErrorMessage = "Zadané heslo není správné.";
            return Page();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        HttpContext.Session.Clear();
        return RedirectToPage("/Login");
    }
}
