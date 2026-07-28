using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Biblio.Web.Services
{
    public class CategorieDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CreateCategorieDto
    {
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CategoriesPagines
    {
        public int Page { get; set; }
        public int TaillePage { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public List<CategorieDto> Categories { get; set; } = new List<CategorieDto>();
    }

    public class CategorieService
    {
        private readonly HttpClient _httpClient;
        private readonly EtatAuthService _etatAuth;

        public CategorieService(IHttpClientFactory httpClientFactory, EtatAuthService etatAuth)
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

        public async Task<List<CategorieDto>> GetCategoriesAsync()
        {
            AjouterToken();
            var reponse = await _httpClient.GetFromJsonAsync<CategoriesPagines>("api/categories");
            return reponse?.Categories ?? new List<CategorieDto>();
        }

        public async Task<CategorieDto?> GetCategorieAsync(int id)
        {
            AjouterToken();
            var reponse = await _httpClient.GetAsync($"api/categories/{id}");
            if (!reponse.IsSuccessStatusCode)
            {
                return null;
            }
            return await reponse.Content.ReadFromJsonAsync<CategorieDto>();
        }

        public async Task<(bool Succes, string? Erreur)> CreateCategorieAsync(CreateCategorieDto dto)
        {
            AjouterToken();
            var reponse = await _httpClient.PostAsJsonAsync("api/categories", dto);
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }

        public async Task<(bool Succes, string? Erreur)> UpdateCategorieAsync(int id, CreateCategorieDto dto)
        {
            AjouterToken();
            var reponse = await _httpClient.PutAsJsonAsync($"api/categories/{id}", dto);
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }

        public async Task<(bool Succes, string? Erreur)> DeleteCategorieAsync(int id)
        {
            AjouterToken();
            var reponse = await _httpClient.DeleteAsync($"api/categories/{id}");
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }
    }
}