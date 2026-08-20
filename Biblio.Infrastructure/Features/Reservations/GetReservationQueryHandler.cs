using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Reservations.Queries;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Reservations
{
    public class GetReservationsQueryHandler : IRequestHandler<GetReservationsQuery, List<Reservation>>
    {
        private readonly BiblioDbContext _context;

        public GetReservationsQueryHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reservation>> Handle(GetReservationsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Reservations
                .Include(r => r.Livre)
                .AsQueryable();

            if (request.UtilisateurIdFiltre != null)
            {
                query = query.Where(r => r.UtilisateurId == request.UtilisateurIdFiltre);
            }

            return await query.OrderByDescending(r => r.DateReservation).ToListAsync(cancellationToken);
        }
    }
}