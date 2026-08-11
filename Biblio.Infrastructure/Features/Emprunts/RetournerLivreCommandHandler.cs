using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Emprunts.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Emprunts
{
    public class RetournerLivreCommandHandler : IRequestHandler<RetournerLivreCommand, ResultatEmprunt>
    {
        private readonly BiblioDbContext _context;

        public RetournerLivreCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<ResultatEmprunt> Handle(RetournerLivreCommand request, CancellationToken cancellationToken)
        {
            var emprunt = await _context.Emprunts.FindAsync(request.Id);
            if (emprunt == null)
            {
                return new ResultatEmprunt { Succes = false, Erreur = $"Aucun emprunt trouvé avec l'id {request.Id}." };
            }

            if (emprunt.Etat == EtatEmprunt.Retourne)
            {
                return new ResultatEmprunt { Succes = false, Erreur = "Ce livre a déjà été retourné." };
            }

            emprunt.Retourner();

            await _context.SaveChangesAsync(cancellationToken);
            return new ResultatEmprunt { Succes = true, Emprunt = emprunt };
        }
    }
}