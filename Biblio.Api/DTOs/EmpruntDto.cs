using System.ComponentModel.DataAnnotations;

namespace Biblio.Api.DTOs
{
    // Ce que l'API renvoie au client
    public class EmpruntDto
    {
        public int Id { get; set; }
        public int LivreId { get; set; }
        public string LivreTitre { get; set; } = string.Empty;
        public string Utilisateur { get; set; } = string.Empty;
        public DateTime DateEmprunt { get; set; }
        public DateTime DateRetourPrevue { get; set; }
        public DateTime? DateRetourEffective { get; set; }
        public string Etat { get; set; } = string.Empty;
    }

    // Ce que le client envoie pour créer un emprunt
    public class CreateEmpruntDto
    {
        [Required(ErrorMessage = "Le livre est obligatoire.")]
        public int LivreId { get; set; }

        [Required(ErrorMessage = "Le nom de l'utilisateur est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Utilisateur { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de retour prévue est obligatoire.")]
        public DateTime DateRetourPrevue { get; set; }
    }

}
