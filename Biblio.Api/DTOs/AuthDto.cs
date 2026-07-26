using System.ComponentModel.DataAnnotations;

namespace Biblio.Api.DTOs
{

    public class RegisterDto
    {
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        public string MotDePasse { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        public string Prenom { get; set; } = string.Empty;
    }



    public class LoginDto
    {
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        public string MotDePasse { get; set; } = string.Empty;
    }



    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }




    public class CreerBibliothecaireDto
    {
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        public string MotDePasse { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        public string Prenom { get; set; } = string.Empty;
    }
}