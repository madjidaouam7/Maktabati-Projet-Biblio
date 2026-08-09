using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Emprunts.Commands
{
    public class EmprunterLivreCommand : IRequest<ResultatEmprunt>
    {
        public int LivreId { get; set; }
        public DateTime DateRetourPrevue { get; set; }
        public string UtilisateurId { get; set; } = string.Empty;
    }
}