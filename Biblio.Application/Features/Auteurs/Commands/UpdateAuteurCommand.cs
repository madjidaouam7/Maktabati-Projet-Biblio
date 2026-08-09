using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Auteurs.Commands
{
    public class UpdateAuteurCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public DateTime? DateNaissance { get; set; }
    }
}