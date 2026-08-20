using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Reservations.Queries
{
    public class GetReservationsQuery : IRequest<List<Reservation>>
    {
        public string? UtilisateurIdFiltre { get; set; }
    }
}