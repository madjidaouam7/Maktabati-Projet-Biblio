using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Categories.Queries;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Categories
{
    public class GetCategorieByIdQueryHandler : IRequestHandler<GetCategorieByIdQuery, Categorie?>
    {
        private readonly BiblioDbContext _context;

        public GetCategorieByIdQueryHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<Categorie?> Handle(GetCategorieByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        }
    }
}