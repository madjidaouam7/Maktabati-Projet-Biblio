using Biblio.Domain.Entities;

namespace Biblio.Application.Interfaces
{
    public interface IAuteurService
    {
        Task<(List<Auteur> Auteurs, int Total)> GetAuteursAsync(
           string? recherche, string? trierPar, string? ordre, int page, int taillePage);
        Task<Auteur?> GetAuteurAsync(int id);
        Task<Auteur> CreateAuteurAsync(Auteur auteur);
        Task<bool> UpdateAuteurAsync(int id, Auteur auteur);
        Task<bool> DeleteAuteurAsync(int id);
    }
}