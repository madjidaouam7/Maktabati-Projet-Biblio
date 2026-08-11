namespace Biblio.Domain.Entities
{
    public class Emprunt
    {
        public int Id { get; private set; }

        public int LivreId { get; private set; }
        public Livre Livre { get; private set; } = null!;

        public string UtilisateurId { get; private set; } = string.Empty;

        public DateTime DateEmprunt { get; private set; }
        public DateTime DateRetourPrevue { get; private set; }
        public DateTime? DateRetourEffective { get; private set; }

        public EtatEmprunt Etat { get; private set; } = EtatEmprunt.EnCours;

        private Emprunt() { } // constructeur privé, nécessaire pour EF Core

        public Emprunt(int livreId, string utilisateurId, DateTime dateEmprunt, DateTime dateRetourPrevue)
        {
            if (dateRetourPrevue < dateEmprunt)
            {
                throw new ArgumentException("La date de retour prévue doit être supérieure ou égale à la date d'emprunt.");
            }

            LivreId = livreId;
            UtilisateurId = utilisateurId;
            DateEmprunt = dateEmprunt;
            DateRetourPrevue = dateRetourPrevue;
            DateRetourEffective = null;
            Etat = EtatEmprunt.EnCours;
        }

        public void Retourner()
        {
            if (Etat == EtatEmprunt.Retourne)
            {
                throw new InvalidOperationException("Ce livre a déjà été retourné.");
            }

            DateRetourEffective = DateTime.Now;
            Etat = EtatEmprunt.Retourne;
        }
    }
}