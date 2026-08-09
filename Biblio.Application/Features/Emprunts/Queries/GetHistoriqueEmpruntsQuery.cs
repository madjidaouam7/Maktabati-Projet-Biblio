using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Emprunts.Queries
{
    public class GetHistoriqueEmpruntsQuery : IRequest<List<Emprunt>>
    {
        public string? UtilisateurIdFiltre { get; set; }
    }
}