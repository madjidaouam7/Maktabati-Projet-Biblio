using Biblio.Domain.Entities;

namespace Biblio.Application.Features.Emprunts.Commands
{
    public class ResultatEmprunt
    {
        public bool Succes { get; set; }
        public string? Erreur { get; set; }
        public Emprunt? Emprunt { get; set; }
        public Livre? Livre { get; set; }
    }
}