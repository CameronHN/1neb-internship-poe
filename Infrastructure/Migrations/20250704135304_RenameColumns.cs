using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_Users_UserId",
                table: "Certifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Users_UserId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_Users_UserId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Users_UserId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalSummaries_Users_UserId",
                table: "ProfessionalSummaries");

            migrationBuilder.DropForeignKey(
                name: "FK_Responsibilities_Experiences_ExperienceId",
                table: "Responsibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_Users_UserId",
                table: "Resumes");

            migrationBuilder.DropForeignKey(
                name: "FK_ResumeTemplates_Resumes_ResumeId",
                table: "ResumeTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Users_UserId",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skills",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResumeTemplates",
                table: "ResumeTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resumes",
                table: "Resumes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Responsibilities",
                table: "Responsibilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfessionalSummaries",
                table: "ProfessionalSummaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Experiences",
                table: "Experiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Educations",
                table: "Educations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certifications",
                table: "Certifications");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "Skills",
                newName: "Skill");

            migrationBuilder.RenameTable(
                name: "ResumeTemplates",
                newName: "ResumeTemplate");

            migrationBuilder.RenameTable(
                name: "Resumes",
                newName: "Resume");

            migrationBuilder.RenameTable(
                name: "Responsibilities",
                newName: "ExperienceResponsibility");

            migrationBuilder.RenameTable(
                name: "ProfessionalSummaries",
                newName: "ProfessionalSummary");

            migrationBuilder.RenameTable(
                name: "Experiences",
                newName: "Experience");

            migrationBuilder.RenameTable(
                name: "Educations",
                newName: "Education");

            migrationBuilder.RenameTable(
                name: "Contacts",
                newName: "Contact");

            migrationBuilder.RenameTable(
                name: "Certifications",
                newName: "Certification");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_UserId",
                table: "Skill",
                newName: "IX_Skill_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ResumeTemplates_ResumeId",
                table: "ResumeTemplate",
                newName: "IX_ResumeTemplate_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Resumes_UserId",
                table: "Resume",
                newName: "IX_Resume_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Responsibilities_ExperienceId",
                table: "ExperienceResponsibility",
                newName: "IX_ExperienceResponsibility_ExperienceId");

            migrationBuilder.RenameIndex(
                name: "IX_ProfessionalSummaries_UserId",
                table: "ProfessionalSummary",
                newName: "IX_ProfessionalSummary_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Experiences_UserId",
                table: "Experience",
                newName: "IX_Experience_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Educations_UserId",
                table: "Education",
                newName: "IX_Education_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Contacts_UserId",
                table: "Contact",
                newName: "IX_Contact_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Certifications_UserId",
                table: "Certification",
                newName: "IX_Certification_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skill",
                table: "Skill",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResumeTemplate",
                table: "ResumeTemplate",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resume",
                table: "Resume",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExperienceResponsibility",
                table: "ExperienceResponsibility",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfessionalSummary",
                table: "ProfessionalSummary",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Experience",
                table: "Experience",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Education",
                table: "Education",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contact",
                table: "Contact",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certification",
                table: "Certification",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Certification_User_UserId",
                table: "Certification",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_User_UserId",
                table: "Contact",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Education_User_UserId",
                table: "Education",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Experience_User_UserId",
                table: "Experience",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExperienceResponsibility_Experience_ExperienceId",
                table: "ExperienceResponsibility",
                column: "ExperienceId",
                principalTable: "Experience",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalSummary_User_UserId",
                table: "ProfessionalSummary",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resume_User_UserId",
                table: "Resume",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResumeTemplate_Resume_ResumeId",
                table: "ResumeTemplate",
                column: "ResumeId",
                principalTable: "Resume",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_User_UserId",
                table: "Skill",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certification_User_UserId",
                table: "Certification");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_User_UserId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Education_User_UserId",
                table: "Education");

            migrationBuilder.DropForeignKey(
                name: "FK_Experience_User_UserId",
                table: "Experience");

            migrationBuilder.DropForeignKey(
                name: "FK_ExperienceResponsibility_Experience_ExperienceId",
                table: "ExperienceResponsibility");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalSummary_User_UserId",
                table: "ProfessionalSummary");

            migrationBuilder.DropForeignKey(
                name: "FK_Resume_User_UserId",
                table: "Resume");

            migrationBuilder.DropForeignKey(
                name: "FK_ResumeTemplate_Resume_ResumeId",
                table: "ResumeTemplate");

            migrationBuilder.DropForeignKey(
                name: "FK_Skill_User_UserId",
                table: "Skill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skill",
                table: "Skill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResumeTemplate",
                table: "ResumeTemplate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resume",
                table: "Resume");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfessionalSummary",
                table: "ProfessionalSummary");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExperienceResponsibility",
                table: "ExperienceResponsibility");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Experience",
                table: "Experience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Education",
                table: "Education");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contact",
                table: "Contact");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certification",
                table: "Certification");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Skill",
                newName: "Skills");

            migrationBuilder.RenameTable(
                name: "ResumeTemplate",
                newName: "ResumeTemplates");

            migrationBuilder.RenameTable(
                name: "Resume",
                newName: "Resumes");

            migrationBuilder.RenameTable(
                name: "ProfessionalSummary",
                newName: "ProfessionalSummaries");

            migrationBuilder.RenameTable(
                name: "ExperienceResponsibility",
                newName: "Responsibilities");

            migrationBuilder.RenameTable(
                name: "Experience",
                newName: "Experiences");

            migrationBuilder.RenameTable(
                name: "Education",
                newName: "Educations");

            migrationBuilder.RenameTable(
                name: "Contact",
                newName: "Contacts");

            migrationBuilder.RenameTable(
                name: "Certification",
                newName: "Certifications");

            migrationBuilder.RenameIndex(
                name: "IX_Skill_UserId",
                table: "Skills",
                newName: "IX_Skills_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ResumeTemplate_ResumeId",
                table: "ResumeTemplates",
                newName: "IX_ResumeTemplates_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Resume_UserId",
                table: "Resumes",
                newName: "IX_Resumes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProfessionalSummary_UserId",
                table: "ProfessionalSummaries",
                newName: "IX_ProfessionalSummaries_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ExperienceResponsibility_ExperienceId",
                table: "Responsibilities",
                newName: "IX_Responsibilities_ExperienceId");

            migrationBuilder.RenameIndex(
                name: "IX_Experience_UserId",
                table: "Experiences",
                newName: "IX_Experiences_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Education_UserId",
                table: "Educations",
                newName: "IX_Educations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Contact_UserId",
                table: "Contacts",
                newName: "IX_Contacts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Certification_UserId",
                table: "Certifications",
                newName: "IX_Certifications_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skills",
                table: "Skills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResumeTemplates",
                table: "ResumeTemplates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resumes",
                table: "Resumes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfessionalSummaries",
                table: "ProfessionalSummaries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Responsibilities",
                table: "Responsibilities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Experiences",
                table: "Experiences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Educations",
                table: "Educations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certifications",
                table: "Certifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_Users_UserId",
                table: "Certifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Users_UserId",
                table: "Contacts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Users_UserId",
                table: "Educations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Users_UserId",
                table: "Experiences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalSummaries_Users_UserId",
                table: "ProfessionalSummaries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Responsibilities_Experiences_ExperienceId",
                table: "Responsibilities",
                column: "ExperienceId",
                principalTable: "Experiences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_Users_UserId",
                table: "Resumes",
                column: "UserId",
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
                name: "FK_Skills_Users_UserId",
                table: "Skills",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
