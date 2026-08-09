using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Categories.Commands
{
    public class DeleteCategorieCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}