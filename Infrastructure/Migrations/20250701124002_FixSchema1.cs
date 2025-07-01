using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSchema1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "varchar(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(12)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "varchar(12)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)");
        }
    }
}
