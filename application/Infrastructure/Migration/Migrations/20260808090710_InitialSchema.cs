using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeEnhancer.Infrastructure.Migration.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "resume");

            migrationBuilder.CreateTable(
                name: "B_Resume",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Photo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResumeTemplate = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_Resume", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_ResumeSectionSetup",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SectionType = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObsoleteFlag = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_S_ResumeSectionSetup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BR_Certification",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResumeId = table.Column<int>(type: "int", nullable: false),
                    CertificationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IssuingOrganization = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CredentialId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CredentialUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_Certification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_Certification_B_Resume_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "resume",
                        principalTable: "B_Resume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BR_Education",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResumeId = table.Column<int>(type: "int", nullable: false),
                    PassingYear = table.Column<int>(type: "int", nullable: true),
                    Degree = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Institution = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_Education", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_Education_B_Resume_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "resume",
                        principalTable: "B_Resume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BR_PersonalInformation",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResumeId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UseSameEmailAsProfile = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UseSamePhoneNumberAsProfile = table.Column<bool>(type: "bit", nullable: false),
                    UseSameAwardsAsProfile = table.Column<bool>(type: "bit", nullable: false),
                    UseSameLanguagesAsProfile = table.Column<bool>(type: "bit", nullable: false),
                    UseSameHobbiesAsProfile = table.Column<bool>(type: "bit", nullable: false),
                    UseSameSocialMediaLinksAsProfile = table.Column<bool>(type: "bit", nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_PersonalInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_PersonalInformation_B_Resume_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "resume",
                        principalTable: "B_Resume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BR_Project",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResumeId = table.Column<int>(type: "int", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TechnologiesUsed = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_Project_B_Resume_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "resume",
                        principalTable: "B_Resume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BR_Skill",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResumeId = table.Column<int>(type: "int", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProficiencyLevel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    YearsOfExperience = table.Column<decimal>(type: "decimal(4,1)", precision: 4, scale: 1, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_Skill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_Skill_B_Resume_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "resume",
                        principalTable: "B_Resume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BR_WorkExperience",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResumeId = table.Column<int>(type: "int", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_WorkExperience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_WorkExperience_B_Resume_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "resume",
                        principalTable: "B_Resume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BR_Address",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonalInformationId = table.Column<int>(type: "int", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_Address_BR_PersonalInformation_PersonalInformationId",
                        column: x => x.PersonalInformationId,
                        principalSchema: "resume",
                        principalTable: "BR_PersonalInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BR_Award",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonalInformationId = table.Column<int>(type: "int", nullable: false),
                    AwardName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IssuingOrganization = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AwardDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_Award", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_Award_BR_PersonalInformation_PersonalInformationId",
                        column: x => x.PersonalInformationId,
                        principalSchema: "resume",
                        principalTable: "BR_PersonalInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BR_Hobby",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonalInformationId = table.Column<int>(type: "int", nullable: false),
                    HobbyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_Hobby", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_Hobby_BR_PersonalInformation_PersonalInformationId",
                        column: x => x.PersonalInformationId,
                        principalSchema: "resume",
                        principalTable: "BR_PersonalInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BR_Language",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonalInformationId = table.Column<int>(type: "int", nullable: false),
                    LanguageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProficiencyLevel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_Language", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_Language_BR_PersonalInformation_PersonalInformationId",
                        column: x => x.PersonalInformationId,
                        principalSchema: "resume",
                        principalTable: "BR_PersonalInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BR_SocialMediaLink",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonalInformationId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_SocialMediaLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_SocialMediaLink_BR_PersonalInformation_PersonalInformationId",
                        column: x => x.PersonalInformationId,
                        principalSchema: "resume",
                        principalTable: "BR_PersonalInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_B_Resume_UserId",
                schema: "resume",
                table: "B_Resume",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_Address_PersonalInformationId",
                schema: "resume",
                table: "BR_Address",
                column: "PersonalInformationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BR_Award_PersonalInformationId",
                schema: "resume",
                table: "BR_Award",
                column: "PersonalInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_Certification_ResumeId",
                schema: "resume",
                table: "BR_Certification",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_Education_ResumeId",
                schema: "resume",
                table: "BR_Education",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_Hobby_PersonalInformationId",
                schema: "resume",
                table: "BR_Hobby",
                column: "PersonalInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_Language_PersonalInformationId",
                schema: "resume",
                table: "BR_Language",
                column: "PersonalInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_PersonalInformation_ResumeId",
                schema: "resume",
                table: "BR_PersonalInformation",
                column: "ResumeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BR_Project_ResumeId",
                schema: "resume",
                table: "BR_Project",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_Skill_ResumeId",
                schema: "resume",
                table: "BR_Skill",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_SocialMediaLink_PersonalInformationId",
                schema: "resume",
                table: "BR_SocialMediaLink",
                column: "PersonalInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_WorkExperience_ResumeId",
                schema: "resume",
                table: "BR_WorkExperience",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_S_ResumeSectionSetup_Code",
                schema: "resume",
                table: "S_ResumeSectionSetup",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_ResumeSectionSetup_DisplayOrder",
                schema: "resume",
                table: "S_ResumeSectionSetup",
                column: "DisplayOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_ResumeSectionSetup_Guid",
                schema: "resume",
                table: "S_ResumeSectionSetup",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_ResumeSectionSetup_SectionType",
                schema: "resume",
                table: "S_ResumeSectionSetup",
                column: "SectionType",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BR_Address",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_Award",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_Certification",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_Education",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_Hobby",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_Language",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_Project",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_Skill",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_SocialMediaLink",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_WorkExperience",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "S_ResumeSectionSetup",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_PersonalInformation",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "B_Resume",
                schema: "resume");
        }
    }
}


