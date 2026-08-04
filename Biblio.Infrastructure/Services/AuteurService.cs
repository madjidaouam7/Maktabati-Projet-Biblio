using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Interfaces;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Services
{
    public class AuteurService : IAuteurService
    {
        private readonly BiblioDbContext _context;
        public AuteurService(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Auteur> Auteurs, int Total)> GetAuteursAsync(
            string? recherche, string? trierPar, string? ordre, int page, int taillePage)
        {
            var query = _context.Auteurs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(recherche))
            {
                query = query.Where(c => c.Nom.Contains(recherche));
            }

            bool decroissant = ordre?.Trim().ToLower() == "desc";

            query = trierPar?.Trim().ToLower() switch
            {
                "nom" => decroissant ? query.OrderByDescending(a => a.Nom) : query.OrderBy(a => a.Nom),
                "prenom" => decroissant ? query.OrderByDescending(a => a.Prenom) : query.OrderBy(a => a.Prenom),
                "datenaissance" => decroissant ? query.OrderByDescending(a => a.DateNaissance) : query.OrderBy(a => a.DateNaissance),
                _ => query.OrderBy(a => a.Id),
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


        public async Task<Auteur?> GetAuteurAsync(int id)
        {
            return await _context.Auteurs.FindAsync(id);
        }


        public async Task<Auteur> CreateAuteurAsync(Auteur auteur)
        {
            _context.Auteurs.Add(auteur);
            await _context.SaveChangesAsync();
            return auteur;
        }


        public async Task<bool> UpdateAuteurAsync(int id, Auteur auteurModifie)
        {
            var auteur = await _context.Auteurs.FindAsync(id);
            if (auteur == null)
            {
                return false;
            }

            auteur.Nom = auteurModifie.Nom;
            auteur.Prenom = auteurModifie.Prenom;
            auteur.DateNaissance = auteurModifie.DateNaissance;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAuteurAsync(int id)
        {
            var auteur = await _context.Auteurs.FindAsync(id);
            if (auteur == null)
            {
                return false;
            }

            _context.Auteurs.Remove(auteur);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}