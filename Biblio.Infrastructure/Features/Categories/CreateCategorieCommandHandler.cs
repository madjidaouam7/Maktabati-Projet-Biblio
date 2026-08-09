using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Categories.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Categories
{
    public class CreateCategorieCommandHandler : IRequestHandler<CreateCategorieCommand, Categorie>
    {
        private readonly BiblioDbContext _context;

        public CreateCategorieCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<Categorie> Handle(CreateCategorieCommand request, CancellationToken cancellationToken)
        {
            var categorie = new Categorie
            {
                Nom = request.Nom,
                Description = request.Description
            };

            _context.Categories.Add(categorie);
            await _context.SaveChangesAsync(cancellationToken);

            return categorie;
        }
    }
}