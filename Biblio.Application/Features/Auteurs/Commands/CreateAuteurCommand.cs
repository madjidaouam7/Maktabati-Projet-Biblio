using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Auteurs.Commands
{
    public class CreateAuteurCommand : IRequest<Auteur>
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public DateTime? DateNaissance { get; set; }
    }
}