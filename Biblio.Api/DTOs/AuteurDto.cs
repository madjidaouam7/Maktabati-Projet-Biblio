using System.ComponentModel.DataAnnotations;

namespace Biblio.Api.DTOs
{
    // Ce que l'API renvoie au client
    public class AuteurDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public DateTime? DateNaissance { get; set; }
    }

    // Ce que le client envoie pour créer un auteur
    public class CreateAuteurDto
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères.")]
        public string Prenom { get; set; } = string.Empty;

        [DataType(DataType.Date, ErrorMessage = "La date de naissance n'est pas valide.")]
        public DateTime? DateNaissance { get; set; }
    }

    // Ce que le client envoie pour modifier un auteur
    public class UpdateAuteurDto
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères.")]
        public string Prenom { get; set; } = string.Empty;

        [DataType(DataType.Date, ErrorMessage = "La date de naissance n'est pas valide.")]
        public DateTime? DateNaissance { get; set; }
    }
}
