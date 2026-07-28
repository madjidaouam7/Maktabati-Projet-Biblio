using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Biblio.Web.Services
{
    public class EmpruntDto
    {
        public int Id { get; set; }
        public int LivreId { get; set; }
        public string LivreTitre { get; set; } = string.Empty;
        public string Utilisateur { get; set; } = string.Empty;
        public DateTime DateEmprunt { get; set; }
        public DateTime DateRetourPrevue { get; set; }
        public DateTime? DateRetourEffective { get; set; }
        public string Etat { get; set; } = string.Empty;
    }



    public class CreateEmpruntDto
    {
        public int LivreId { get; set; }
        public DateTime DateRetourPrevue { get; set; }
    }



    public class EmpruntService
    {
        private readonly HttpClient _httpClient;
        private readonly EtatAuthService _etatAuth;

        public EmpruntService(IHttpClientFactory httpClientFactory, EtatAuthService etatAuth)
        {
            _httpClient = httpClientFactory.CreateClient("BiblioApi");
            _etatAuth = etatAuth;
        }

        private void AjouterToken()
        {
            if (_etatAuth.EstConnecte)
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _etatAuth.Token);
            }
        }

        public async Task<(bool Succes, string? Erreur)> CreateEmpruntAsync(CreateEmpruntDto dto)
        {
            AjouterToken();
            var reponse = await _httpClient.PostAsJsonAsync("api/emprunts/emprunter", dto);
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }


        public async Task<(bool Succes, string? Erreur)> RetournerLivreAsync(int id)
        {
            AjouterToken();
            var reponse = await _httpClient.PostAsync($"api/emprunts/{id}/retourner", null);
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }


        public async Task<List<EmpruntDto>> GetEmpruntsActifsAsync()
        {
            AjouterToken();
            var reponse = await _httpClient.GetFromJsonAsync<List<EmpruntDto>>("api/emprunts/actifs");
            return reponse ?? new List<EmpruntDto>();
        }



        public async Task<List<EmpruntDto>> GetHistoriqueAsync()
        {
            AjouterToken();
            var reponse = await _httpClient.GetFromJsonAsync<List<EmpruntDto>>("api/emprunts/historique");
            return reponse ?? new List<EmpruntDto>();
        }
    }
}