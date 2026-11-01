using MediatR;
//using Biblio.Application.Features.Emprunts.Queries;
using Biblio.Application.Features.Emprunts.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Biblio.Infrastructure.Identity;
using Biblio.Api.DTOs;
using System.Security.Claims;
using Biblio.Application.Features.Emprunts.Queries;

namespace Biblio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpruntsController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly UserManager<Utilisateur> _userManager;

        public EmpruntsController(IMediator mediator, UserManager<Utilisateur> userManager)
        {
            _mediator = mediator;
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

            var command = new EmprunterLivreCommand
            {
                LivreId = createDto.LivreId,
                UtilisateurId = userId,
                DateRetourPrevue = createDto.DateRetourPrevue
            };

            var resultat = await _mediator.Send(command);

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
                LivreTitre = resultat.Livre?.Titre ?? "",
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
            var command = new RetournerLivreCommand
            {
                Id = id
            };

            var resultat = await _mediator.Send(command);
            if (!resultat.Succes)
            {
                return BadRequest(resultat.Erreur);
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

            var query = new GetEmpruntsActifsQuery
            {
                UtilisateurIdFiltre = filtre
            };

            var emprunts = await _mediator.Send(query);
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

            var query = new GetHistoriqueEmpruntsQuery
            {
                UtilisateurIdFiltre = filtre
            };

            var emprunts = await _mediator.Send(query);
            var dto = await ConstruireDtos(emprunts);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmpruntDto>> GetEmprunt(int id)
        {
            var query = new GetEmpruntByIdQuery
            {
                Id = id
            };

            var emprunt = await _mediator.Send(query);
            if (emprunt == null)
            {
                return NotFound($"Aucun emprunt trouvé avec l'id {id}.");
            }

            if (!User.IsInRole("Admin") &&
            !User.IsInRole("Bibliothecaire") &&
            emprunt.UtilisateurId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
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