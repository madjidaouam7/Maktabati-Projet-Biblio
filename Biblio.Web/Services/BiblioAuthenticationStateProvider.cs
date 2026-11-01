using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Biblio.Web.Services
{
    public class BiblioAuthenticationStateProvider
        : AuthenticationStateProvider
    {
        private ClaimsPrincipal _utilisateur = new(
            new ClaimsIdentity());

        public string? Token { get; private set; }

        public bool EstConnecte =>
            _utilisateur.Identity?.IsAuthenticated == true;

        public override Task<AuthenticationState>
            GetAuthenticationStateAsync()
        {
            return Task.FromResult(
                new AuthenticationState(_utilisateur));
        }

        public void Connecter(
            string token,
            string email,
            List<string> roles)
        {
            Token = token;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identite = new ClaimsIdentity(
                claims,
                authenticationType: "BiblioAuth",
                nameType: ClaimTypes.Name,
                roleType: ClaimTypes.Role);

            _utilisateur = new ClaimsPrincipal(identite);

            NotifyAuthenticationStateChanged(
                GetAuthenticationStateAsync());
        }

        public void Deconnecter()
        {
            Token = null;
            _utilisateur = new ClaimsPrincipal(
                new ClaimsIdentity());

            NotifyAuthenticationStateChanged(
                GetAuthenticationStateAsync());
        }
    }
}