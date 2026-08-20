using MediatR;
using Biblio.Application.Features.Reservations.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Reservations
{
    public class AnnulerReservationCommandHandler : IRequestHandler<AnnulerReservationCommand, (bool Succes, string? Erreur)>
    {
        private readonly BiblioDbContext _context;

        public AnnulerReservationCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Succes, string? Erreur)> Handle(AnnulerReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations.FindAsync(new object[] { request.ReservationId }, cancellationToken);
            if (reservation == null)
            {
                return (false, $"Aucune réservation trouvée avec l'id {request.ReservationId}.");
            }

            try
            {
                reservation.Annuler();
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