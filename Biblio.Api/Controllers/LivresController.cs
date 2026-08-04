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
    //[Authorize]
    public class LivresController : ControllerBase
    {
        private readonly BiblioDbContext _context;
        private readonly ILivreService _livreService;

        public LivresController(ILivreService livreService)
        {
            _livreService = livreService;
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
            var (livres, total) = await _livreService.GetLivresAsync(recherche, trierPar, ordre, page, taillePage);

            var dtos = livres
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
              .ToList();


            var resultat = new
            {
                Page = page,
                TaillePage = taillePage,
                TotalElements = total,
                totalPages = (int)Math.Ceiling((double)total / taillePage),
                Livres = dtos
            };

            return Ok(resultat);
        }





        // GET: api/livres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LivreDto>> GetLivre(int id)
        {
            var livre = await _livreService.GetLivreAsync(id);

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

            var resultat = await _livreService.CreateLivreAsync(livre);

            if (!resultat.Succes)
            {
                return BadRequest(resultat.Erreur);
            }

            var dto = new LivreDto
            {
                Id = resultat.Livre.Id,
                Titre = resultat.Livre.Titre,
                ISBN = resultat.Livre.ISBN,
                DatePublication = resultat.Livre.DatePublication,
                NombrePages = resultat.Livre.NombrePages,
                NombreExemplaires = resultat.Livre.NombreExemplaires,
                AuteurId = resultat.Livre.AuteurId,
                AuteurNomComplet = resultat.Livre.Auteur.Nom + " " + resultat.Livre.Auteur.Prenom,
                CategorieId = resultat.Livre.CategorieId,
                CategorieNom = resultat.Livre.Categorie.Nom
            };

            return CreatedAtAction(nameof(GetLivre), new { id = livre.Id }, dto);
        }




        // PUT: api/livres/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> UpdateLivre(int id, UpdateLivreDto updateDto)
        {
            var livre = new Livre
            {
                Titre = updateDto.Titre,
                ISBN = updateDto.ISBN,
                DatePublication = updateDto.DatePublication,
                NombrePages = updateDto.NombrePages,
                NombreExemplaires = updateDto.NombreExemplaires,
                AuteurId = updateDto.AuteurId,
                CategorieId = updateDto.CategorieId
            };

            var resultat = await _livreService.UpdateLivreAsync(id, livre);
           
            if (!resultat.Succes)
            {
                return BadRequest(resultat.Erreur);
            }

            return NoContent();
        }




        // DELETE: api/livres/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLivre(int id)
        {
            var (succes, erreur) = await _livreService.DeleteLivreAsync(id);

            if (!succes)
            {
                return BadRequest(erreur);
            }

            return NoContent();
        }


        


        [HttpGet("disponibles")]
        public async Task<ActionResult<IEnumerable<LivreDisponibleDto>>> GetLivresDisponibles()
        {
            var resultats = await _livreService.GetLivresDisponiblesAsync();

            var dtos = resultats
                .Select(x => new LivreDisponibleDto
                {
                    Id = x.Livre.Id,
                    Titre = x.Livre.Titre,
                    AuteurNomComplet = x.Livre.Auteur.Nom + " " + x.Livre.Auteur.Prenom,
                    NombreExemplaires = x.Livre.NombreExemplaires,
                    NombreExemplairesDisponibles = x.NombreExemplairesDisponibles
                })
                .ToList();

            return Ok(dtos);
        } 
    }
}