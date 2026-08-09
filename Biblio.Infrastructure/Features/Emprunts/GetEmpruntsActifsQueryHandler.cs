using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Emprunts.Queries;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Emprunts
{
    public class GetEmpruntsActifsQueryHandler : IRequestHandler<GetEmpruntsActifsQuery, List<Emprunt>>
    {
        private readonly BiblioDbContext _context;

        public GetEmpruntsActifsQueryHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<List<Emprunt>> Handle(GetEmpruntsActifsQuery request, CancellationToken cancellationToken)
         {
            var query = _context.Emprunts
                .Include(e => e.Livre)
                .Where(e => e.Etat == EtatEmprunt.EnCours)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.UtilisateurIdFiltre))
            {
                query = query.Where(e => e.UtilisateurId == request.UtilisateurIdFiltre);
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}