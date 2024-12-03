using MainPortfolio.Security.Services.Interfaces;

namespace MainPortfolio.Middleware;

public class RefreshTokenMiddleware
{
    private readonly RequestDelegate _next;

    public RefreshTokenMiddleware(RequestDelegate next) { _next = next; }

    public async Task InvokeAsync(HttpContext context, IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService, IConfiguration configuration)
    {
        var accessToken = context.Request.Headers["Authorization"].ToString().Replace(configuration["Keys:Bearer"]!, "");

        if (string.IsNullOrWhiteSpace(accessToken) || !accessTokenService.ValidateAccessToken(accessToken))
        {
            var refreshToken = context.Request.Cookies[configuration["Keys:RefreshToken"]!];

            if (!string.IsNullOrEmpty(refreshToken) && refreshTokenService.ValidateRefreshToken(refreshToken))
            {
                var newAccessToken = await accessTokenService.GenerateNewAccessTokenAsync(refreshToken);

                if (newAccessToken != null)
                {
                    context.Request.Headers["Authorization"] = $"{configuration["Keys:Bearer"]!}{newAccessToken}";
                    context.Response.Headers.Append(configuration["Keys:AccessToken"]!, newAccessToken!);
                }
            }
        }

        await _next(context);
    }

}
