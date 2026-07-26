using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblio.Api.Data;
using Biblio.Api.Models;
using Biblio.Api.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Biblio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // s'applique par défaut à TOUTES les actions du Controller
    public class CategoriesController : ControllerBase
    {
        private readonly BiblioDbContext _context;

        public CategoriesController(BiblioDbContext context)
        {
            _context = context;
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
            var query = _context.Categories
                .AsQueryable();
                
                if (!string.IsNullOrWhiteSpace(recherche))
                {
                    query = query.Where(c => c.Nom.Contains(recherche));
                }

                bool decroissant = ordre?.Trim().ToLower() == "desc";

                query = trierPar?.Trim().ToLower() switch
                {
                    "nom" => decroissant ? query.OrderByDescending(c => c.Nom) : query.OrderBy(c => c.Nom),
                    "description" => decroissant ? query.OrderByDescending(c => c.Description) : query.OrderBy(c => c.Description),
                    _ => query.OrderBy(c => c.Id) // tri par défaut si rien n'est précisé ou valeur inconnue
                };

                var totalElements = await query.CountAsync();

                if (page < 1) page = 1;
                if (taillePage < 1) taillePage = 10;


            var categories = await query
                .Skip((page - 1) * taillePage)
                .Take(taillePage)
                .Select(c => new CategorieDto
                {
                    Id = c.Id,
                    Nom = c.Nom,
                    Description = c.Description
                })
                .ToListAsync();

                var resultat = new
                {
                     Page = page,
                     TaillePage = taillePage,
                     TotalElements = totalElements,
                     TotalPages = (int)Math.Ceiling((double)totalElements / taillePage),
                     Categories = categories
                };

            return Ok(resultat);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategorieDto>> GetCategorie(int id)
        {
            var categorie = await _context.Categories.FindAsync(id);

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

            _context.Categories.Add(categorie);
            await _context.SaveChangesAsync();

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
            var categorie = await _context.Categories.FindAsync(id);

            if (categorie == null)
            {
                return NotFound($"Aucune catégorie trouvée avec l'id {id}.");
            }

            categorie.Nom = updateDto.Nom;
            categorie.Description = updateDto.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategorie(int id)
        {
            var categorie = await _context.Categories.FindAsync(id);

            if (categorie == null)
            {
                return NotFound($"Aucune catégorie trouvée avec l'id {id}.");
            }

            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}