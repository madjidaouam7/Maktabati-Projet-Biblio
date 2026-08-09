using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Categories.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Categories
{
    public class UpdateCategorieCommandHandler : IRequestHandler<UpdateCategorieCommand, bool>
    {
        private readonly BiblioDbContext _context;

        public UpdateCategorieCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateCategorieCommand request, CancellationToken cancellationToken)
        {
            var categorie = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken); //new object[] { request.Id } a la place de request id pour update et delete

            if (categorie == null)
            {
                return false;
            }

            categorie.Nom = request.Nom;
            categorie.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}