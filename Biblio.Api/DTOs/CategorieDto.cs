using System.ComponentModel.DataAnnotations;

namespace Biblio.Api.DTOs
{
    // Ce que l'API renvoie au client
    public class CategorieDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    // Ce que le client envoie pour créer une catégorie
    public class CreateCategorieDto
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Nom { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string? Description { get; set; }
    }

    // Ce que le client envoie pour modifier une catégorie
    public class UpdateCategorieDto
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Nom { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string? Description { get; set; }
    }
}