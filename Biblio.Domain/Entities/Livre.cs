namespace Biblio.Domain.Entities
{
    public class Livre
    {
        public int Id { get; private set; }
        public string Titre { get; private set; } = string.Empty;
        public string ISBN { get; private set; } = string.Empty;
        public DateTime DatePublication { get; private set; }
        public int NombrePages { get; private set; }

        // Clé étrangère + propriété de navigation vers Auteur
        public int AuteurId { get; private set; }
        public Auteur Auteur { get; private set; } = null!;

        // Clé étrangère + propriété de navigation vers Categorie
        public int CategorieId { get; private set; }
        public Categorie Categorie { get; private set; } = null!;
        public int NombreExemplaires { get; private set; }

        public ICollection<Emprunt> Emprunts { get; private set; } = new List<Emprunt>();
        public ICollection<Reservation> Reservations { get; private set; } = new List<Reservation>();

        private Livre() { }

        public Livre(string titre, string isbn, DateTime datePublication, int nombrePages,
            int nombreExemplaires, int auteurId, int categorieId)
        {
            if (datePublication > DateTime.Now)
            {
                throw new ArgumentException("La date de publication de publication ne peut pas etre dans le futur.");
            }
            if (nombreExemplaires < 1)
            {
                throw new ArgumentException("Le nombre d'exemplaires doit être au moins 1.");
            }

            Titre = titre;
            ISBN = isbn;
            DatePublication = datePublication;
            NombrePages = nombrePages;
            NombreExemplaires = nombreExemplaires;
            AuteurId = auteurId;
            CategorieId = categorieId;
        }

        public void Modifier(string titre, string isbn, DateTime datePublication,
            int nombrePages, int nombreExemplaires, int auteurId, int categorieId)
        {
            if (datePublication > DateTime.Now)
            {
                throw new ArgumentException("La date de publication ne peut pas etre dans le futur.");
            }

            if (nombreExemplaires < 1)
            {
                throw new ArgumentException("Le nombre d'exemplaires doit atre au moins 1.");
            }

            Titre = titre;
            ISBN = isbn;
            DatePublication = datePublication;
            NombrePages = nombrePages;
            NombreExemplaires = nombreExemplaires;
            AuteurId = auteurId;
            CategorieId = categorieId;
        }

        public bool EstDisponible(int nombreEmpruntsEnCours)
        {
            return nombreEmpruntsEnCours < NombreExemplaires;
        }
    }
}