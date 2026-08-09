using MediatR;
using Microsoft.EntityFrameworkCore;
using Biblio.Domain.Entities;
using Biblio.Application.Features.Categories.Commands;
using Biblio.Infrastructure.Data;

namespace Biblio.Infrastructure.Features.Categories
{
    public class DeleteCategorieCommandHandler : IRequestHandler<DeleteCategorieCommand, bool>
    {
        private readonly BiblioDbContext _context;

        public DeleteCategorieCommandHandler(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCategorieCommand request, CancellationToken cancellationToken)
        {
            var categorie = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
           
            if (categorie == null)
            {
                return false;
            }

            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync(cancellationToken);
           
            return true;
        }
    }
}