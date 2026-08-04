using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblio.Infrastructure.Data;
using Biblio.Domain.Entities;
using Biblio.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Biblio.Application.Interfaces;

namespace Biblio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // s'applique par défaut à TOUTES les actions du Controller
    public class AuteursController : ControllerBase
    {
        private readonly IAuteurService _auteurService;

        public AuteursController(IAuteurService auteurService)
        {
            _auteurService = auteurService;
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
            var (auteurs, total) = await _auteurService.GetAuteursAsync(recherche, trierPar, ordre, page, taillePage);

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
            var auteur = await _auteurService.GetAuteurAsync(id);

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
            var auteur = new Auteur
            {
                Nom = createDto.Nom,
                Prenom = createDto.Prenom,
                DateNaissance = createDto.DateNaissance
            };

            var auteurCree = await _auteurService.CreateAuteurAsync(auteur);

            var dto = new AuteurDto
            {
                Id = auteurCree.Id,
                Nom = auteurCree.Nom,
                Prenom = auteurCree.Prenom,
                DateNaissance = auteurCree.DateNaissance
            };

            return CreatedAtAction(nameof(GetAuteur), new { id = auteur.Id }, dto);
        }

        // PUT: api/auteurs/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> UpdateAuteur(int id, UpdateAuteurDto updateDto)
        {

             var auteur = new Auteur
            {
                Nom = updateDto.Nom,
                Prenom = updateDto.Prenom,
                DateNaissance = updateDto.DateNaissance
            };

            var succes = await _auteurService.UpdateAuteurAsync(id, auteur);

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
            var succes = await _auteurService.DeleteAuteurAsync(id);

            if (!succes)
            {
                return NotFound($"Aucun auteur trouvé avec l'id {id}.");
            }

            return NoContent();
        }
    }
}