using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Auteurs.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Auteurs
{
    public class DeleteAuteurCommandHandler : IRequestHandler<DeleteAuteurCommand, bool>
    {
        private readonly BiblioDbContext _context;

        public DeleteAuteurCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteAuteurCommand request, CancellationToken cancellationToken)
        {
            var auteur = await _context.Auteurs.FindAsync(new object[] { request.Id }, cancellationToken);

            if (auteur == null)
            {
                return false;
            }

            _context.Auteurs.Remove(auteur);
            await _context.SaveChangesAsync(cancellationToken);
           
            return true;
        }
    }
}