namespace MainPortfolio.Models;

public class TokenRequest { public string Token { get; set; } }

public class TokenBlacklist
{
    public int Id { get; set; }
    public string Token { get; set; } // Or TokenId (jti) if you store the jti
    public DateTime Expiration { get; set; }
}
