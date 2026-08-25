using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeEnhancer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "billing");

            migrationBuilder.EnsureSchema(
                name: "resume");

            migrationBuilder.EnsureSchema(
                name: "profiling");

            migrationBuilder.EnsureSchema(
                name: "template");

            migrationBuilder.CreateTable(
                name: "B_User",
                schema: "profiling",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    IsDeactivated = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_AccessProfile",
                schema: "profiling",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_S_AccessProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_BillingPlan",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BillingInterval = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeactivated = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
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
                    table.PrimaryKey("PK_S_BillingPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_ResumeSectionSetup",
                schema: "resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(type: "int", nullable: false),
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
                name: "S_Role",
                schema: "profiling",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_S_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_TemplateCategory",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsDeactivated = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
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
                    table.PrimaryKey("PK_S_TemplateCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_TemplateRenderTypeSetup",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_S_TemplateRenderTypeSetup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_UserAddressTypeSetup",
                schema: "profiling",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_S_UserAddressTypeSetup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "B_BillingAccount",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExternalReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_BillingAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_B_BillingAccount_B_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "profiling",
                        principalTable: "B_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BR_UserAccessProfile",
                schema: "profiling",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AccessProfileId = table.Column<int>(type: "int", nullable: false),
                    AssignedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_UserAccessProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_UserAccessProfile_B_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "profiling",
                        principalTable: "B_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BR_UserAccessProfile_S_AccessProfile_AccessProfileId",
                        column: x => x.AccessProfileId,
                        principalSchema: "profiling",
                        principalTable: "S_AccessProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SR_AccessProfileRole",
                schema: "profiling",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessProfileId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SR_AccessProfileRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SR_AccessProfileRole_S_AccessProfile_AccessProfileId",
                        column: x => x.AccessProfileId,
                        principalSchema: "profiling",
                        principalTable: "S_AccessProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SR_AccessProfileRole_S_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "profiling",
                        principalTable: "S_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "S_Template",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TemplateCategoryId = table.Column<int>(type: "int", nullable: false),
                    RenderTypeId = table.Column<int>(type: "int", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", maxLength: 20000, nullable: false),
                    PreviewImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeactivated = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
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
                    table.PrimaryKey("PK_S_Template", x => x.Id);
                    table.ForeignKey(
                        name: "FK_S_Template_S_TemplateCategory_TemplateCategoryId",
                        column: x => x.TemplateCategoryId,
                        principalSchema: "template",
                        principalTable: "S_TemplateCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_S_Template_S_TemplateRenderTypeSetup_RenderTypeId",
                        column: x => x.RenderTypeId,
                        principalSchema: "template",
                        principalTable: "S_TemplateRenderTypeSetup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "B_UserAddress",
                schema: "profiling",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AddressTypeId = table.Column<int>(type: "int", nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_UserAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_B_UserAddress_B_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "profiling",
                        principalTable: "B_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_B_UserAddress_S_UserAddressTypeSetup_AddressTypeId",
                        column: x => x.AddressTypeId,
                        principalSchema: "profiling",
                        principalTable: "S_UserAddressTypeSetup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    TemplateId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_Resume", x => x.Id);
                    table.ForeignKey(
                        name: "FK_B_Resume_B_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "profiling",
                        principalTable: "B_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_B_Resume_S_Template_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "template",
                        principalTable: "S_Template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BR_BillingSubscription",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillingAccountId = table.Column<int>(type: "int", nullable: false),
                    BillingPlanId = table.Column<int>(type: "int", nullable: false),
                    ResumeId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_BillingSubscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BR_BillingSubscription_B_BillingAccount_BillingAccountId",
                        column: x => x.BillingAccountId,
                        principalSchema: "billing",
                        principalTable: "B_BillingAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BR_BillingSubscription_B_Resume_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "resume",
                        principalTable: "B_Resume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BR_BillingSubscription_S_BillingPlan_BillingPlanId",
                        column: x => x.BillingPlanId,
                        principalSchema: "billing",
                        principalTable: "S_BillingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_B_BillingAccount_AccountNumber",
                schema: "billing",
                table: "B_BillingAccount",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_B_BillingAccount_UserId",
                schema: "billing",
                table: "B_BillingAccount",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_B_Resume_TemplateId",
                schema: "resume",
                table: "B_Resume",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_B_Resume_UserId",
                schema: "resume",
                table: "B_Resume",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_B_User_Email",
                schema: "profiling",
                table: "B_User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_B_UserAddress_AddressTypeId",
                schema: "profiling",
                table: "B_UserAddress",
                column: "AddressTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_B_UserAddress_UserId_AddressTypeId",
                schema: "profiling",
                table: "B_UserAddress",
                columns: new[] { "UserId", "AddressTypeId" },
                unique: true);

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
                name: "IX_BR_BillingSubscription_BillingAccountId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "BillingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_BillingSubscription_BillingPlanId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "BillingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_BillingSubscription_ResumeId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "ResumeId");

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
                name: "IX_BR_UserAccessProfile_AccessProfileId",
                schema: "profiling",
                table: "BR_UserAccessProfile",
                column: "AccessProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_UserAccessProfile_UserId_AccessProfileId",
                schema: "profiling",
                table: "BR_UserAccessProfile",
                columns: new[] { "UserId", "AccessProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BR_WorkExperience_ResumeId",
                schema: "resume",
                table: "BR_WorkExperience",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_S_AccessProfile_Code",
                schema: "profiling",
                table: "S_AccessProfile",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_AccessProfile_Guid",
                schema: "profiling",
                table: "S_AccessProfile",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_BillingPlan_Code",
                schema: "billing",
                table: "S_BillingPlan",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_BillingPlan_Guid",
                schema: "billing",
                table: "S_BillingPlan",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_ResumeSectionSetup_Code",
                schema: "resume",
                table: "S_ResumeSectionSetup",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_ResumeSectionSetup_Guid",
                schema: "resume",
                table: "S_ResumeSectionSetup",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_ResumeSectionSetup_Order",
                schema: "resume",
                table: "S_ResumeSectionSetup",
                column: "Order",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_Role_Code",
                schema: "profiling",
                table: "S_Role",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_Role_Guid",
                schema: "profiling",
                table: "S_Role",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_Template_Code",
                schema: "template",
                table: "S_Template",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_Template_Guid",
                schema: "template",
                table: "S_Template",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_Template_RenderTypeId",
                schema: "template",
                table: "S_Template",
                column: "RenderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_S_Template_TemplateCategoryId",
                schema: "template",
                table: "S_Template",
                column: "TemplateCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_S_TemplateCategory_Code",
                schema: "template",
                table: "S_TemplateCategory",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_TemplateCategory_Guid",
                schema: "template",
                table: "S_TemplateCategory",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_TemplateRenderTypeSetup_Code",
                schema: "template",
                table: "S_TemplateRenderTypeSetup",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_TemplateRenderTypeSetup_Guid",
                schema: "template",
                table: "S_TemplateRenderTypeSetup",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_UserAddressTypeSetup_Code",
                schema: "profiling",
                table: "S_UserAddressTypeSetup",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_UserAddressTypeSetup_Guid",
                schema: "profiling",
                table: "S_UserAddressTypeSetup",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SR_AccessProfileRole_AccessProfileId_RoleId",
                schema: "profiling",
                table: "SR_AccessProfileRole",
                columns: new[] { "AccessProfileId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SR_AccessProfileRole_Code",
                schema: "profiling",
                table: "SR_AccessProfileRole",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SR_AccessProfileRole_Guid",
                schema: "profiling",
                table: "SR_AccessProfileRole",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SR_AccessProfileRole_RoleId",
                schema: "profiling",
                table: "SR_AccessProfileRole",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "B_UserAddress",
                schema: "profiling");

            migrationBuilder.DropTable(
                name: "BR_Address",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_Award",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "BR_BillingSubscription",
                schema: "billing");

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
                name: "BR_UserAccessProfile",
                schema: "profiling");

            migrationBuilder.DropTable(
                name: "BR_WorkExperience",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "S_ResumeSectionSetup",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "SR_AccessProfileRole",
                schema: "profiling");

            migrationBuilder.DropTable(
                name: "S_UserAddressTypeSetup",
                schema: "profiling");

            migrationBuilder.DropTable(
                name: "B_BillingAccount",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "S_BillingPlan",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "BR_PersonalInformation",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "S_AccessProfile",
                schema: "profiling");

            migrationBuilder.DropTable(
                name: "S_Role",
                schema: "profiling");

            migrationBuilder.DropTable(
                name: "B_Resume",
                schema: "resume");

            migrationBuilder.DropTable(
                name: "B_User",
                schema: "profiling");

            migrationBuilder.DropTable(
                name: "S_Template",
                schema: "template");

            migrationBuilder.DropTable(
                name: "S_TemplateCategory",
                schema: "template");

            migrationBuilder.DropTable(
                name: "S_TemplateRenderTypeSetup",
                schema: "template");
        }
    }
}
