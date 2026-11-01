using MediatR;
using Biblio.Application.Features.Auteurs.Queries;
using Biblio.Application.Features.Auteurs.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblio.Infrastructure.Data;
using Biblio.Domain.Entities;
using Biblio.Api.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Biblio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuteursController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuteursController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/auteurs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuteurDto>>> GetAuteurs(
            [FromQuery] string? recherche,
            [FromQuery] string? trierPar,
            [FromQuery] string? ordre,
            [FromQuery] int page = 1,
            [FromQuery] int taillePage = 10)

        {
            var query = new GetAuteursQuery
            {
                Recherche = recherche,
                TrierPar = trierPar,
                Ordre = ordre,
                Page = page,
                TaillePage = taillePage
            };

            var (auteurs, total) = await _mediator.Send(query);

            var dtos = auteurs
                .Select(a => new AuteurDto
                {
                    Id = a.Id,
                    Nom = a.Nom,
                    Prenom = a.Prenom,
                    DateNaissance = a.DateNaissance
                })
                .ToList();

            var resultat = new
            {
                Page = page,
                TaillePage = taillePage,
                TotalElements = total,
                TotalPages = (int)Math.Ceiling((double)total / taillePage),
                Auteurs = dtos
            };

            return Ok(resultat);
        }




        // GET: api/auteurs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuteurDto>> GetAuteur(int id)
        {
            var query = new GetAuteursByIdQuery
            {
                Id = id
            };

            var auteur = await _mediator.Send(query);

            if (auteur == null)
            {
                return NotFound($"Aucun auteur trouvé avec l'id {id}.");
            }

            var dto = new AuteurDto
            {
                Id = auteur.Id,
                Nom = auteur.Nom,
                Prenom = auteur.Prenom,
                DateNaissance = auteur.DateNaissance
            };

            return Ok(dto);
        }




        // POST: api/auteurs
        [HttpPost]
        [Authorize(Roles = "Admin,Bibliothecaire")]  // surcharge la règle du Controller pour cette action précise
        public async Task<ActionResult<AuteurDto>> CreateAuteur(CreateAuteurDto createDto)
        {
            var command = new CreateAuteurCommand
            {
                Nom = createDto.Nom,
                Prenom = createDto.Prenom,
                DateNaissance = createDto.DateNaissance
            };

            var auteur = await _mediator.Send(command);

            var dto = new AuteurDto
            {
                Id = auteur.Id,
                Nom = auteur.Nom,
                Prenom = auteur.Prenom,
                DateNaissance = auteur.DateNaissance
            };

            return CreatedAtAction(nameof(GetAuteur), new { id = auteur.Id }, dto);
        }

        // PUT: api/auteurs/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> UpdateAuteur(int id, UpdateAuteurDto updateDto)
        {
            var command = new UpdateAuteurCommand
            {
                Id = id,
                Nom = updateDto.Nom,
                Prenom = updateDto.Prenom,
                DateNaissance = updateDto.DateNaissance
            };
            

            var succes = await _mediator.Send(command);

            if (!succes)
            {
                return NotFound($"Aucun auteur trouvé avec l'id {id}.");
            }

            return NoContent();
        }

        // DELETE: api/auteurs/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuteur(int id)
        {
            var command = new DeleteAuteurCommand
            {
                Id = id
            };
            
            var succes = await _mediator.Send(command);

            if (!succes)
            {
                return NotFound($"Aucun auteur trouvé avec l'id {id}.");
            }

            return NoContent();
        }
    }
}