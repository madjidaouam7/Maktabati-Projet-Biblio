using MediatR;
using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Livres.Commands
{
    public class UpdateLivreCommand : IRequest<bool>
    {
        public int Id { get; set;}
        public string Titre { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public DateTime DatePublication { get; set; }
        public int NombrePages { get; set; }
        public int AuteurId { get; set; }
        public int CategorieId { get; set; }
        public int NombreExemplaires { get; set; }
    }
}