namespace Biblio.Api.DTOs
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public int LivreId { get; set; }
        public string LivreTitre { get; set; } = string.Empty;
        public string Utilisateur { get; set; } = string.Empty;
        public DateTime DateReservation { get; set; }
        public string Etat { get; set; } = string.Empty;
    }

    public class CreateReservationDto
    {
        public int LivreId { get; set; }
    }
}