using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Biblio.Api.Models;

namespace Biblio.Api.Data
{
    public class BiblioDbContext : IdentityDbContext<Utilisateur>
    {
        public BiblioDbContext(DbContextOptions<BiblioDbContext> options)
            : base(options)
        {
        }

        public DbSet<Livre> Livres { get; set; }
        public DbSet<Auteur> Auteurs { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Emprunt> Emprunts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Contrainte d'unicité sur l'ISBN
            modelBuilder.Entity<Livre>()
                .HasIndex(l => l.ISBN)
                .IsUnique();

            // Relation Livre -> Auteur (plusieurs livres pour un auteur)
            modelBuilder.Entity<Livre>()
                .HasOne(l => l.Auteur)
                .WithMany(a => a.Livres)
                .HasForeignKey(l => l.AuteurId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Livre -> Categorie (plusieurs livres pour une catégorie)
            modelBuilder.Entity<Livre>()
                .HasOne(l => l.Categorie)
                .WithMany(c => c.Livres)
                .HasForeignKey(l => l.CategorieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Emprunt -> Livre (un livre peut avoir plusieurs emprunts)
            modelBuilder.Entity<Emprunt>()
                .HasOne(e => e.Livre)
                .WithMany(l => l.Emprunts)
                .HasForeignKey(e => e.LivreId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Emprunt -> Utilisateur (un utilisateur peut avoir plusieurs emprunts)
            modelBuilder.Entity<Emprunt>()
                .HasOne(e => e.Utilisateur)
                .WithMany()
                .HasForeignKey(e => e.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}