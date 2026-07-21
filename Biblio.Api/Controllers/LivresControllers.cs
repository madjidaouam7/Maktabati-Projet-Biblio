using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblio.Api.Data;
using Biblio.Api.Models;
using Biblio.Api.DTOs;

namespace Biblio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LivresController : ControllerBase
    {
        private readonly BiblioDbContext _context;

        public LivresController(BiblioDbContext context)
        {
            _context = context;
        }

        // GET: api/livres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LivreDto>>> GetLivres()
        {
            var livres = await _context.Livres
                .Include(l => l.Auteur)
                .Include(l => l.Categorie)
                .Select(l => new LivreDto
                {
                    Id = l.Id,
                    Titre = l.Titre,
                    ISBN = l.ISBN,
                    DatePublication = l.DatePublication,
                    NombrePages = l.NombrePages,
                    AuteurId = l.AuteurId,
                    AuteurNomComplet = l.Auteur.Nom + " " + l.Auteur.Prenom,
                    CategorieId = l.CategorieId,
                    CategorieNom = l.Categorie.Nom
                })
                .ToListAsync();

            return Ok(livres);
        }

        // GET: api/livres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LivreDto>> GetLivre(int id)
        {
            var livre = await _context.Livres
                .Include(l => l.Auteur)
                .Include(l => l.Categorie)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (livre == null)
            {
                return NotFound($"Aucun livre trouvé avec l'id {id}.");
            }

            var dto = new LivreDto
            {
                Id = livre.Id,
                Titre = livre.Titre,
                ISBN = livre.ISBN,
                DatePublication = livre.DatePublication,
                NombrePages = livre.NombrePages,
                AuteurId = livre.AuteurId,
                AuteurNomComplet = livre.Auteur.Nom + " " + livre.Auteur.Prenom,
                CategorieId = livre.CategorieId,
                CategorieNom = livre.Categorie.Nom
            };

            return Ok(dto);
        }

        // POST: api/livres
        [HttpPost]
        public async Task<ActionResult<LivreDto>> CreateLivre(CreateLivreDto createDto)
        {
            var auteur = await _context.Auteurs.FindAsync(createDto.AuteurId);
            if (auteur == null)
            {
                return BadRequest($"Aucun auteur trouvé avec l'id {createDto.AuteurId}.");
            }

            var categorie = await _context.Categories.FindAsync(createDto.CategorieId);
            if (categorie == null)
            {
                return BadRequest($"Aucune catégorie trouvée avec l'id {createDto.CategorieId}.");
            }

            var isbnExiste = await _context.Livres.AnyAsync(l => l.ISBN == createDto.ISBN);
            if (isbnExiste)
            {
                return BadRequest($"Un livre avec l'ISBN '{createDto.ISBN}' existe déjà.");
            }

            if (createDto.DatePublication > DateTime.Now)
            {
                return BadRequest("La date de publication ne peut pas être dans le futur.");
            }

            var livre = new Livre
            {
                Titre = createDto.Titre,
                ISBN = createDto.ISBN,
                DatePublication = createDto.DatePublication,
                NombrePages = createDto.NombrePages,
                AuteurId = createDto.AuteurId,
                CategorieId = createDto.CategorieId
            };

            _context.Livres.Add(livre);
            await _context.SaveChangesAsync();

            var dto = new LivreDto
            {
                Id = livre.Id,
                Titre = livre.Titre,
                ISBN = livre.ISBN,
                DatePublication = livre.DatePublication,
                NombrePages = livre.NombrePages,
                AuteurId = livre.AuteurId,
                AuteurNomComplet = auteur.Nom + " " + auteur.Prenom,
                CategorieId = livre.CategorieId,
                CategorieNom = categorie.Nom
            };

            return CreatedAtAction(nameof(GetLivre), new { id = livre.Id }, dto);
        }

        // PUT: api/livres/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLivre(int id, UpdateLivreDto updateDto)
        {
            var livre = await _context.Livres.FindAsync(id);

            if (livre == null)
            {
                return NotFound($"Aucun livre trouvé avec l'id {id}.");
            }

            var auteurExiste = await _context.Auteurs.AnyAsync(a => a.Id == updateDto.AuteurId);
            if (!auteurExiste)
            {
                return BadRequest($"Aucun auteur trouvé avec l'id {updateDto.AuteurId}.");
            }

            var categorieExiste = await _context.Categories.AnyAsync(c => c.Id == updateDto.CategorieId);
            if (!categorieExiste)
            {
                return BadRequest($"Aucune catégorie trouvée avec l'id {updateDto.CategorieId}.");
            }

            var isbnExiste = await _context.Livres
                .AnyAsync(l => l.ISBN == updateDto.ISBN && l.Id != id);
            if (isbnExiste)
            {
                return BadRequest($"Un autre livre utilise déjà l'ISBN '{updateDto.ISBN}'.");
            }

            if (updateDto.DatePublication > DateTime.Now)
            {
                return BadRequest("La date de publication ne peut pas être dans le futur.");
            }

            livre.Titre = updateDto.Titre;
            livre.ISBN = updateDto.ISBN;
            livre.DatePublication = updateDto.DatePublication;
            livre.NombrePages = updateDto.NombrePages;
            livre.AuteurId = updateDto.AuteurId;
            livre.CategorieId = updateDto.CategorieId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/livres/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivre(int id)
        {
            var livre = await _context.Livres.FindAsync(id);

            if (livre == null)
            {
                return NotFound($"Aucun livre trouvé avec l'id {id}.");
            }

            _context.Livres.Remove(livre);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}