namespace Biblio.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; private set; }

        public int LivreId { get; private set; }
        public Livre Livre { get; private set; } = null!;

        public string UtilisateurId { get; private set; } = string.Empty;

        public DateTime DateReservation { get; private set; }

        public EtatReservation Etat { get; private set; } = EtatReservation.EnAttente;

        private Reservation() { }

        public Reservation(int livreId, string utilisateurId, DateTime dateReservation)
        {
            LivreId = livreId;
            UtilisateurId = utilisateurId;
            DateReservation = dateReservation;
            Etat = EtatReservation.EnAttente;
        }

        public void Honorer()
        {
            if (Etat != EtatReservation.EnAttente)
            {
                throw new InvalidOperationException("Cette réservation n'est plus en attente.");
            }
            Etat = EtatReservation.Honoree;
        }

        public void Annuler()
        {
            if (Etat != EtatReservation.EnAttente)
            {
                throw new InvalidOperationException("Cette réservation ne peut plus être annulée.");
            }
            Etat = EtatReservation.Annulee;
        }
    }
}