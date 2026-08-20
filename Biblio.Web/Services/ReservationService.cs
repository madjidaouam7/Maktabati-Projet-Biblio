using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Biblio.Web.Services
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public int LivreId { get; set; }
        public string LivreTitre { get; set; } = string.Empty;
        public string Utilisateur { get; set; } = string.Empty;
        public DateTime DateReservation { get; set; }
        public string Etat { get; set; } = string.Empty;
    }

    public class CreateReservationDto
    {
        public int LivreId { get; set; }
    }

    public class ReservationService
    {
        private readonly HttpClient _httpClient;
        private readonly EtatAuthService _etatAuth;

        public ReservationService(IHttpClientFactory httpClientFactory, EtatAuthService etatAuth)
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

        public async Task<List<ReservationDto>> GetReservationsAsync()
        {
            AjouterToken();
            var reponse = await _httpClient.GetFromJsonAsync<List<ReservationDto>>("api/reservations");
            return reponse ?? new List<ReservationDto>();
        }

        public async Task<(bool Succes, string? Erreur)> ReserverLivreAsync(CreateReservationDto dto)
        {
            AjouterToken();
            var reponse = await _httpClient.PostAsJsonAsync("api/reservations/reserver", dto);
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }

        public async Task<(bool Succes, string? Erreur)> AnnulerReservationAsync(int id)
        {
            AjouterToken();
            var reponse = await _httpClient.PostAsync($"api/reservations/{id}/annuler", null);
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }

        public async Task<(bool Succes, string? Erreur)> HonorerReservationAsync(int id)
        {
            AjouterToken();
            var reponse = await _httpClient.PostAsync($"api/reservations/{id}/honorer", null);
            if (reponse.IsSuccessStatusCode)
            {
                return (true, null);
            }
            var erreur = await reponse.Content.ReadAsStringAsync();
            return (false, erreur);
        }
    }
}