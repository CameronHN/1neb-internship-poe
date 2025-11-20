using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameContactEntityAddContactType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactUrl",
                table: "Contact",
                newName: "LinkType");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Contact",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Link",
                table: "Contact");

            migrationBuilder.RenameColumn(
                name: "LinkType",
                table: "Contact",
                newName: "ContactUrl");
        }
    }
}
