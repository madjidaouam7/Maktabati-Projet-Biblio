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
    [Authorize]
    public class LivresController : ControllerBase
    {
        private readonly BiblioDbContext _context;

        public LivresController(BiblioDbContext context)
        {
            _context = context;
        }

        // GET: api/livres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LivreDto>>> GetLivres(
            [FromQuery] string? recherche,
            [FromQuery] string? trierPar,
            [FromQuery] string? ordre,
            [FromQuery] int page = 1,
            [FromQuery] int taillePage = 10)
        {
            var query = _context.Livres
                .Include(l => l.Auteur)
                .Include(l => l.Categorie)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                query = query.Where(l => l.Titre.Contains(recherche));
            }

            bool decroissant = ordre?.Trim().ToLower() == "desc";

            query = trierPar?.Trim().ToLower() switch
            {
                "titre" => decroissant ? query.OrderByDescending(l => l.Titre) : query.OrderBy(l => l.Titre),
                "datepublication" => decroissant ? query.OrderByDescending(l => l.DatePublication) : query.OrderBy(l => l.DatePublication),
                "nombrepages" => decroissant ? query.OrderByDescending(l => l.NombrePages) : query.OrderBy(l => l.NombrePages),
                _ => query.OrderBy(l => l.Id) // tri par défaut si rien n'est précisé ou valeur inconnue
            };

            var totalElements = await query.CountAsync();

            if (page < 1) page = 1;
            if (taillePage < 1) taillePage = 10;

            var livres = await query
                .Skip((page - 1) * taillePage)
                .Take(taillePage)
                .Select(l => new LivreDto
                {
                    Id = l.Id,
                    Titre = l.Titre,
                    ISBN = l.ISBN,
                    DatePublication = l.DatePublication,
                    NombrePages = l.NombrePages,
                    NombreExemplaires = l.NombreExemplaires,
                    AuteurId = l.AuteurId,
                    AuteurNomComplet = l.Auteur.Nom + " " + l.Auteur.Prenom,
                    CategorieId = l.CategorieId,
                    CategorieNom = l.Categorie.Nom
                })
                .ToListAsync();

            var resultat = new
            {
                Page = page,
                TaillePage = taillePage,
                TotalElements = totalElements,
                totalPages = (int)Math.Ceiling((double)totalElements / taillePage),
                Livres = livres
            };

            return Ok(resultat);
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
                NombreExemplaires = livre.NombreExemplaires,
                AuteurId = livre.AuteurId,
                AuteurNomComplet = livre.Auteur.Nom + " " + livre.Auteur.Prenom,
                CategorieId = livre.CategorieId,
                CategorieNom = livre.Categorie.Nom
            };

            return Ok(dto);
        }

        // POST: api/livres
        [HttpPost]
        [Authorize(Roles = "Admin,Bibliothecaire")]  // surcharge la règle du Controller pour cette action précise
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
                NombreExemplaires = createDto.NombreExemplaires,
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
                NombreExemplaires = livre.NombreExemplaires,
                AuteurId = livre.AuteurId,
                AuteurNomComplet = auteur.Nom + " " + auteur.Prenom,
                CategorieId = livre.CategorieId,
                CategorieNom = categorie.Nom
            };

            return CreatedAtAction(nameof(GetLivre), new { id = livre.Id }, dto);
        }

        // PUT: api/livres/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Bibliothecaire")]
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
            livre.NombreExemplaires = updateDto.NombreExemplaires;
            livre.AuteurId = updateDto.AuteurId;
            livre.CategorieId = updateDto.CategorieId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/livres/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLivre(int id)
        {
            var livre = await _context.Livres.FindAsync(id);

            if (livre == null)
            {
                return NotFound($"Aucun livre trouvé avec l'id {id}.");
            }

            var aDesEmprunts = await _context.Emprunts.AnyAsync(e => e.LivreId == id);
            if (aDesEmprunts)
            {
                return BadRequest($"Impossible de supprimer le livre'{livre.Titre}' : il possède un historique d'emprunts.");
            }

            _context.Livres.Remove(livre);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpGet("disponibles")]
        public async Task<ActionResult<IEnumerable<LivreDisponibleDto>>> GetLivresDisponibles()
        {
            var livres = await _context.Livres
                .Include(l => l.Auteur)
                .Include(l => l.Emprunts)
                .ToListAsync();

            var resultat = livres
                .Select(l => new LivreDisponibleDto
                {
                    Id = l.Id,
                    Titre = l.Titre,
                    AuteurNomComplet = l.Auteur.Nom + " " + l.Auteur.Prenom,
                    NombreExemplaires = l.NombreExemplaires,
                    NombreExemplairesDisponibles = l.NombreExemplaires - l.Emprunts.Count(e => e.Etat == EtatEmprunt.EnCours)
                })
                .Where(dto => dto.NombreExemplairesDisponibles > 0)
                .ToList();

            return Ok(resultat);
        }
    }
}