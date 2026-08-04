using Microsoft.AspNetCore.Identity;

namespace Biblio.Infrastructure.Identity
{
    public class Utilisateur : IdentityUser
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
    }
}