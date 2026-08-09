using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Livres.Commands
{
    public class DeleteLivreCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}