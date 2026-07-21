using System.ComponentModel.DataAnnotations;

namespace Biblio.Api.DTOs
{
    // Ce que l'API renvoie au client
    public class LivreDto
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public DateTime DatePublication { get; set; }
        public int NombrePages { get; set; }

        public int AuteurId { get; set; }
        public string AuteurNomComplet { get; set; } = string.Empty;

        public int CategorieId { get; set; }
        public string CategorieNom { get; set; } = string.Empty;
    }

    // Ce que le client envoie pour créer un livre
    public class CreateLivreDto
    {
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        [MaxLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        public string Titre { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'ISBN est obligatoire.")]
        [MaxLength(20, ErrorMessage = "L'ISBN ne peut pas dépasser 20 caractères.")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de publication est obligatoire.")]
        public DateTime DatePublication { get; set; }

        [Range(1, 10000, ErrorMessage = "Le nombre de pages doit être compris entre 1 et 10000.")]
        public int NombrePages { get; set; }

        [Required(ErrorMessage = "L'auteur est obligatoire.")]
        public int AuteurId { get; set; }

        [Required(ErrorMessage = "La catégorie est obligatoire.")]
        public int CategorieId { get; set; }
    }

    // Ce que le client envoie pour modifier un livre
    public class UpdateLivreDto
    {
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        [MaxLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        public string Titre { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'ISBN est obligatoire.")]
        [MaxLength(20, ErrorMessage = "L'ISBN ne peut pas dépasser 20 caractères.")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de publication est obligatoire.")]
        public DateTime DatePublication { get; set; }

        [Range(1, 10000, ErrorMessage = "Le nombre de pages doit être compris entre 1 et 10000.")]
        public int NombrePages { get; set; }

        [Required(ErrorMessage = "L'auteur est obligatoire.")]
        public int AuteurId { get; set; }

        [Required(ErrorMessage = "La catégorie est obligatoire.")]
        public int CategorieId { get; set; }
    }
}