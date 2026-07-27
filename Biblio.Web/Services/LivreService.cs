using System.Net.Http.Json;

namespace Biblio.Web.Services
{
    public class LivreDto
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public DateTime DatePublication { get; set; }
        public int NombrePages { get; set; }
        public int NombreExemplaires { get; set; }
        public int AuteurId { get; set; }
        public string AuteurNomComplet { get; set; } = string.Empty;
        public int CategorieId { get; set; }
        public string CategorieNom { get; set; } = string.Empty;
    }

    public class CreateLivreDto
    {
        public string Titre { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public DateTime DatePublication { get; set; }
        public int NombrePages { get; set; }
        public int NombreExemplaires { get; set; }
        public int AuteurId { get; set; }
        public int CategorieId { get; set; }
    }

    public class LivresPagines
    {
        public int Page { get; set; }
        public int TaillePage { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public List<LivreDto> Livres { get; set; } = new List<LivreDto>();
    }

    public class LivreService
    {
        private readonly HttpClient _httpClient;

        public LivreService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BiblioApi");
        }

        public async Task<List<LivreDto>> GetLivresAsync()
        {
            var reponse = await _httpClient.GetFromJsonAsync<LivresPagines>("api/livres");
            return reponse?.Livres ?? new List<LivreDto>();
        }

        public async Task<LivreDto?> GetLivreAsync(int id)
        {
            var reponse = await _httpClient.GetAsync($"api/livres/{id}");
            if (!reponse.IsSuccessStatusCode)
            {
                return null;
            }
            return await reponse.Content.ReadFromJsonAsync<LivreDto>();
        }

        public async Task<(bool Succes, string? Erreur)> CreateLivreAsync(CreateLivreDto dto)
        {
            var reponse = await _httpClient.PostAsJsonAsync("api/livres", dto);
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }

        public async Task<(bool Succes, string? Erreur)> UpdateLivreAsync(int id, CreateLivreDto dto)
        {
            var reponse = await _httpClient.PutAsJsonAsync($"api/livres/{id}", dto);
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }

        public async Task<(bool Succes, string? Erreur)> DeleteLivreAsync(int id)
        {
            var reponse = await _httpClient.DeleteAsync($"api/livres/{id}");
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }
    }
}