using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Livres.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Livres
{
    public class UpdateLivreCommandHandler : IRequestHandler<UpdateLivreCommand, bool>
    {
        private readonly BiblioDbContext _context;

        public UpdateLivreCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateLivreCommand request, CancellationToken cancellationToken)
        {
            var livre = await _context.Livres.FindAsync(request.Id); //new object[] { request.Id } a la place de request id pour update et delete

            if (livre == null)
            {
                return false;
            }

            var auteur = await _context.Auteurs.FindAsync(request.AuteurId);
            if (auteur == null)
            {
                return false;
            }

            var categorie = await _context.Categories.FindAsync(request.CategorieId);
            if (categorie == null)
            {
                return false;
            }

            var isbnExiste = await _context.Livres.AnyAsync(l => l.ISBN == request.ISBN
             && l.Id != request.Id, cancellationToken);
            if (isbnExiste)
            {
                return false;
            }

            if (request.DatePublication > DateTime.Now)
            {
                return false;
            }

            livre.Titre = request.Titre;
            livre.ISBN = request.ISBN;
            livre.DatePublication = request.DatePublication;
            livre.NombrePages = request.NombrePages;
            livre.AuteurId = request.AuteurId;
            livre.CategorieId = request.CategorieId;
            livre.NombreExemplaires = request.NombreExemplaires;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}