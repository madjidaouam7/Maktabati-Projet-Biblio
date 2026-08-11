using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Livres.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Livres
{
    public class UpdateLivreCommandHandler : IRequestHandler<UpdateLivreCommand, ResultatLivre>
    {
        private readonly BiblioDbContext _context;

        public UpdateLivreCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<ResultatLivre> Handle(UpdateLivreCommand request, CancellationToken cancellationToken)
        {
            var livre = await _context.Livres.FindAsync(new object[] { request.Id }, cancellationToken); //new object[] { request.Id } a la place de request id pour update et delete

            if (livre == null)
            {
                return new ResultatLivre { Succes = false, Erreur = $"Aucun livre trouvé avec l'id {request.Id}." };
            }

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

            var isbnExiste = await _context.Livres.AnyAsync(l => l.ISBN == request.ISBN
             && l.Id != request.Id, cancellationToken);
            if (isbnExiste)
            {
                return new ResultatLivre { Succes = false, Erreur = $"Un livre avec l'ISBN '{request.ISBN}' existe déjà." };
            }

            if (request.DatePublication > DateTime.Now)
            {
                return new ResultatLivre { Succes = false, Erreur = "La date de publication ne peut pas être dans le futur." };
            }

            livre.Modifier(request.Titre, request.ISBN, request.DatePublication, request.NombrePages,
            request.NombreExemplaires, request.AuteurId, request.CategorieId);

            await _context.SaveChangesAsync(cancellationToken);
            return new ResultatLivre{ Succes = true, Livre = livre };
        }
    }
}