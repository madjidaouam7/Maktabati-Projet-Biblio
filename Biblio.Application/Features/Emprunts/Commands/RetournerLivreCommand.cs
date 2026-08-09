using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Emprunts.Commands
{
    public class RetournerLivreCommand : IRequest<ResultatEmprunt>
    {
        public int Id { get; set; }

    }
}