using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_And_Add_Types_To_Certification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Responsibilities",
                table: "Experiences");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Skills",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ResumeId",
                table: "ResumeTemplates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TemplateThumbnailPath",
                table: "ResumeTemplates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Resumes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "ProfessionalSummaries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Experiences",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Educations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Contacts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "IssuingOrganisation",
                table: "Certifications",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CredentialUrl",
                table: "Certifications",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CertificationName",
                table: "Certifications",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Certifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "Certifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IssuedDate",
                table: "Certifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Certifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdaterId",
                table: "Certifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Certifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ExperienceResponsibility",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExperienceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Resonsibility = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceResponsibility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperienceResponsibility_Experiences_ExperienceId",
                        column: x => x.ExperienceId,
                        principalTable: "Experiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skills_UserId1",
                table: "Skills",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeTemplates_ResumeId",
                table: "ResumeTemplates",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_UserId1",
                table: "Resumes",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalSummaries_UserId1",
                table: "ProfessionalSummaries",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_UserId1",
                table: "Experiences",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_UserId1",
                table: "Educations",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId1",
                table: "Contacts",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Certifications_UserId1",
                table: "Certifications",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceResponsibility_ExperienceId",
                table: "ExperienceResponsibility",
                column: "ExperienceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_Users_UserId1",
                table: "Certifications",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Users_UserId1",
                table: "Contacts",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Users_UserId1",
                table: "Educations",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Users_UserId1",
                table: "Experiences",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalSummaries_Users_UserId1",
                table: "ProfessionalSummaries",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_Users_UserId1",
                table: "Resumes",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResumeTemplates_Resumes_ResumeId",
                table: "ResumeTemplates",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Users_UserId1",
                table: "Skills",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_Users_UserId1",
                table: "Certifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Users_UserId1",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_Users_UserId1",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Users_UserId1",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalSummaries_Users_UserId1",
                table: "ProfessionalSummaries");

            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_Users_UserId1",
                table: "Resumes");

            migrationBuilder.DropForeignKey(
                name: "FK_ResumeTemplates_Resumes_ResumeId",
                table: "ResumeTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Users_UserId1",
                table: "Skills");

            migrationBuilder.DropTable(
                name: "ExperienceResponsibility");

            migrationBuilder.DropIndex(
                name: "IX_Skills_UserId1",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_ResumeTemplates_ResumeId",
                table: "ResumeTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Resumes_UserId1",
                table: "Resumes");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalSummaries_UserId1",
                table: "ProfessionalSummaries");

            migrationBuilder.DropIndex(
                name: "IX_Experiences_UserId1",
                table: "Experiences");

            migrationBuilder.DropIndex(
                name: "IX_Educations_UserId1",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_UserId1",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Certifications_UserId1",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "ResumeId",
                table: "ResumeTemplates");

            migrationBuilder.DropColumn(
                name: "TemplateThumbnailPath",
                table: "ResumeTemplates");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ProfessionalSummaries");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "IssuedDate",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "UpdaterId",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Certifications");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Responsibilities",
                table: "Experiences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "IssuingOrganisation",
                table: "Certifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "CredentialUrl",
                table: "Certifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CertificationName",
                table: "Certifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");
        }
    }
}
