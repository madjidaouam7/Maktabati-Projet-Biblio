using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Livres.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Livres
{
    public class CreateLivreCommandHandler : IRequestHandler<CreateLivreCommand, ResultatLivre>
    {
        private readonly BiblioDbContext _context;

        public CreateLivreCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<ResultatLivre> Handle(CreateLivreCommand request, CancellationToken cancellationToken)
        {

            var auteur = await _context.Auteurs.FindAsync(request.AuteurId);
            if (auteur == null)
            {
                return new ResultatLivre { Succes = false, Erreur = $"Aucun auteur trouvé avec l'id {request.AuteurId}." };
            }

            var categorie = await _context.Categories.FindAsync(request.CategorieId);
            if (categorie == null)
            {
                return new ResultatLivre { Succes = false, Erreur = $"Aucune catégorie trouvée avec l'id {request.CategorieId}." };
            }

            var isbnExiste = await _context.Livres.AnyAsync(l => l.ISBN == request.ISBN);
            if (isbnExiste)
            {
                return new ResultatLivre { Succes = false, Erreur = $"Un livre avec l'ISBN '{request.ISBN}' existe déjà." };
            }

            try
            {
                var livre = new Livre(request.Titre, request.ISBN, request.DatePublication, request.NombrePages,
                request.NombreExemplaires, request.AuteurId, request.CategorieId);

                _context.Livres.Add(livre);
                await _context.SaveChangesAsync(cancellationToken);

                return new ResultatLivre { Succes = true, Livre = livre };
            }
            catch (ArgumentException ex)
            {
                return new ResultatLivre { Succes = false, Erreur = ex.Message };
            }
        }

    }
}