namespace Biblio.Domain.Entities
{
    public class Categorie
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Propriété de navigation : une catégorie a plusieurs livres
        public ICollection<Livre> Livres { get; set; } = new List<Livre>();
    }
}