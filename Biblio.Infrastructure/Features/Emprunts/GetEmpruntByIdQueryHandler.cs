using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Emprunts.Queries;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Emprunts
{
    public class GetEmpruntByIdQueryHandler : IRequestHandler<GetEmpruntByIdQuery, Emprunt?>
    {
        private readonly BiblioDbContext _context;

        public GetEmpruntByIdQueryHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<Emprunt?> Handle(GetEmpruntByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Emprunts
            .Include(e => e.Livre)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        }
    }
}