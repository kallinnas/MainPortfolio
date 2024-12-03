namespace MainPortfolio.Models;

public class TokenRequest { public string? Token { get; set; } }

public enum TokenStatus { Invalid, NotFound, Valid }