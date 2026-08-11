using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Emprunts.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Emprunts
{
    public class EmprunterLivreCommandHandler : IRequestHandler<EmprunterLivreCommand, ResultatEmprunt>
    {
        private readonly BiblioDbContext _context;

        public EmprunterLivreCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<ResultatEmprunt> Handle(EmprunterLivreCommand request, CancellationToken cancellationToken)
        {
            var maintenant = DateTime.Now;

            if (request.DateRetourPrevue < maintenant)
            {
                return new ResultatEmprunt { Succes = false, Erreur = "La date de retour prévue doit être supérieure ou égale à la date d'emprunt." };
            }

            var livre = await _context.Livres.FindAsync(request.LivreId);
            if (livre == null)
            {
                return new ResultatEmprunt { Succes = false, Erreur = $"Aucun livre trouvé avec l'id {request.LivreId}." };
            }

            var nombreEmpruntsEnCours = await _context.Emprunts
                .CountAsync(e => e.LivreId == request.LivreId && e.Etat == EtatEmprunt.EnCours, cancellationToken);

            if (!livre.EstDisponible(nombreEmpruntsEnCours))
            {
                return new ResultatEmprunt { Succes = false, Erreur = $"Tous les exemplaires du livre '{livre.Titre}' sont déjà empruntés." };
            }

            var emprunt = new Emprunt(
                request.LivreId,
                request.UtilisateurId,
                maintenant,
                request.DateRetourPrevue);


            _context.Emprunts.Add(emprunt);
            await _context.SaveChangesAsync(cancellationToken);

            return new ResultatEmprunt { Succes = true, Emprunt = emprunt, Livre = livre };
        }
    }
}