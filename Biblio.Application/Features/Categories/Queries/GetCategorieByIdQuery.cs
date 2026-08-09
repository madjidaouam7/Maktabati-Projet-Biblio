using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Categories.Queries
{
    public class GetCategorieByIdQuery : IRequest<Categorie?>
    {
        public int Id { get; set; }
    }
}