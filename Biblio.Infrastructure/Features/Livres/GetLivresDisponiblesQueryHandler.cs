using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Livres.Queries;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Livres
{
    public class GetLivresDisponiblesQueryHandler : IRequestHandler<GetLivresDisponiblesQuery, List<LivreDisponible>>
    {
        private readonly BiblioDbContext _context;

        public GetLivresDisponiblesQueryHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<List<LivreDisponible>> Handle(GetLivresDisponiblesQuery request, CancellationToken cancellationToken)
        {
            var livres = await _context.Livres
                .Include(l => l.Auteur)
                .Include(l => l.Emprunts)
                .ToListAsync(cancellationToken);

            var resultat = livres
                .Select(l => new LivreDisponible
                {
                    Livre = l,
                    NombreExemplairesDisponibles = l.NombreExemplaires - l.Emprunts.Count(e => e.Etat == EtatEmprunt.EnCours)
                })
                .Where(x => x.NombreExemplairesDisponibles > 0)
                .ToList();

            return resultat;
        }
    }
}