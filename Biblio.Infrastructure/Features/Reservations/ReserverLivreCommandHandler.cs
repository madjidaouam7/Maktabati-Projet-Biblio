using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Reservations.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Reservations
{
    public class ReserverLivreCommandHandler : IRequestHandler<ReserverLivreCommand, ResultatReservation>
    {
        private readonly BiblioDbContext _context;

        public ReserverLivreCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<ResultatReservation> Handle(ReserverLivreCommand request, CancellationToken cancellationToken)
        {
            var livre = await _context.Livres.FindAsync(new object[] { request.LivreId }, cancellationToken);
            if (livre == null)
            {
                return new ResultatReservation { Succes = false, Erreur = $"Aucun livre trouvé avec l'id {request.LivreId}." };
            }

            var nombreEmpruntsEnCours = await _context.Emprunts
                .CountAsync(e => e.LivreId == request.LivreId && e.Etat == EtatEmprunt.EnCours, cancellationToken);

            if (livre.EstDisponible(nombreEmpruntsEnCours))
            {
                return new ResultatReservation { Succes = false, Erreur = $"Le livre '{livre.Titre}' a encore des exemplaires disponibles, la réservation n'est pas nécessaire." };
            }

            var reservationExistante = await _context.Reservations
                .AnyAsync(r => r.LivreId == request.LivreId && r.UtilisateurId == request.UtilisateurId && r.Etat == EtatReservation.EnAttente, cancellationToken);

            if (reservationExistante)
            {
                return new ResultatReservation { Succes = false, Erreur = "Vous avez déjà une réservation en attente pour ce livre." };
            }

            var reservation = new Reservation(request.LivreId, request.UtilisateurId, DateTime.Now);

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync(cancellationToken);

            return new ResultatReservation { Succes = true, Reservation = reservation, Livre = livre };
        }
    }
}