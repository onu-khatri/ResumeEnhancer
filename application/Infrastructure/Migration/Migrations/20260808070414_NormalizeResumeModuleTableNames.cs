using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeEnhancer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeResumeModuleTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Awards_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Awards");

            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_Resumes_ResumeId",
                schema: "resume",
                table: "Certifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Education_Resumes_ResumeId",
                schema: "resume",
                table: "Education");

            migrationBuilder.DropForeignKey(
                name: "FK_Hobbies_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Hobbies");

            migrationBuilder.DropForeignKey(
                name: "FK_Languages_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Languages");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInformation_Resumes_ResumeId",
                schema: "resume",
                table: "PersonalInformation");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Resumes_ResumeId",
                schema: "resume",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Resumes_ResumeId",
                schema: "resume",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_SocialMediaLinks_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "SocialMediaLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkExperiences_Resumes_ResumeId",
                schema: "resume",
                table: "WorkExperiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkExperiences",
                schema: "resume",
                table: "WorkExperiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SocialMediaLinks",
                schema: "resume",
                table: "SocialMediaLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skills",
                schema: "resume",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResumeSectionSetups",
                schema: "resume",
                table: "ResumeSectionSetups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resumes",
                schema: "resume",
                table: "Resumes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projects",
                schema: "resume",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Languages",
                schema: "resume",
                table: "Languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hobbies",
                schema: "resume",
                table: "Hobbies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certifications",
                schema: "resume",
                table: "Certifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Awards",
                schema: "resume",
                table: "Awards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                schema: "resume",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "WorkExperiences",
                schema: "resume",
                newName: "WorkExperience",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "SocialMediaLinks",
                schema: "resume",
                newName: "SocialMediaLink",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Skills",
                schema: "resume",
                newName: "Skill",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "ResumeSectionSetups",
                schema: "resume",
                newName: "ResumeSectionSetup",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Resumes",
                schema: "resume",
                newName: "Resume",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Projects",
                schema: "resume",
                newName: "Project",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Languages",
                schema: "resume",
                newName: "Language",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Hobbies",
                schema: "resume",
                newName: "Hobby",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Certifications",
                schema: "resume",
                newName: "Certification",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Awards",
                schema: "resume",
                newName: "Award",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Addresses",
                schema: "resume",
                newName: "Address",
                newSchema: "resume");

            migrationBuilder.RenameIndex(
                name: "IX_WorkExperiences_ResumeId",
                schema: "resume",
                table: "WorkExperience",
                newName: "IX_WorkExperience_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_SocialMediaLinks_PersonalInformationId",
                schema: "resume",
                table: "SocialMediaLink",
                newName: "IX_SocialMediaLink_PersonalInformationId");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_ResumeId",
                schema: "resume",
                table: "Skill",
                newName: "IX_Skill_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_ResumeSectionSetups_SectionType",
                schema: "resume",
                table: "ResumeSectionSetup",
                newName: "IX_ResumeSectionSetup_SectionType");

            migrationBuilder.RenameIndex(
                name: "IX_ResumeSectionSetups_DisplayOrder",
                schema: "resume",
                table: "ResumeSectionSetup",
                newName: "IX_ResumeSectionSetup_DisplayOrder");

            migrationBuilder.RenameIndex(
                name: "IX_Resumes_UserId",
                schema: "resume",
                table: "Resume",
                newName: "IX_Resume_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_ResumeId",
                schema: "resume",
                table: "Project",
                newName: "IX_Project_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Languages_PersonalInformationId",
                schema: "resume",
                table: "Language",
                newName: "IX_Language_PersonalInformationId");

            migrationBuilder.RenameIndex(
                name: "IX_Hobbies_PersonalInformationId",
                schema: "resume",
                table: "Hobby",
                newName: "IX_Hobby_PersonalInformationId");

            migrationBuilder.RenameIndex(
                name: "IX_Certifications_ResumeId",
                schema: "resume",
                table: "Certification",
                newName: "IX_Certification_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Awards_PersonalInformationId",
                schema: "resume",
                table: "Award",
                newName: "IX_Award_PersonalInformationId");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_PersonalInformationId",
                schema: "resume",
                table: "Address",
                newName: "IX_Address_PersonalInformationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkExperience",
                schema: "resume",
                table: "WorkExperience",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SocialMediaLink",
                schema: "resume",
                table: "SocialMediaLink",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skill",
                schema: "resume",
                table: "Skill",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResumeSectionSetup",
                schema: "resume",
                table: "ResumeSectionSetup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resume",
                schema: "resume",
                table: "Resume",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project",
                schema: "resume",
                table: "Project",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Language",
                schema: "resume",
                table: "Language",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hobby",
                schema: "resume",
                table: "Hobby",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certification",
                schema: "resume",
                table: "Certification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Award",
                schema: "resume",
                table: "Award",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                schema: "resume",
                table: "Address",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Address",
                column: "PersonalInformationId",
                principalSchema: "resume",
                principalTable: "PersonalInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Award_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Award",
                column: "PersonalInformationId",
                principalSchema: "resume",
                principalTable: "PersonalInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certification_Resume_ResumeId",
                schema: "resume",
                table: "Certification",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resume",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Education_Resume_ResumeId",
                schema: "resume",
                table: "Education",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resume",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hobby_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Hobby",
                column: "PersonalInformationId",
                principalSchema: "resume",
                principalTable: "PersonalInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Language_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Language",
                column: "PersonalInformationId",
                principalSchema: "resume",
                principalTable: "PersonalInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInformation_Resume_ResumeId",
                schema: "resume",
                table: "PersonalInformation",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resume",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Resume_ResumeId",
                schema: "resume",
                table: "Project",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resume",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_Resume_ResumeId",
                schema: "resume",
                table: "Skill",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resume",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SocialMediaLink_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "SocialMediaLink",
                column: "PersonalInformationId",
                principalSchema: "resume",
                principalTable: "PersonalInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkExperience_Resume_ResumeId",
                schema: "resume",
                table: "WorkExperience",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resume",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Award_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Award");

            migrationBuilder.DropForeignKey(
                name: "FK_Certification_Resume_ResumeId",
                schema: "resume",
                table: "Certification");

            migrationBuilder.DropForeignKey(
                name: "FK_Education_Resume_ResumeId",
                schema: "resume",
                table: "Education");

            migrationBuilder.DropForeignKey(
                name: "FK_Hobby_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Hobby");

            migrationBuilder.DropForeignKey(
                name: "FK_Language_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Language");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInformation_Resume_ResumeId",
                schema: "resume",
                table: "PersonalInformation");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Resume_ResumeId",
                schema: "resume",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Skill_Resume_ResumeId",
                schema: "resume",
                table: "Skill");

            migrationBuilder.DropForeignKey(
                name: "FK_SocialMediaLink_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "SocialMediaLink");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkExperience_Resume_ResumeId",
                schema: "resume",
                table: "WorkExperience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkExperience",
                schema: "resume",
                table: "WorkExperience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SocialMediaLink",
                schema: "resume",
                table: "SocialMediaLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skill",
                schema: "resume",
                table: "Skill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResumeSectionSetup",
                schema: "resume",
                table: "ResumeSectionSetup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resume",
                schema: "resume",
                table: "Resume");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Project",
                schema: "resume",
                table: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Language",
                schema: "resume",
                table: "Language");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hobby",
                schema: "resume",
                table: "Hobby");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certification",
                schema: "resume",
                table: "Certification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Award",
                schema: "resume",
                table: "Award");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                schema: "resume",
                table: "Address");

            migrationBuilder.RenameTable(
                name: "WorkExperience",
                schema: "resume",
                newName: "WorkExperiences",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "SocialMediaLink",
                schema: "resume",
                newName: "SocialMediaLinks",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Skill",
                schema: "resume",
                newName: "Skills",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "ResumeSectionSetup",
                schema: "resume",
                newName: "ResumeSectionSetups",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Resume",
                schema: "resume",
                newName: "Resumes",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Project",
                schema: "resume",
                newName: "Projects",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Language",
                schema: "resume",
                newName: "Languages",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Hobby",
                schema: "resume",
                newName: "Hobbies",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Certification",
                schema: "resume",
                newName: "Certifications",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Award",
                schema: "resume",
                newName: "Awards",
                newSchema: "resume");

            migrationBuilder.RenameTable(
                name: "Address",
                schema: "resume",
                newName: "Addresses",
                newSchema: "resume");

            migrationBuilder.RenameIndex(
                name: "IX_WorkExperience_ResumeId",
                schema: "resume",
                table: "WorkExperiences",
                newName: "IX_WorkExperiences_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_SocialMediaLink_PersonalInformationId",
                schema: "resume",
                table: "SocialMediaLinks",
                newName: "IX_SocialMediaLinks_PersonalInformationId");

            migrationBuilder.RenameIndex(
                name: "IX_Skill_ResumeId",
                schema: "resume",
                table: "Skills",
                newName: "IX_Skills_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_ResumeSectionSetup_SectionType",
                schema: "resume",
                table: "ResumeSectionSetups",
                newName: "IX_ResumeSectionSetups_SectionType");

            migrationBuilder.RenameIndex(
                name: "IX_ResumeSectionSetup_DisplayOrder",
                schema: "resume",
                table: "ResumeSectionSetups",
                newName: "IX_ResumeSectionSetups_DisplayOrder");

            migrationBuilder.RenameIndex(
                name: "IX_Resume_UserId",
                schema: "resume",
                table: "Resumes",
                newName: "IX_Resumes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ResumeId",
                schema: "resume",
                table: "Projects",
                newName: "IX_Projects_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Language_PersonalInformationId",
                schema: "resume",
                table: "Languages",
                newName: "IX_Languages_PersonalInformationId");

            migrationBuilder.RenameIndex(
                name: "IX_Hobby_PersonalInformationId",
                schema: "resume",
                table: "Hobbies",
                newName: "IX_Hobbies_PersonalInformationId");

            migrationBuilder.RenameIndex(
                name: "IX_Certification_ResumeId",
                schema: "resume",
                table: "Certifications",
                newName: "IX_Certifications_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Award_PersonalInformationId",
                schema: "resume",
                table: "Awards",
                newName: "IX_Awards_PersonalInformationId");

            migrationBuilder.RenameIndex(
                name: "IX_Address_PersonalInformationId",
                schema: "resume",
                table: "Addresses",
                newName: "IX_Addresses_PersonalInformationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkExperiences",
                schema: "resume",
                table: "WorkExperiences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SocialMediaLinks",
                schema: "resume",
                table: "SocialMediaLinks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skills",
                schema: "resume",
                table: "Skills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResumeSectionSetups",
                schema: "resume",
                table: "ResumeSectionSetups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resumes",
                schema: "resume",
                table: "Resumes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projects",
                schema: "resume",
                table: "Projects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Languages",
                schema: "resume",
                table: "Languages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hobbies",
                schema: "resume",
                table: "Hobbies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certifications",
                schema: "resume",
                table: "Certifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Awards",
                schema: "resume",
                table: "Awards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                schema: "resume",
                table: "Addresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Addresses",
                column: "PersonalInformationId",
                principalSchema: "resume",
                principalTable: "PersonalInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Awards",
                column: "PersonalInformationId",
                principalSchema: "resume",
                principalTable: "PersonalInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_Resumes_ResumeId",
                schema: "resume",
                table: "Certifications",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Education_Resumes_ResumeId",
                schema: "resume",
                table: "Education",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hobbies_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Hobbies",
                column: "PersonalInformationId",
                principalSchema: "resume",
                principalTable: "PersonalInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "Languages",
                column: "PersonalInformationId",
                principalSchema: "resume",
                principalTable: "PersonalInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInformation_Resumes_ResumeId",
                schema: "resume",
                table: "PersonalInformation",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Resumes_ResumeId",
                schema: "resume",
                table: "Projects",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Resumes_ResumeId",
                schema: "resume",
                table: "Skills",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SocialMediaLinks_PersonalInformation_PersonalInformationId",
                schema: "resume",
                table: "SocialMediaLinks",
                column: "PersonalInformationId",
                principalSchema: "resume",
                principalTable: "PersonalInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkExperiences_Resumes_ResumeId",
                schema: "resume",
                table: "WorkExperiences",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
