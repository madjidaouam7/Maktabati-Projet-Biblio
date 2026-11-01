using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;

namespace Biblio.Web.New.Authentication;

public class TokenProvider
{
    private const string TokenStorageKey = "biblio-token";
    public const string TokenCookieKey = "biblio-token";
    private readonly IJSRuntime _jsRuntime;
    private bool _hasLoaded;

    public TokenProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string? Token { get; private set; }

    public async Task LoadTokenAsync()
    {
        if (_hasLoaded)
            return;

        try
        {
            Token = await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem", TokenStorageKey);

            if (!string.IsNullOrWhiteSpace(Token))
            {
                await SetTokenCookieAsync(Token);
            }

            _hasLoaded = true;
        }
        catch (InvalidOperationException)
        {
            // JavaScript n'est pas disponible pendant le prerendering.
        }
    }

    public async Task SetTokenAsync(string? token)
    {
        Token = token;
        _hasLoaded = true;

        if (string.IsNullOrWhiteSpace(token))
        {
            await _jsRuntime.InvokeVoidAsync(
                "localStorage.removeItem", TokenStorageKey);
            await _jsRuntime.InvokeVoidAsync("biblioAuthCookie.clearToken");
            return;
        }

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem", TokenStorageKey, token);
        await SetTokenCookieAsync(token);
    }

    private Task SetTokenCookieAsync(string token)
    {
        var expiresAt = new JwtSecurityTokenHandler().ReadJwtToken(token).ValidTo;
        return _jsRuntime.InvokeVoidAsync(
            "biblioAuthCookie.setToken", TokenCookieKey, token, expiresAt).AsTask();
    }
}