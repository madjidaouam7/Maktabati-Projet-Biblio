namespace Biblio.Api.Models
{
    public class Livre
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public DateTime DatePublication { get; set; }
        public int NombrePages { get; set; }

        // Clé étrangère + propriété de navigation vers Auteur
        public int AuteurId { get; set; }
        public Auteur Auteur { get; set; } = null!;

        // Clé étrangère + propriété de navigation vers Categorie
        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; } = null!;
        public int NombreExemplaires { get; set; } 

        public ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();
    }
}