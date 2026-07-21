using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblio.Api.Data;
using Biblio.Api.Models;
using Biblio.Api.DTOs;

namespace Biblio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuteursController : ControllerBase
    {
        private readonly BiblioDbContext _context;

        public AuteursController(BiblioDbContext context)
        {
            _context = context;
        }

        // GET: api/auteurs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuteurDto>>> GetAuteurs()
        {
            var auteurs = await _context.Auteurs
                .Select(a => new AuteurDto
                {
                    Id = a.Id,
                    Nom = a.Nom,
                    Prenom = a.Prenom,
                    DateNaissance = a.DateNaissance
                })
                .ToListAsync();

            return Ok(auteurs);
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