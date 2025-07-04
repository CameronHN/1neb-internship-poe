using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResponsibilitiesToContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExperienceResponsibility_Experiences_ExperienceId",
                table: "ExperienceResponsibility");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExperienceResponsibility",
                table: "ExperienceResponsibility");

            migrationBuilder.RenameTable(
                name: "ExperienceResponsibility",
                newName: "Responsibilities");

            migrationBuilder.RenameIndex(
                name: "IX_ExperienceResponsibility_ExperienceId",
                table: "Responsibilities",
                newName: "IX_Responsibilities_ExperienceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Responsibilities",
                table: "Responsibilities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Responsibilities_Experiences_ExperienceId",
                table: "Responsibilities",
                column: "ExperienceId",
                principalTable: "Experiences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responsibilities_Experiences_ExperienceId",
                table: "Responsibilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Responsibilities",
                table: "Responsibilities");

            migrationBuilder.RenameTable(
                name: "Responsibilities",
                newName: "ExperienceResponsibility");

            migrationBuilder.RenameIndex(
                name: "IX_Responsibilities_ExperienceId",
                table: "ExperienceResponsibility",
                newName: "IX_ExperienceResponsibility_ExperienceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExperienceResponsibility",
                table: "ExperienceResponsibility",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExperienceResponsibility_Experiences_ExperienceId",
                table: "ExperienceResponsibility",
                column: "ExperienceId",
                principalTable: "Experiences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
