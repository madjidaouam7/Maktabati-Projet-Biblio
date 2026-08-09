using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Auteurs.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Auteurs
{
    public class CreateAuteurCommandHandler : IRequestHandler<CreateAuteurCommand, Auteur>
    {
        private readonly BiblioDbContext _context;

        public CreateAuteurCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<Auteur> Handle(CreateAuteurCommand request, CancellationToken cancellationToken)
        {
            var auteur = new Auteur
            {
                Nom = request.Nom,
                Prenom = request.Prenom,
                DateNaissance = request.DateNaissance
            };

            _context.Auteurs.Add(auteur);
            await _context.SaveChangesAsync(cancellationToken);

            return auteur;
        }
    }
}