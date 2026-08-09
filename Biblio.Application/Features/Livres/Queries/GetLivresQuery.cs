using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Livres.Queries
{
    public class GetLivresQuery : IRequest<(List<Livre> Livres, int Total)>
    {
        public string? Recherche { get; set; }
        public string? TrierPar { get; set; }
        public string? Ordre { get; set; }
        public int Page { get; set; } = 1;
        public int TaillePage { get; set; } = 10;
    }
}