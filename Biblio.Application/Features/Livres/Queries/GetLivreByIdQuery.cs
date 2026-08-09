using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Livres.Queries
{
    public class GetLivreByIdQuery : IRequest<Livre?>
    {
        public int Id { get; set; }
    }
}