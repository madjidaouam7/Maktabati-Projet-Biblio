using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblio.Api.Data;
using Biblio.Api.Models;
using Biblio.Api.DTOs;

namespace Biblio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpruntsController : ControllerBase
    {
        private readonly BiblioDbContext _context;

        public EmpruntsController(BiblioDbContext context)
        {
            _context = context;
        }

        // POST: api/emprunts/emprunter
        [HttpPost("emprunter")]
        public async Task<ActionResult<EmpruntDto>> EmprunterLivre(CreateEmpruntDto createDto)
        {
            var maintenant = DateTime.Now;

            if (createDto.DateRetourPrevue < maintenant)
            {
                return BadRequest("La date de retour prévue doit être supérieure ou égale à la date d'emprunt.");
            }

            var livre = await _context.Livres.FindAsync(createDto.LivreId);
            if (livre == null)
            {
                return BadRequest($"Aucun livre trouvé avec l'id {createDto.LivreId}.");
            }

            var NombreEmpruntsEnCours = await _context.Emprunts
                .CountAsync(e => e.LivreId == createDto.LivreId && e.Etat == EtatEmprunt.EnCours);

            if (NombreEmpruntsEnCours >= livre.NombreExemplaires)
            {
                return BadRequest($"Tout les exemplaires du livre '{livre.Titre}' sont déja empruntés.");
            }

            var emprunt = new Emprunt
            {
                LivreId = createDto.LivreId,
                Utilisateur = createDto.Utilisateur,
                DateEmprunt = maintenant,
                DateRetourPrevue = createDto.DateRetourPrevue,
                DateRetourEffective = null,
                Etat = EtatEmprunt.EnCours
            };

            _context.Emprunts.Add(emprunt);
            await _context.SaveChangesAsync();

            var dto = new EmpruntDto
            {
                Id = emprunt.Id,
                LivreId = emprunt.LivreId,
                LivreTitre = livre.Titre,
                Utilisateur = emprunt.Utilisateur,
                DateEmprunt = emprunt.DateEmprunt,
                DateRetourPrevue = emprunt.DateRetourPrevue,
                DateRetourEffective = emprunt.DateRetourEffective,
                Etat = emprunt.Etat.ToString()
            };

            return CreatedAtAction(nameof(GetEmprunt), new { id = emprunt.Id }, dto);
        }

        // POST: api/emprunts/5/retourner
        [HttpPost("{id}/retourner")]
        public async Task<IActionResult> RetournerLivre(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);

            if (emprunt == null)
            {
                return NotFound($"Aucun emprunt trouvé avec l'id {id}.");
            }

            if (emprunt.Etat == EtatEmprunt.Retourne)
            {
                return BadRequest("Ce livre a déjà été retourné.");
            }

            emprunt.DateRetourEffective = DateTime.Now;
            emprunt.Etat = EtatEmprunt.Retourne;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/emprunts/actifs
        [HttpGet("actifs")]
        public async Task<ActionResult<IEnumerable<EmpruntDto>>> GetEmpruntsActifs()
        {
            var emprunts = await _context.Emprunts
                .Include(e => e.Livre)
                .Where(e => e.Etat == EtatEmprunt.EnCours)
                .Select(e => new EmpruntDto
                {
                    Id = e.Id,
                    LivreId = e.LivreId,
                    LivreTitre = e.Livre.Titre,
                    Utilisateur = e.Utilisateur,
                    DateEmprunt = e.DateEmprunt,
                    DateRetourPrevue = e.DateRetourPrevue,
                    DateRetourEffective = e.DateRetourEffective,
                    Etat = e.Etat.ToString()
                })
                .ToListAsync();

            return Ok(emprunts);
        }

        // GET: api/emprunts/historique
        [HttpGet("historique")]
        public async Task<ActionResult<IEnumerable<EmpruntDto>>> GetHistorique()
        {
            var emprunts = await _context.Emprunts
                .Include(e => e.Livre)
                .OrderByDescending(e => e.DateEmprunt)
                .Select(e => new EmpruntDto
                {
                    Id = e.Id,
                    LivreId = e.LivreId,
                    LivreTitre = e.Livre.Titre,
                    Utilisateur = e.Utilisateur,
                    DateEmprunt = e.DateEmprunt,
                    DateRetourPrevue = e.DateRetourPrevue,
                    DateRetourEffective = e.DateRetourEffective,
                    Etat = e.Etat.ToString()
                })
                .ToListAsync();

            return Ok(emprunts);
        }

        // GET: api/emprunts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmpruntDto>> GetEmprunt(int id)
        {
            var emprunt = await _context.Emprunts
                .Include(e => e.Livre)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emprunt == null)
            {
                return NotFound($"Aucun emprunt trouvé avec l'id {id}.");
            }

            var dto = new EmpruntDto
            {
                Id = emprunt.Id,
                LivreId = emprunt.LivreId,
                LivreTitre = emprunt.Livre.Titre,
                Utilisateur = emprunt.Utilisateur,
                DateEmprunt = emprunt.DateEmprunt,
                DateRetourPrevue = emprunt.DateRetourPrevue,
                DateRetourEffective = emprunt.DateRetourEffective,
                Etat = emprunt.Etat.ToString()
            };

            return Ok(dto);
        }
    }
}