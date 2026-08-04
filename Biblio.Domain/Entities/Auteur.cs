namespace Biblio.Domain.Entities
{
    public class Auteur
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public DateTime? DateNaissance { get; set; }

        // Propriété de navigation : un auteur a plusieurs livres
        public ICollection<Livre> Livres { get; set; } = new List<Livre>();
    }
}