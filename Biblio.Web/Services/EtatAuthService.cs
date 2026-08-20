namespace Biblio.Web.Services
{
    public class EtatAuthService
    {
        public string? Token { get; private set; }
        public string? Email { get; private set; }
        public List<string> Roles { get; private set; } = new List<string>();
        public bool EstConnecte => !string.IsNullOrEmpty(Token);

        public event Action? OnChange;

        public void Connecter(string token, string email, List<string> roles)
        {
            Token = token;
            Email = email;
            Roles = roles;
            NotifierChangement();
        }

        public void Deconnecter()
        {
            Token = null;
            Email = null;
            Roles = new List<string>();
            NotifierChangement();
        }

        private void NotifierChangement() => OnChange?.Invoke();
    }
}