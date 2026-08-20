using MediatR;
using Biblio.Application.Features.Reservations.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Reservations
{
    public class HonorerReservationCommandHandler : IRequestHandler<HonorerReservationCommand, (bool Succes, string? Erreur)>
    {
        private readonly BiblioDbContext _context;

        public HonorerReservationCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Succes, string? Erreur)> Handle(HonorerReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations.FindAsync(new object[] { request.ReservationId }, cancellationToken);
            if (reservation == null)
            {
                return (false, $"Aucune réservation trouvée avec l'id {request.ReservationId}.");
            }

            try
            {
                reservation.Honorer();
                await _context.SaveChangesAsync(cancellationToken);
                return (true, null);
            }
            catch (InvalidOperationException ex)
            {
                return (false, ex.Message);
            }
        }
    }
}