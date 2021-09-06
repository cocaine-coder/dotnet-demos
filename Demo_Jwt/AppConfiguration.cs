
using System.Text;

namespace Demo_Jwt;

public class SecurityJwtConfig
{
    public const string Forever = "FOREVER";

    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? Key { get; set; }
    public int ExpireMinutes { get; set; }

    public byte[] KeyBytes => 
        string.IsNullOrEmpty(Key) ? 
        throw new ArgumentNullException(Key) : 
        Encoding.UTF8.GetBytes(Key);
};
