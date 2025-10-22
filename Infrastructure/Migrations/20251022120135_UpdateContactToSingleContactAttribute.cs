using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContactToSingleContactAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitHub",
                table: "Contact");

            migrationBuilder.RenameColumn(
                name: "LinkedIn",
                table: "Contact",
                newName: "ContactUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactUrl",
                table: "Contact",
                newName: "LinkedIn");

            migrationBuilder.AddColumn<string>(
                name: "GitHub",
                table: "Contact",
                type: "varchar(100)",
                nullable: true);
        }
    }
}
