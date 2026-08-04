using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Interfaces;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Services
{
    public class LivreService : ILivreService
    {
        private readonly BiblioDbContext _context;
        public LivreService(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Livre> Livres, int Total)> GetLivresAsync(
            string? recherche, string? trierPar, string? ordre, int page, int taillePage)
        {
            var query = _context.Livres
                .Include(l => l.Auteur)
                .Include(l => l.Categorie)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                query = query.Where(l => l.Titre.Contains(recherche));
            }

            bool decroissant = ordre?.Trim().ToLower() == "desc";

            query = trierPar?.Trim().ToLower() switch
            {
                "titre" => decroissant ? query.OrderByDescending(l => l.Titre) : query.OrderBy(l => l.Titre),
                "datepublication" => decroissant ? query.OrderByDescending(l => l.DatePublication) : query.OrderBy(l => l.DatePublication),
                "nombrepages" => decroissant ? query.OrderByDescending(l => l.NombrePages) : query.OrderBy(l => l.NombrePages),
                _ => query.OrderBy(l => l.Id),
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




        public async Task<Livre?> GetLivreAsync(int id)
        {
            return await _context.Livres
            .Include(l => l.Auteur)      
            .Include(l => l.Categorie)   
            .FirstOrDefaultAsync(l => l.Id == id);

        }




        public async Task<ResultatLivre> CreateLivreAsync(Livre livre)
        {
            var auteur = await _context.Auteurs.FindAsync(livre.AuteurId);
            if (auteur == null)
            {
                return new ResultatLivre { Succes = false, Erreur = $"Aucun auteur trouvé avec l'id {livre.AuteurId}." };
            }

            var categorie = await _context.Categories.FindAsync(livre.CategorieId);
            if (categorie == null)
            {
                return new ResultatLivre { Succes = false, Erreur = $"Aucune catégorie trouvée avec l'id {livre.CategorieId}." };
            }

            var isbnExiste = await _context.Livres.AnyAsync(l => l.ISBN == livre.ISBN);
            if (isbnExiste)
            {
                return new ResultatLivre { Succes = false, Erreur = $"Un livre avec l'ISBN '{livre.ISBN}' existe déjà." };
            }

            if (livre.DatePublication > DateTime.Now)
            {
                return new ResultatLivre { Succes = false, Erreur = "La date de publication ne peut pas être dans le futur." };
            }


            _context.Livres.Add(livre);
            await _context.SaveChangesAsync();
            return new ResultatLivre { Succes = true, Livre = livre };
        }




        public async Task<ResultatLivre> UpdateLivreAsync(int id, Livre livreModifie)
        {
            var livre = await _context.Livres.FindAsync(id);
            if (livre == null)
            {
                return new ResultatLivre { Succes = false, Erreur = "Livre non trouvé" };
            }

            var auteur = await _context.Auteurs.FindAsync(livreModifie.AuteurId);
            if (auteur == null)
            {
                return new ResultatLivre { Succes = false, Erreur = $"Aucun auteur trouvé avec l'id {livreModifie.AuteurId}." };
            }

            var categorie = await _context.Categories.FindAsync(livreModifie.CategorieId);
            if (categorie == null)
            {
                return new ResultatLivre { Succes = false, Erreur = $"Aucune catégorie trouvée avec l'id {livreModifie.CategorieId}." };
            }

            var isbnExiste = await _context.Livres.AnyAsync(l => l.ISBN == livreModifie.ISBN);
            if (isbnExiste)
            {
                return new ResultatLivre { Succes = false, Erreur = $"Un livre avec l'ISBN '{livreModifie.ISBN}' existe déjà." };
            }

            if (livre.DatePublication > DateTime.Now)
            {
                return new ResultatLivre { Succes = false, Erreur = "La date de publication ne peut pas être dans le futur." };
            }

            // Mettre à jour les propriétés de la livre
            livre.Titre = livreModifie.Titre;
            livre.ISBN = livreModifie.ISBN;
            livre.DatePublication = livreModifie.DatePublication;
            livre.NombrePages = livreModifie.NombrePages;
            livre.NombreExemplaires = livreModifie.NombreExemplaires;
            livre.AuteurId = livreModifie.AuteurId;
            livre.CategorieId = livreModifie.CategorieId;

            await _context.SaveChangesAsync();
            return new ResultatLivre { Succes = true, Livre = livre };
        }






        public async Task<(bool Succes, string? Erreur)> DeleteLivreAsync(int id)
        {
            var livre = await _context.Livres.FindAsync(id);
            if (livre == null)
            {
                return (false, $"Aucun livre trouvé avec l'id {id}.");
            }

            var aDesEmprunts = await _context.Emprunts.AnyAsync(e => e.LivreId == id);
            if (aDesEmprunts)
            {
                return (false, $"Impossible de supprimer '{livre.Titre}' : il possède un historique d'emprunts.");
            }

            _context.Livres.Remove(livre);
            await _context.SaveChangesAsync();
            return (true, null);
        }




        public async Task<List<LivreDisponible>> GetLivresDisponiblesAsync()
        {
            var livres = await _context.Livres
                .Include(l => l.Auteur)
                .Include(l => l.Emprunts)
                .ToListAsync();

            var resultat = livres
        .Select(l => new LivreDisponible
        {
            Livre = l,
            NombreExemplairesDisponibles = l.NombreExemplaires - l.Emprunts.Count(e => e.Etat == EtatEmprunt.EnCours)
        })
        .Where(x => x.NombreExemplairesDisponibles > 0)
        .ToList();

            return resultat;

        }

    }
}
