using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Reservations.Commands
{
    public class ResultatReservation
    {
        public bool Succes { get; set; }
        public string? Erreur { get; set; }
        public Reservation? Reservation { get; set; }
        public Livre? Livre { get; set; }
    }

    public class ReserverLivreCommand : IRequest<ResultatReservation>
    {
        public int LivreId { get; set; }
        public string UtilisateurId { get; set; } = string.Empty;
    }
}