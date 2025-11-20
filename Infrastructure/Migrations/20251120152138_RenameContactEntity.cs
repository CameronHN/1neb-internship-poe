using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameContactEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_AspNetUsers_UserId",
                table: "Contact");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contact",
                table: "Contact");

            migrationBuilder.RenameTable(
                name: "Contact",
                newName: "ProfessionalLink");

            migrationBuilder.RenameIndex(
                name: "IX_Contact_UserId",
                table: "ProfessionalLink",
                newName: "IX_ProfessionalLink_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfessionalLink",
                table: "ProfessionalLink",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalLink_AspNetUsers_UserId",
                table: "ProfessionalLink",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalLink_AspNetUsers_UserId",
                table: "ProfessionalLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfessionalLink",
                table: "ProfessionalLink");

            migrationBuilder.RenameTable(
                name: "ProfessionalLink",
                newName: "Contact");

            migrationBuilder.RenameIndex(
                name: "IX_ProfessionalLink_UserId",
                table: "Contact",
                newName: "IX_Contact_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contact",
                table: "Contact",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_AspNetUsers_UserId",
                table: "Contact",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
