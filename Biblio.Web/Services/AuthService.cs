using System.Net.Http.Json;

namespace Biblio.Web.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly EtatAuthService _etatAuth;

        public AuthService(IHttpClientFactory httpClientFactory, EtatAuthService etatAuth)
        {
            _httpClient = httpClientFactory.CreateClient("BiblioApi");
            _etatAuth = etatAuth;
        }

        public async Task<(bool Succes, string? Erreur)> LoginAsync(string email, string motDePasse)
        {
            var dto = new LoginDto { Email = email, MotDePasse = motDePasse };
            var reponse = await _httpClient.PostAsJsonAsync("api/auth/login", dto);

            if (!reponse.IsSuccessStatusCode)
            {
                var erreur = await reponse.Content.ReadAsStringAsync();
                return (false, erreur);
            }

            var resultat = await reponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (resultat == null)
            {
                return (false, "Réponse invalide du serveur.");
            }

            _etatAuth.Connecter(resultat.Token, resultat.Email, resultat.Roles);
            return (true, null);
        }
    }
}