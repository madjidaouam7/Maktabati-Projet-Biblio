using Biblio.Domain.Entities;

namespace Biblio.Application.Interfaces
{
    public interface ICategorieService
    {
        Task<(List<Categorie> Categories, int Total)> GetCategoriesAsync(
            string? recherche, string? trierPar, string? ordre, int page, int taillePage);
        Task<Categorie?> GetCategorieAsync (int id);
        Task<Categorie> CreateCategorieAsync (Categorie categorie);
        Task<bool> UpdateCategorieAsync (int id, Categorie categorie);
        Task<bool> DeleteCategorieAsync (int id);
    }
}