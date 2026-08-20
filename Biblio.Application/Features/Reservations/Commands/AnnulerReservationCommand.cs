using MediatR;

namespace Biblio.Application.Features.Reservations.Commands
{
    public class AnnulerReservationCommand : IRequest<(bool Succes, string? Erreur)>
    {
        public int ReservationId { get; set; }
    }
}