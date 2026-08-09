using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Auteurs.Queries;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Auteurs
{
    public class GetAuteurByIdQueryHandler : IRequestHandler<GetAuteursByIdQuery, Auteur?>
    {
        private readonly BiblioDbContext _context;

        public GetAuteurByIdQueryHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<Auteur?> Handle(GetAuteursByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Auteurs.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        }
    }
}