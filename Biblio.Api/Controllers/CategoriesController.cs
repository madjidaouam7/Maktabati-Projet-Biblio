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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategorieService _categorieService;

        public CategoriesController(ICategorieService categorieService)
        {
            _categorieService = categorieService;
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

            var (categories, total) = await _categorieService.GetCategoriesAsync(recherche, trierPar, ordre, page, taillePage);

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
            var categorie = await _categorieService.GetCategorieAsync(id);

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
            var categorie = new Categorie
            {
                Nom = createDto.Nom,
                Description = createDto.Description
            };

            var categorieCreee = await _categorieService.CreateCategorieAsync(categorie);

            var dto = new CategorieDto
            {
                Id = categorieCreee.Id,
                Nom = categorieCreee.Nom,
                Description = categorieCreee.Description
            };

            return CreatedAtAction(nameof(GetCategorie), new { id = categorieCreee.Id }, dto);
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> UpdateCategorie(int id, UpdateCategorieDto updateDto)
        {
            var categorie = new Categorie
            {
                Nom = updateDto.Nom,
                Description = updateDto.Description,
            };

            var succes = await _categorieService.UpdateCategorieAsync(id, categorie);
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
            var succes = await _categorieService.DeleteCategorieAsync(id);
            if (!succes)
            {
                return NotFound($"Aucune catégorie trouvée avec l'id {id}.");
            }

            return NoContent();
        }
    }
}