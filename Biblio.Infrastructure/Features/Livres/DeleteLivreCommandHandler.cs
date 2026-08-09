using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Livres.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Livres
{
    public class DeleteLivreCommandHandler : IRequestHandler<DeleteLivreCommand, bool>
    {
        private readonly BiblioDbContext _context;

        public DeleteLivreCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteLivreCommand request, CancellationToken cancellationToken)
        {
            var livre = await _context.Livres.FindAsync(request.Id);
           
            if (livre == null)
            {
                return false;
            }

            _context.Livres.Remove(livre);
            await _context.SaveChangesAsync(cancellationToken);
           
            return true;
        }
    }
}