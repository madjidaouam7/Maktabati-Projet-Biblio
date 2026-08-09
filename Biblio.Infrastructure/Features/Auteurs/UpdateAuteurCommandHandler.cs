using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Auteurs.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Auteurs
{
    public class UpdateAuteurCommandHandler : IRequestHandler<UpdateAuteurCommand, bool>
    {
        private readonly BiblioDbContext _context;

        public UpdateAuteurCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateAuteurCommand request, CancellationToken cancellationToken)
        {
            var auteur = await _context.Auteurs.FindAsync(new object[] { request.Id }, cancellationToken);

            if (auteur == null)
            {
                return false;
            }

            auteur.Nom = request.Nom;
            auteur.Prenom = request.Prenom;
            auteur.DateNaissance = request.DateNaissance;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}