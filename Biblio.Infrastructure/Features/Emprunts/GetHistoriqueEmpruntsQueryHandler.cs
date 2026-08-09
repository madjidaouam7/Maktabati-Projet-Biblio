using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Emprunts.Queries;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Emprunts
{
    public class GetHistoriqueEmpruntsQueryHandler : IRequestHandler<GetHistoriqueEmpruntsQuery, List<Emprunt>>
    {
        private readonly BiblioDbContext _context;

        public GetHistoriqueEmpruntsQueryHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<List<Emprunt>> Handle(GetHistoriqueEmpruntsQuery request, CancellationToken cancellationToken)
         {
            var query = _context.Emprunts
                .Include(e => e.Livre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.UtilisateurIdFiltre))
            {
                query = query.Where(e => e.UtilisateurId == request.UtilisateurIdFiltre);
            }

            query = query.OrderByDescending(e => e.DateEmprunt);
            
            return await query.ToListAsync(cancellationToken);
        }
    }
}