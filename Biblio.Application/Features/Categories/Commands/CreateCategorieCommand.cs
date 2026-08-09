using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Categories.Commands
{
    public class CreateCategorieCommand : IRequest<Categorie>
    {
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}