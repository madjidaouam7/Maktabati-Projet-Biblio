using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Biblio.Web.New.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly TokenProvider _tokenProvider;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(TokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await _tokenProvider.LoadTokenAsync();
        var token = _tokenProvider.Token;

        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(_anonymous);

        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            if (jwt.ValidTo <= DateTime.UtcNow)
            {
                await _tokenProvider.SetTokenAsync(null);
                return new AuthenticationState(_anonymous);
            }

            var identity = new ClaimsIdentity(jwt.Claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (ArgumentException)
        {
            await _tokenProvider.SetTokenAsync(null);
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task InitializeAsync()
    {
        await _tokenProvider.LoadTokenAsync();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsAuthenticatedAsync(string token)
    {
        await _tokenProvider.SetTokenAsync(token);

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _tokenProvider.SetTokenAsync(null);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}