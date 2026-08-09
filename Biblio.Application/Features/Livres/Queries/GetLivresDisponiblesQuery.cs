using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Livres.Queries
{
    public class LivreDisponible
    {
        public Livre Livre { get; set; } = null!;
        public int NombreExemplairesDisponibles { get; set; }
    }

    public class GetLivresDisponiblesQuery : IRequest<List<LivreDisponible>>
    {
    }
}