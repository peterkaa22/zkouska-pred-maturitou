using System.Security.Cryptography;
using System.Text;

namespace NotesApp.Services;

public interface IAuthService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public class AuthService : IAuthService
{
    // Hašuje heslo pomocí PBKDF2
    public string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBuffer = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBuffer);
        }
    }

    // Ověří, zda heslo odpovídá hašu
    public bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}
