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
    //[Authorize] // s'applique par défaut à TOUTES les actions du Controller
    public class AuteursController : ControllerBase
    {
        private readonly BiblioDbContext _context;

        public AuteursController(BiblioDbContext context)
        {
            _context = context;
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
            var query = _context.Auteurs
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                query = query.Where(a => a.Nom.Contains(recherche) || a.Prenom.Contains(recherche));
            }

            bool decroissant = ordre?.Trim().ToLower() == "desc";

            query = trierPar?.Trim().ToLower() switch
            {
                "nom" => decroissant ? query.OrderByDescending(a => a.Nom) : query.OrderBy(a => a.Nom),
                "prenom" => decroissant ? query.OrderByDescending(a => a.Prenom) : query.OrderBy(a => a.Prenom),
                "datenaissance" => decroissant ? query.OrderByDescending(a => a.DateNaissance) : query.OrderBy(a => a.DateNaissance),
                _ => query.OrderBy(c => c.Id) // tri par défaut si rien n'est précisé ou valeur inconnue
            };

            var totalElements = await query.CountAsync();

            if (page < 1) page = 1;
            if (taillePage < 1) taillePage = 10;


            var auteurs = await query
                .Skip((page - 1) * taillePage)
                .Take(taillePage)
                .Select(a => new AuteurDto
                {
                    Id = a.Id,
                    Nom = a.Nom,
                    Prenom = a.Prenom,
                    DateNaissance = a.DateNaissance
                })
                .ToListAsync();

            var resultat = new
            {
                Page = page,
                TaillePage = taillePage,
                TotalElements = totalElements,
                TotalPages = (int)Math.Ceiling((double)totalElements / taillePage),
                Auteurs = auteurs
            };

            return Ok(resultat);
        }

        // GET: api/auteurs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuteurDto>> GetAuteur(int id)
        {

            var auteur = await _context.Auteurs.FindAsync(id);

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
        //[Authorize(Roles = "Admin,Bibliothecaire")]  // surcharge la règle du Controller pour cette action précise
        public async Task<ActionResult<AuteurDto>> CreateAuteur(CreateAuteurDto createDto)
        {
            var auteur = new Auteur
            {
                Nom = createDto.Nom,
                Prenom = createDto.Prenom,
                DateNaissance = createDto.DateNaissance
            };

            _context.Auteurs.Add(auteur);
            await _context.SaveChangesAsync();

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
        //[Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> UpdateAuteur(int id, UpdateAuteurDto updateDto)
        {
            var auteur = await _context.Auteurs.FindAsync(id);

            if (auteur == null)
            {
                return NotFound($"Aucun auteur trouvé avec l'id {id}.");
            }

            auteur.Nom = updateDto.Nom;
            auteur.Prenom = updateDto.Prenom;
            auteur.DateNaissance = updateDto.DateNaissance;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/auteurs/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuteur(int id)
        {
            var auteur = await _context.Auteurs.FindAsync(id);

            if (auteur == null)
            {
                return NotFound($"Aucun auteur trouvé avec l'id {id}.");
            }

            _context.Auteurs.Remove(auteur);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}