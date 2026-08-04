using Biblio.Domain.Entities;

namespace Biblio.Application.Interfaces
{
    public class ResultatLivre
    {
        public bool Succes { get; set; }
        public string? Erreur { get; set; }
        public Livre? Livre { get; set; }
    }


    public interface ILivreService
    {
        Task<(List<Livre> Livres, int Total)> GetLivresAsync(
            string? recherche, string? trierPar, string? ordre, int page, int taillePage);

        Task<Livre?> GetLivreAsync(int id);

        Task<ResultatLivre> CreateLivreAsync(Livre livre);

        Task<ResultatLivre> UpdateLivreAsync(int id, Livre livreModifie);

        Task<(bool Succes, string? Erreur)> DeleteLivreAsync(int id);

        Task<List<LivreDisponible>> GetLivresDisponiblesAsync();
    }

    public class LivreDisponible
    {
        public Livre Livre { get; set; } = null!;
        public int NombreExemplairesDisponibles { get; set; }
    }
}