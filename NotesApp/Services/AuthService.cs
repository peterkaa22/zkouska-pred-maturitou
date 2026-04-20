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
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    // Heslo ukládáme jako PBKDF2$iterations$saltBase64$hashBase64
    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    // Ověří heslo proti novému formátu i proti legacy SHA256 hashům.
    public bool VerifyPassword(string password, string hash)
    {
        if (hash.StartsWith("PBKDF2$", StringComparison.Ordinal))
        {
            var parts = hash.Split('$');
            if (parts.Length != 4)
            {
                return false;
            }

            if (!int.TryParse(parts[1], out var iterations))
            {
                return false;
            }

            byte[] salt;
            byte[] expectedHash;

            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expectedHash = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }

        // Legacy fallback (stávající účty vytvořené v předchozí verzi projektu)
        using var sha256 = SHA256.Create();
        var legacyHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

        byte[] legacyStoredHash;
        try
        {
            legacyStoredHash = Convert.FromBase64String(hash);
        }
        catch (FormatException)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(legacyHash, legacyStoredHash);
    }
}
