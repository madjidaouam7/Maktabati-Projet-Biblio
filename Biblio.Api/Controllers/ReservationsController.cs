using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Biblio.Application.Features.Reservations.Commands;
using Biblio.Application.Features.Reservations.Queries;
using Biblio.Infrastructure.Identity;
using Biblio.Api.DTOs;
using System.Security.Claims;

namespace Biblio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<Utilisateur> _userManager;

        public ReservationsController(IMediator mediator, UserManager<Utilisateur> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpPost("reserver")]
        public async Task<ActionResult<ReservationDto>> ReserverLivre(CreateReservationDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new ReserverLivreCommand
            {
                LivreId = createDto.LivreId,
                UtilisateurId = userId
            };

            var resultat = await _mediator.Send(command);

            if (!resultat.Succes)
            {
                return BadRequest(resultat.Erreur);
            }

            var utilisateur = await _userManager.FindByIdAsync(userId);

            var dto = new ReservationDto
            {
                Id = resultat.Reservation!.Id,
                LivreId = resultat.Reservation.LivreId,
                LivreTitre = resultat.Livre?.Titre ?? "",
                Utilisateur = utilisateur?.Email ?? "Inconnu",
                DateReservation = resultat.Reservation.DateReservation,
                Etat = resultat.Reservation.Etat.ToString()
            };

            return Ok(dto);
        }

        [HttpPost("{id}/annuler")]
        public async Task<IActionResult> AnnulerReservation(int id)
        {
            var resultat = await _mediator.Send(new AnnulerReservationCommand { ReservationId = id });
            if (!resultat.Succes)
            {
                return BadRequest(resultat.Erreur);
            }
            return NoContent();
        }

        [HttpPost("{id}/honorer")]
        public async Task<IActionResult> HonorerReservation(int id)
        {
            var resultat = await _mediator.Send(new HonorerReservationCommand { ReservationId = id });
            if (!resultat.Succes)
            {
                return BadRequest(resultat.Erreur);
            }
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations()
        {
            string? filtre = null;
            if (!User.IsInRole("Admin") && !User.IsInRole("Bibliothecaire"))
            {
                filtre = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var reservations = await _mediator.Send(new GetReservationsQuery { UtilisateurIdFiltre = filtre });

            var utilisateurIds = reservations.Select(r => r.UtilisateurId).Distinct().ToList();
            var utilisateurs = await _userManager.Users
                .Where(u => utilisateurIds.Contains(u.Id))
                .ToListAsync();

            var dtos = reservations.Select(r => new ReservationDto
            {
                Id = r.Id,
                LivreId = r.LivreId,
                LivreTitre = r.Livre.Titre,
                Utilisateur = utilisateurs.FirstOrDefault(u => u.Id == r.UtilisateurId)?.Email ?? "Inconnu",
                DateReservation = r.DateReservation,
                Etat = r.Etat.ToString()
            }).ToList();

            return Ok(dtos);
        }
    }
}