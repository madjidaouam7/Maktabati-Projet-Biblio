using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Interfaces;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Services
{
    public class EmpruntService : IEmpruntService
    {
        private readonly BiblioDbContext _context;

        public EmpruntService(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<ResultatEmprunt> EmprunterLivreAsync(int livreId, string utilisateurId, DateTime dateRetourPrevue)
        {
            var maintenant = DateTime.Now;

            if (dateRetourPrevue < maintenant)
            {
                return new ResultatEmprunt { Succes = false, Erreur = "La date de retour prévue doit être supérieure ou égale à la date d'emprunt." };
            }

            var livre = await _context.Livres.FindAsync(livreId);
            if (livre == null)
            {
                return new ResultatEmprunt { Succes = false, Erreur = $"Aucun livre trouvé avec l'id {livreId}." };
            }

            var nombreEmpruntsEnCours = await _context.Emprunts
                .CountAsync(e => e.LivreId == livreId && e.Etat == EtatEmprunt.EnCours);

            if (nombreEmpruntsEnCours >= livre.NombreExemplaires)
            {
                return new ResultatEmprunt { Succes = false, Erreur = $"Tous les exemplaires du livre '{livre.Titre}' sont déjà empruntés." };
            }

            var emprunt = new Emprunt
            {
                LivreId = livreId,
                UtilisateurId = utilisateurId,
                DateEmprunt = maintenant,
                DateRetourPrevue = dateRetourPrevue,
                DateRetourEffective = null,
                Etat = EtatEmprunt.EnCours
            };

            _context.Emprunts.Add(emprunt);
            await _context.SaveChangesAsync();

            return new ResultatEmprunt { Succes = true, Emprunt = emprunt };
        }

        public async Task<(bool Succes, string? Erreur)> RetournerLivreAsync(int empruntId)
        {
            var emprunt = await _context.Emprunts.FindAsync(empruntId);
            if (emprunt == null)
            {
                return (false, $"Aucun emprunt trouvé avec l'id {empruntId}.");
            }

            if (emprunt.Etat == EtatEmprunt.Retourne)
            {
                return (false, "Ce livre a déjà été retourné.");
            }

            emprunt.DateRetourEffective = DateTime.Now;
            emprunt.Etat = EtatEmprunt.Retourne;

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<List<Emprunt>> GetEmpruntsActifsAsync(string? utilisateurIdFiltre)
        {
            var query = _context.Emprunts
                .Include(e => e.Livre)
                .Where(e => e.Etat == EtatEmprunt.EnCours)
                .AsQueryable();

            if (utilisateurIdFiltre != null)
            {
                query = query.Where(e => e.UtilisateurId == utilisateurIdFiltre);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Emprunt>> GetHistoriqueAsync(string? utilisateurIdFiltre)
        {
            var query = _context.Emprunts
                .Include(e => e.Livre)
                .AsQueryable();

            if (utilisateurIdFiltre != null)
            {
                query = query.Where(e => e.UtilisateurId == utilisateurIdFiltre);
            }

            return await query.OrderByDescending(e => e.DateEmprunt).ToListAsync();
        }

        public async Task<Emprunt?> GetEmpruntAsync(int id)
        {
            return await _context.Emprunts
                .Include(e => e.Livre)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}