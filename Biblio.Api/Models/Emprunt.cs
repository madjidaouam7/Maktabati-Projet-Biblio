namespace Biblio.Api.Models
{
    public class Emprunt
    {
        public int Id { get; set; }

        public int LivreId { get; set; }
        public Livre Livre { get; set; } = null!;

        public string UtilisateurId { get; set; } = string.Empty;
        public Utilisateur Utilisateur { get; set; } = null!;

        public DateTime DateEmprunt { get; set; }
        public DateTime DateRetourPrevue { get; set; }
        public DateTime? DateRetourEffective { get; set; }

        public EtatEmprunt Etat { get; set; } = EtatEmprunt.EnCours;
    }
}