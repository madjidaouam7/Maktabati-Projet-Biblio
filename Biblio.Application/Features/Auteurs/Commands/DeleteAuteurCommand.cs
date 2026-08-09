using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Auteurs.Commands
{
    public class DeleteAuteurCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}