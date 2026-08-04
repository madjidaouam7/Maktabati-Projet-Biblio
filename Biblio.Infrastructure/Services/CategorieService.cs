using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Interfaces;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Services
{
    public class CategorieService : ICategorieService
    {
        private readonly BiblioDbContext _context;
        public CategorieService(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Categorie> Categories, int Total)> GetCategoriesAsync(
            string? recherche, string? trierPar, string? ordre, int page, int taillePage)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                query = query.Where(c => c.Nom.Contains(recherche));
            }

            bool decroissant = ordre?.Trim().ToLower() == "desc";

            query = trierPar?.Trim().ToLower() switch
            {
                "nom" => decroissant ? query.OrderByDescending(c => c.Nom) : query.OrderBy(c => c.Nom),
                "description" => decroissant ? query.OrderByDescending(c => c.Description) : query.OrderBy(c => c.Description),
                _ => query.OrderBy(c => c.Id),
            };

            var total = await query.CountAsync();

            if (page < 1) page = 1;
            if (taillePage < 1) taillePage = 10;

            var categories = await query
            .Skip((page - 1) * taillePage)
            .Take(taillePage)
            .ToListAsync();

            return (categories, total);
        }

        public async Task<Categorie?> GetCategorieAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Categorie> CreateCategorieAsync(Categorie categorie)
        {
            _context.Categories.Add(categorie);
            await _context.SaveChangesAsync();
            return categorie;
        }

        public async Task<bool> UpdateCategorieAsync(int id, Categorie categorieModifiee)
        {
            var categorie = await _context.Categories.FindAsync(id);
            if (categorie == null)
            {
                return false;
            }

            categorie.Nom = categorieModifiee.Nom;
            categorie.Description = categorieModifiee.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategorieAsync(int id)
        {
            var categorie = await _context.Categories.FindAsync(id);
            if (categorie == null)
            {
                return false;
            }

            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}