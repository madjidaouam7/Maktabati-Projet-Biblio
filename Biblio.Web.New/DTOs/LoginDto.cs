
using System.ComponentModel.DataAnnotations;

namespace Biblio.Web.New.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "L'email est obligatoire.")]
    [EmailAddress(ErrorMessage = "Format d'email invalide.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
    [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
    public string MotDePasse { get; set; } = string.Empty;
}
