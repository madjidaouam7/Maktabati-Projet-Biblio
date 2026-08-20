using MediatR;

namespace Biblio.Application.Features.Reservations.Commands
{
    public class HonorerReservationCommand : IRequest<(bool Succes, string? Erreur)>
    {
        public int ReservationId { get; set; }
    }
}