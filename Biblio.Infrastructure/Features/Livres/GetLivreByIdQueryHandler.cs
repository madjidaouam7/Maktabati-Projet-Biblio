using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Livres.Queries;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Livres
{
    public class GetLivreByIdQueryHandler : IRequestHandler<GetLivreByIdQuery, Livre?>

    {
        private readonly BiblioDbContext _context;

        public GetLivreByIdQueryHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<Livre?> Handle(GetLivreByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Livres
            .Include(l => l.Auteur)
            .Include(l => l.Categorie)
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        }
    }
}