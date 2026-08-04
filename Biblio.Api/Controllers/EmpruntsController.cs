using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Biblio.Application.Interfaces;
using Biblio.Infrastructure.Identity;
using Biblio.Api.DTOs;
using System.Security.Claims;

namespace Biblio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //Authorize]
    public class EmpruntsController : ControllerBase
    {
        private readonly IEmpruntService _empruntService;
        private readonly UserManager<Utilisateur> _userManager;

        public EmpruntsController(IEmpruntService empruntService, UserManager<Utilisateur> userManager)
        {
            _empruntService = empruntService;
            _userManager = userManager;
        }

        [HttpPost("emprunter")]
        public async Task<ActionResult<EmpruntDto>> EmprunterLivre(CreateEmpruntDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var resultat = await _empruntService.EmprunterLivreAsync(createDto.LivreId, userId, createDto.DateRetourPrevue);

            if (!resultat.Succes)
            {
                return BadRequest(resultat.Erreur);
            }

            var utilisateur = await _userManager.FindByIdAsync(userId);
            var emprunt = resultat.Emprunt!;

            var dto = new EmpruntDto
            {
                Id = emprunt.Id,
                LivreId = emprunt.LivreId,
                LivreTitre = emprunt.Livre?.Titre ?? "",
                Utilisateur = utilisateur?.Email ?? "Inconnu",
                DateEmprunt = emprunt.DateEmprunt,
                DateRetourPrevue = emprunt.DateRetourPrevue,
                DateRetourEffective = emprunt.DateRetourEffective,
                Etat = emprunt.Etat.ToString()
            };

            return CreatedAtAction(nameof(GetEmprunt), new { id = emprunt.Id }, dto);
        }

        [HttpPost("{id}/retourner")]
        public async Task<IActionResult> RetournerLivre(int id)
        {
            var (succes, erreur) = await _empruntService.RetournerLivreAsync(id);
            if (!succes)
            {
                return BadRequest(erreur);
            }
            return NoContent();
        }

        [HttpGet("actifs")]
        public async Task<ActionResult<IEnumerable<EmpruntDto>>> GetEmpruntsActifs()
        {
            string? filtre = null;
            if (!User.IsInRole("Admin") && !User.IsInRole("Bibliothecaire"))
            {
                filtre = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var emprunts = await _empruntService.GetEmpruntsActifsAsync(filtre);
            var dto = await ConstruireDtos(emprunts);

            return Ok(dto);
        }

        [HttpGet("historique")]
        public async Task<ActionResult<IEnumerable<EmpruntDto>>> GetHistorique()
        {
            string? filtre = null;
            if (!User.IsInRole("Admin") && !User.IsInRole("Bibliothecaire"))
            {
                filtre = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var emprunts = await _empruntService.GetHistoriqueAsync(filtre);
            var dto = await ConstruireDtos(emprunts);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmpruntDto>> GetEmprunt(int id)
        {
            var emprunt = await _empruntService.GetEmpruntAsync(id);
            if (emprunt == null)
            {
                return NotFound($"Aucun emprunt trouvé avec l'id {id}.");
            }

            var utilisateur = await _userManager.FindByIdAsync(emprunt.UtilisateurId);

            var dto = new EmpruntDto
            {
                Id = emprunt.Id,
                LivreId = emprunt.LivreId,
                LivreTitre = emprunt.Livre.Titre,
                Utilisateur = utilisateur?.Email ?? "Inconnu",
                DateEmprunt = emprunt.DateEmprunt,
                DateRetourPrevue = emprunt.DateRetourPrevue,
                DateRetourEffective = emprunt.DateRetourEffective,
                Etat = emprunt.Etat.ToString()
            };

            return Ok(dto);
        }

        private async Task<List<EmpruntDto>> ConstruireDtos(List<Biblio.Domain.Entities.Emprunt> emprunts)
        {
            var utilisateurIds = emprunts.Select(e => e.UtilisateurId).Distinct().ToList();
            var utilisateurs = await _userManager.Users
                .Where(u => utilisateurIds.Contains(u.Id))
                .ToListAsync();

            return emprunts.Select(e => new EmpruntDto
            {
                Id = e.Id,
                LivreId = e.LivreId,
                LivreTitre = e.Livre.Titre,
                Utilisateur = utilisateurs.FirstOrDefault(u => u.Id == e.UtilisateurId)?.Email ?? "Inconnu",
                DateEmprunt = e.DateEmprunt,
                DateRetourPrevue = e.DateRetourPrevue,
                DateRetourEffective = e.DateRetourEffective,
                Etat = e.Etat.ToString()
            }).ToList();
        }
    }
}