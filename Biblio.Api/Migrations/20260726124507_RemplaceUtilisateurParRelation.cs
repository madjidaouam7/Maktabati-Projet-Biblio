using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblio.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemplaceUtilisateurParRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Utilisateur",
                table: "Emprunts",
                newName: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Emprunts_UtilisateurId",
                table: "Emprunts",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprunts_AspNetUsers_UtilisateurId",
                table: "Emprunts",
                column: "UtilisateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprunts_AspNetUsers_UtilisateurId",
                table: "Emprunts");

            migrationBuilder.DropIndex(
                name: "IX_Emprunts_UtilisateurId",
                table: "Emprunts");

            migrationBuilder.RenameColumn(
                name: "UtilisateurId",
                table: "Emprunts",
                newName: "Utilisateur");
        }
    }
}
