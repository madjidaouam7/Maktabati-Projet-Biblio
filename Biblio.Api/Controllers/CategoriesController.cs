using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblio.Api.Data;
using Biblio.Api.Models;
using Biblio.Api.DTOs;

namespace Biblio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly BiblioDbContext _context;

        public CategoriesController(BiblioDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategorieDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new CategorieDto
                {
                    Id = c.Id,
                    Nom = c.Nom,
                    Description = c.Description
                })
                .ToListAsync();

            return Ok(categories);
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