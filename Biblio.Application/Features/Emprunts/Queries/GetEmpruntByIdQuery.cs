using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Emprunts.Queries
{
    public class GetEmpruntByIdQuery : IRequest<Emprunt?>
    {
        public int Id { get; set; }
    }
}   