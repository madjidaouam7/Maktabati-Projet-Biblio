using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Categories.Queries;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Categories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, (List<Categorie> Categories, int Total)>
    {
        private readonly BiblioDbContext _context;

        public GetCategoriesQueryHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Categorie> Categories, int Total)> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Recherche))
            {
                query = query.Where(c => c.Nom.Contains(request.Recherche));
            }

            bool decroissant = request.Ordre?.Trim().ToLower() == "desc";

            query = request.TrierPar?.Trim().ToLower() switch
            {
                "nom" => decroissant ? query.OrderByDescending(c => c.Nom) : query.OrderBy(c => c.Nom),
                "description" => decroissant ? query.OrderByDescending(c => c.Description) : query.OrderBy(c => c.Description),
                _ => query.OrderBy(c => c.Id)
            };

            var total = await query.CountAsync(cancellationToken);

            var page = request.Page < 1 ? 1 : request.Page;
            var taillePage = request.TaillePage < 1 ? 10 : request.TaillePage;

            var categories = await query
                .Skip((page - 1) * taillePage)
                .Take(taillePage)
                .ToListAsync(cancellationToken);

            return (categories, total);
        }
    }
}