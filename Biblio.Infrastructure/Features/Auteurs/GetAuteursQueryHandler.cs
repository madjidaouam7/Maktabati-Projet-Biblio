using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Auteurs.Queries;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Auteurs
{
    public class GetAuteursQueryHandler : IRequestHandler<GetAuteursQuery, (List<Auteur> Auteurs, int Total)>
    {
        private readonly BiblioDbContext _context;

        public GetAuteursQueryHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Auteur> Auteurs, int Total)> Handle(GetAuteursQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Auteurs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Recherche))
            {
                query = query.Where(a => a.Nom.Contains(request.Recherche));
            }

            bool decroissant = request.Ordre?.Trim().ToLower() == "desc";

            query = request.TrierPar?.Trim().ToLower() switch
            {
                "nom" => decroissant ? query.OrderByDescending(a => a.Nom) : query.OrderBy(a => a.Nom),
                "prenom" => decroissant ? query.OrderByDescending(a => a.Prenom) : query.OrderBy(a => a.Prenom),
                "datenaissance" => decroissant ? query.OrderByDescending(a => a.DateNaissance) : query.OrderBy(a => a.DateNaissance),
                _ => query.OrderBy(a => a.Id),
            };

            var total = await query.CountAsync(cancellationToken);

            var page = request.Page < 1 ? 1 : request.Page;
            var taillePage = request.TaillePage < 1 ? 10 : request.TaillePage;

            var auteurs = await query
                .Skip((page - 1) * taillePage)
                .Take(taillePage)
                .ToListAsync(cancellationToken);

            return (auteurs, total);
        }
    }
}