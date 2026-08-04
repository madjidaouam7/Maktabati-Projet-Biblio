using Biblio.Domain.Entities;

namespace Biblio.Application.Interfaces
{

    public class ResultatEmprunt
    {
        public bool Succes { get; set; }
        public string? Erreur { get; set; }
        public Emprunt? Emprunt { get; set; }
    }


    public interface IEmpruntService
    {
        Task<ResultatEmprunt> EmprunterLivreAsync(int livreId, string utilisateurId, DateTime dateRetourPrevue);

        Task<(bool Succes, string? Erreur)> RetournerLivreAsync (int empruntId);

        Task<List<Emprunt>> GetEmpruntsActifsAsync(string? utilisateurIdFiltre);

        Task<List<Emprunt>> GetHistoriqueAsync(string? utilisateurIdFiltre);

        Task<Emprunt?> GetEmpruntAsync(int id);
    }
}