using System.Security.Cryptography;
using System.Text;

namespace Application.Security;

public class Password
{
    private readonly string _password;

    public Password(string password)
    {
        ValidatePassword(password);

        _password = password;
    }

    public string PasswordHash()
    {
        return ComputeSha256Hash(_password);
    }

    private void ValidatePassword(string password)
    {
        if (password.Length < 2)
        {
            throw new InvalidDataException("Password is to short");
        }
    }

    private string ComputeSha256Hash(string rawData)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
        StringBuilder builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString();
    }
}