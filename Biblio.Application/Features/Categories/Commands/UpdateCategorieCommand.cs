using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Categories.Commands
{
    public class UpdateCategorieCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}