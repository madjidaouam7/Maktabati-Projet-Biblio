using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Biblio.Web.Services
{
    public class AuteurDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public DateTime? DateNaissance { get; set; }
    }

    public class CreateAuteurDto
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public DateTime? DateNaissance { get; set; }
    }

    public class AuteursPagines
    {
        public int Page { get; set; }
        public int TaillePage { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public List<AuteurDto> Auteurs { get; set; } = new List<AuteurDto>();
    }

    public class AuteurService
    {
        private readonly HttpClient _httpClient;
        private readonly EtatAuthService _etatAuth;

        public AuteurService(IHttpClientFactory httpClientFactory, EtatAuthService etatAuth)
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


        public async Task<List<AuteurDto>> GetAuteursAsync()
        {
            AjouterToken();
            var reponse = await _httpClient.GetFromJsonAsync<AuteursPagines>("api/auteurs");
            return reponse?.Auteurs ?? new List<AuteurDto>();
        }

        public async Task<AuteurDto?> GetAuteurAsync(int id)
        {
            AjouterToken();
            var reponse = await _httpClient.GetAsync($"api/auteurs/{id}");
            if (!reponse.IsSuccessStatusCode)
            {
                return null;
            }
            return await reponse.Content.ReadFromJsonAsync<AuteurDto>();
        }

        public async Task<(bool Succes, string? Erreur)> CreateAuteurAsync(CreateAuteurDto dto)
        {
            AjouterToken();
            var reponse = await _httpClient.PostAsJsonAsync("api/auteurs", dto);
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }

        public async Task<(bool Succes, string? Erreur)> UpdateAuteurAsync(int id, CreateAuteurDto dto)
        {
            AjouterToken();
            var reponse = await _httpClient.PutAsJsonAsync($"api/auteurs/{id}", dto);
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }

        public async Task<(bool Succes, string? Erreur)> DeleteAuteurAsync(int id)
        {
            AjouterToken();
            var reponse = await _httpClient.DeleteAsync($"api/auteurs/{id}");
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }
    }
}