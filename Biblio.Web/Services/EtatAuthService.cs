namespace Biblio.Web.Services
{
    public class EtatAuthService
    {
        public string? Token { get; private set; }
        public string? Email { get; private set; }
        public List<string> Roles { get; private set; } = new List<string>();
        public bool EstConnecte => !string.IsNullOrEmpty(Token);

        public void Connecter(string token, string email, List<string> roles)
        {
            Token = token;
            Email = email;
            Roles = roles;
        }

        public void Deconnecter()
        {
            Token = null;
            Email = null;
            Roles = new List<string>();
        }
    }
}