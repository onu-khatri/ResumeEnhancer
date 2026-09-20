using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeEnhancer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidatedAuthenticationAuthorizationHardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "profiling",
                table: "B_User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "auth",
                table: "B_RefreshSession",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAtUtc",
                schema: "auth",
                table: "B_RefreshSession",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevocationReason",
                schema: "auth",
                table: "B_RefreshSession",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                schema: "auth",
                table: "B_AuthenticationIdentity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastFailedLoginAtUtc",
                schema: "auth",
                table: "B_AuthenticationIdentity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedUntilUtc",
                schema: "auth",
                table: "B_AuthenticationIdentity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "B_AuthSigningKeyMetadata",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyIdentifier = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProtectedMaterial = table.Column<byte[]>(type: "varbinary(max)", maxLength: 8192, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LifecycleVersion = table.Column<long>(type: "bigint", nullable: false),
                    ActivatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RetiredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvalidatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_AuthSigningKeyMetadata", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "B_PasswordHistoryEntry",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthenticationIdentityId = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_PasswordHistoryEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_B_PasswordHistoryEntry_B_AuthenticationIdentity_AuthenticationIdentityId",
                        column: x => x.AuthenticationIdentityId,
                        principalSchema: "auth",
                        principalTable: "B_AuthenticationIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "S_AuthChallengePurpose",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_S_AuthChallengePurpose", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_DbCacheLimiter",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    WindowStartedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_S_DbCacheLimiter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "B_AuthChallenge",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthenticationIdentityId = table.Column<int>(type: "int", nullable: false),
                    AuthChallengePurposeId = table.Column<int>(type: "int", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IssuedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_AuthChallenge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_B_AuthChallenge_B_AuthenticationIdentity_AuthenticationIdentityId",
                        column: x => x.AuthenticationIdentityId,
                        principalSchema: "auth",
                        principalTable: "B_AuthenticationIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_B_AuthChallenge_S_AuthChallengePurpose_AuthChallengePurposeId",
                        column: x => x.AuthChallengePurposeId,
                        principalSchema: "auth",
                        principalTable: "S_AuthChallengePurpose",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_B_AuthenticationIdentity_LockedUntilUtc_LastFailedLoginAtUtc",
                schema: "auth",
                table: "B_AuthenticationIdentity",
                columns: new[] { "LockedUntilUtc", "LastFailedLoginAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_B_AuthChallenge_AuthChallengePurposeId_TokenHash_ConsumedAtUtc_ExpiresAtUtc",
                schema: "auth",
                table: "B_AuthChallenge",
                columns: new[] { "AuthChallengePurposeId", "TokenHash", "ConsumedAtUtc", "ExpiresAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_B_AuthChallenge_AuthenticationIdentityId_AuthChallengePurposeId_TokenHash",
                schema: "auth",
                table: "B_AuthChallenge",
                columns: new[] { "AuthenticationIdentityId", "AuthChallengePurposeId", "TokenHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_B_AuthSigningKeyMetadata_IsActive",
                schema: "auth",
                table: "B_AuthSigningKeyMetadata",
                column: "IsActive",
                unique: true,
                filter: "[IsActive] = 1 AND [RetiredAtUtc] IS NULL AND [InvalidatedAtUtc] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_B_AuthSigningKeyMetadata_KeyIdentifier",
                schema: "auth",
                table: "B_AuthSigningKeyMetadata",
                column: "KeyIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_B_AuthSigningKeyMetadata_RetiredAtUtc_InvalidatedAtUtc_ActivatedAtUtc",
                schema: "auth",
                table: "B_AuthSigningKeyMetadata",
                columns: new[] { "RetiredAtUtc", "InvalidatedAtUtc", "ActivatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_B_PasswordHistoryEntry_AuthenticationIdentityId_CreatedAtUtc",
                schema: "auth",
                table: "B_PasswordHistoryEntry",
                columns: new[] { "AuthenticationIdentityId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_S_AuthChallengePurpose_Code",
                schema: "auth",
                table: "S_AuthChallengePurpose",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_AuthChallengePurpose_Guid",
                schema: "auth",
                table: "S_AuthChallengePurpose",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_DbCacheLimiter_ExpiresAtUtc",
                schema: "dbo",
                table: "S_DbCacheLimiter",
                column: "ExpiresAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_S_DbCacheLimiter_KeyHash",
                schema: "dbo",
                table: "S_DbCacheLimiter",
                column: "KeyHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "B_AuthChallenge",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "B_AuthSigningKeyMetadata",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "B_PasswordHistoryEntry",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "S_DbCacheLimiter",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "S_AuthChallengePurpose",
                schema: "auth");

            migrationBuilder.DropIndex(
                name: "IX_B_AuthenticationIdentity_LockedUntilUtc_LastFailedLoginAtUtc",
                schema: "auth",
                table: "B_AuthenticationIdentity");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "profiling",
                table: "B_User");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "auth",
                table: "B_RefreshSession");

            migrationBuilder.DropColumn(
                name: "LastUsedAtUtc",
                schema: "auth",
                table: "B_RefreshSession");

            migrationBuilder.DropColumn(
                name: "RevocationReason",
                schema: "auth",
                table: "B_RefreshSession");

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                schema: "auth",
                table: "B_AuthenticationIdentity");

            migrationBuilder.DropColumn(
                name: "LastFailedLoginAtUtc",
                schema: "auth",
                table: "B_AuthenticationIdentity");

            migrationBuilder.DropColumn(
                name: "LockedUntilUtc",
                schema: "auth",
                table: "B_AuthenticationIdentity");
        }
    }
}
