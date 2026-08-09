using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Auteurs.Queries
{
    public class GetAuteursByIdQuery : IRequest<Auteur?>
    {
        public int Id { get; set; }
    }
}