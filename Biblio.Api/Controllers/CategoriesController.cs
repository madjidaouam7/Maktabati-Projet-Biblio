using MediatR;
using Biblio.Application.Features.Categories.Queries;
using Biblio.Application.Features.Categories.Commands;
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
    [Authorize] // s'applique par défaut à TOUTES les actions du Controller
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategorieDto>>> GetCategories(
            [FromQuery] string? recherche,
            [FromQuery] string? trierPar,
            [FromQuery] string? ordre,
            [FromQuery] int page = 1,
            [FromQuery] int taillePage = 10)
        {

            var query = new GetCategoriesQuery
            {
                Recherche = recherche,
                TrierPar = trierPar,
                Ordre = ordre,
                Page = page,
                TaillePage = taillePage
            };

            var (categories, total) = await _mediator.Send(query);

            var dtos = categories
                .Select(c => new CategorieDto
                {
                    Id = c.Id,
                    Nom = c.Nom,
                    Description = c.Description
                })
                .ToList();

            var resultat = new
            {
                Page = page,
                TaillePage = taillePage,
                TotalElements = total,
                TotalPages = (int)Math.Ceiling((double)total / taillePage),
                Categories = dtos
            };

            return Ok(resultat);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategorieDto>> GetCategorie(int id)
        {
            var query = new GetCategorieByIdQuery
            {
                Id = id
            };
            
            var categorie = await _mediator.Send(query);

            if (categorie == null)
            {
                return NotFound($"Aucune catégorie trouvée avec l'id {id}.");
            }

            var dto = new CategorieDto
            {
                Id = categorie.Id,
                Nom = categorie.Nom,
                Description = categorie.Description
            };

            return Ok(dto);
        }

        // POST: api/categories
        [HttpPost]
        [Authorize(Roles = "Admin,Bibliothecaire")]  // surcharge la règle du Controller pour cette action précise
        public async Task<ActionResult<CategorieDto>> CreateCategorie(CreateCategorieDto createDto)
        {
            var command = new CreateCategorieCommand
            {
                Nom = createDto.Nom,
                Description = createDto.Description
            };

            var categorie = await _mediator.Send(command);

            var dto = new CategorieDto
            {
                Id = categorie.Id,
                Nom = categorie.Nom,
                Description = categorie.Description
            };

            return CreatedAtAction(nameof(GetCategorie), new { id = categorie.Id }, dto);
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> UpdateCategorie(int id, UpdateCategorieDto updateDto)
        {
            var command = new UpdateCategorieCommand
            {
                Id = id,
                Nom = updateDto.Nom,
                Description = updateDto.Description
            };

            var succes = await _mediator.Send(command);
            if (!succes)
            {
                return NotFound($"Aucune catégorie trouvée avec l'id {id}.");
            }

            return NoContent();
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategorie(int id)
        {
            var command = new DeleteCategorieCommand
            {
                Id = id
            };

            var succes = await _mediator.Send(command);
            
            if (!succes)
            {
                return NotFound($"Aucune catégorie trouvée avec l'id {id}.");
            }

            return NoContent();
        }
    }
}