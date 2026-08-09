using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<(List<Categorie> Categories, int Total)>
    {
        public string? Recherche { get; set; }
        public string? TrierPar { get; set; }
        public string? Ordre { get; set; }
        public int Page { get; set; } = 1;
        public int TaillePage { get; set; } = 10;
    }
}