using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Livres.Queries;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Livres
{
    public class GetLivresQueryHandler : IRequestHandler<GetLivresQuery, (List<Livre> Livres, int Total)>
    {
        private readonly BiblioDbContext _context;

        public GetLivresQueryHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Livre> Livres, int Total)> Handle(GetLivresQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Livres
                .Include(l => l.Auteur)
                .Include(l => l.Categorie)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Recherche))
            {
                query = query.Where(l => l.Titre.Contains(request.Recherche));
            }

            bool decroissant = request.Ordre?.Trim().ToLower() == "desc";

            query = request.TrierPar?.Trim().ToLower() switch
            {
                "titre" => decroissant ? query.OrderByDescending(l => l.Titre) : query.OrderBy(l => l.Titre),
                "datepublication" => decroissant ? query.OrderByDescending(l => l.DatePublication) : query.OrderBy(l => l.DatePublication),
                "nombrepages" => decroissant ? query.OrderByDescending(l => l.NombrePages) : query.OrderBy(l => l.NombrePages),
                _ => query.OrderBy(l => l.Id),
            };

            var total = await query.CountAsync(cancellationToken);

            var page = request.Page < 1 ? 1 : request.Page;
            var taillePage = request.TaillePage < 1 ? 10 : request.TaillePage;

            var livres = await query
                .Skip((page - 1) * taillePage)
                .Take(taillePage)
                .ToListAsync(cancellationToken);

            return (livres, total);
        }
    }
}