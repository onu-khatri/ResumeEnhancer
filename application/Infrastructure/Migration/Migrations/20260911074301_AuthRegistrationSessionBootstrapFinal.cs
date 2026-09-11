using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeEnhancer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuthRegistrationSessionBootstrapFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BR_BillingSubscription_B_Resume_ResumeId",
                schema: "billing",
                table: "BR_BillingSubscription"
            );

            migrationBuilder.EnsureSchema(name: "auth");

            migrationBuilder.DropIndex(
                name: "IX_BR_BillingSubscription_ResumeId",
                schema: "billing",
                table: "BR_BillingSubscription"
            );

            migrationBuilder.AddColumn<string>(
                name: "Capability",
                schema: "profiling",
                table: "S_Role",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "billing",
                table: "BR_BillingSubscription",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "AccessProfileId",
                schema: "billing",
                table: "BR_BillingSubscription",
                type: "int",
                nullable: true
            );

            migrationBuilder.Sql(
                """
                UPDATE subscription
                SET UserId = account.UserId
                FROM billing.BR_BillingSubscription subscription
                INNER JOIN billing.B_BillingAccount account ON account.Id = subscription.BillingAccountId;
                IF EXISTS (SELECT 1 FROM billing.BR_BillingSubscription WHERE UserId IS NULL)
                    THROW 51000, 'Cannot migrate billing subscriptions without an owning billing account user.', 1;
                """
            );

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "billing",
                table: "BR_BillingSubscription",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.CreateTable(
                name: "B_AuthAuditEvent",
                schema: "auth",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<string>(
                        type: "nvarchar(80)",
                        maxLength: 80,
                        nullable: false
                    ),
                    MetadataJson = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
                        nullable: false
                    ),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "SYSUTCDATETIME()"
                    ),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_AuthAuditEvent", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "B_AuthenticationIdentity",
                schema: "auth",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NormalizedEmail = table.Column<string>(
                        type: "nvarchar(320)",
                        maxLength: 320,
                        nullable: false
                    ),
                    PasswordHash = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: false
                    ),
                    EmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    EmailVerifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "SYSUTCDATETIME()"
                    ),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_AuthenticationIdentity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_B_AuthenticationIdentity_B_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "profiling",
                        principalTable: "B_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "B_AuthRegistrationIdempotency",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequestHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ResponseJson = table.Column<string>(type: "nvarchar(12000)", maxLength: 12000, nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_AuthRegistrationIdempotency", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "B_AuthOutboxMessage",
                schema: "auth",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(
                        type: "nvarchar(80)",
                        maxLength: 80,
                        nullable: false
                    ),
                    PayloadJson = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
                        nullable: false
                    ),
                    AvailableAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "SYSUTCDATETIME()"
                    ),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_AuthOutboxMessage", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "B_ConsentRecord",
                schema: "auth",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    VersionId = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Accepted = table.Column<bool>(type: "bit", nullable: false),
                    AcceptedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "SYSUTCDATETIME()"
                    ),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_ConsentRecord", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "B_RefreshSession",
                schema: "auth",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(
                        type: "nvarchar(128)",
                        maxLength: 128,
                        nullable: false
                    ),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RotatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpAddress = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    UserAgent = table.Column<string>(
                        type: "nvarchar(300)",
                        maxLength: 300,
                        nullable: true
                    ),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "SYSUTCDATETIME()"
                    ),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_RefreshSession", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "B_UserEntitlement",
                schema: "profiling",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AccessProfileId = table.Column<int>(type: "int", nullable: false),
                    BillingSubscriptionId = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Source = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "SYSUTCDATETIME()"
                    ),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_UserEntitlement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_B_UserEntitlement_BR_BillingSubscription_BillingSubscriptionId",
                        column: x => x.BillingSubscriptionId,
                        principalSchema: "billing",
                        principalTable: "BR_BillingSubscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_B_UserEntitlement_B_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "profiling",
                        principalTable: "B_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_B_UserEntitlement_S_AccessProfile_AccessProfileId",
                        column: x => x.AccessProfileId,
                        principalSchema: "profiling",
                        principalTable: "S_AccessProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "B_UserPreference",
                schema: "profiling",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Locale = table.Column<string>(
                        type: "nvarchar(10)",
                        maxLength: 10,
                        nullable: false
                    ),
                    OnboardingCompleted = table.Column<bool>(type: "bit", nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "SYSUTCDATETIME()"
                    ),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_UserPreference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_B_UserPreference_B_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "profiling",
                        principalTable: "B_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "S_AuthConsentVersion",
                schema: "auth",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    VersionId = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "SYSUTCDATETIME()"
                    ),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false
                    ),
                    Code = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: false
                    ),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObsoleteFlag = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_S_AuthConsentVersion", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_BR_BillingSubscription_UserId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_AuthAuditEvent_UserId_EventType",
                schema: "auth",
                table: "B_AuthAuditEvent",
                columns: new[] { "UserId", "EventType" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_AuthenticationIdentity_NormalizedEmail",
                schema: "auth",
                table: "B_AuthenticationIdentity",
                column: "NormalizedEmail",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_AuthenticationIdentity_UserId",
                schema: "auth",
                table: "B_AuthenticationIdentity",
                column: "UserId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_AuthRegistrationIdempotency_NormalizedEmail_IdempotencyKey",
                schema: "auth",
                table: "B_AuthRegistrationIdempotency",
                columns: new[] { "NormalizedEmail", "IdempotencyKey" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_AuthOutboxMessage_ProcessedAtUtc_AvailableAtUtc_LeaseExpiresAtUtc",
                schema: "auth",
                table: "B_AuthOutboxMessage",
                columns: new[] { "ProcessedAtUtc", "AvailableAtUtc", "LeaseExpiresAtUtc" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_ConsentRecord_UserId_Type_VersionId",
                schema: "auth",
                table: "B_ConsentRecord",
                columns: new[] { "UserId", "Type", "VersionId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_RefreshSession_ExpiresAtUtc",
                schema: "auth",
                table: "B_RefreshSession",
                column: "ExpiresAtUtc"
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_RefreshSession_TokenHash",
                schema: "auth",
                table: "B_RefreshSession",
                column: "TokenHash",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_RefreshSession_UserId_FamilyId_RevokedAtUtc",
                schema: "auth",
                table: "B_RefreshSession",
                columns: new[] { "UserId", "FamilyId", "RevokedAtUtc" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_UserEntitlement_AccessProfileId",
                schema: "profiling",
                table: "B_UserEntitlement",
                column: "AccessProfileId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_UserEntitlement_BillingSubscriptionId",
                schema: "profiling",
                table: "B_UserEntitlement",
                column: "BillingSubscriptionId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_UserEntitlement_UserId_BillingSubscriptionId_AccessProfileId",
                schema: "profiling",
                table: "B_UserEntitlement",
                columns: new[] { "UserId", "BillingSubscriptionId", "AccessProfileId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_UserEntitlement_UserId_Enabled_ExpiresAtUtc",
                schema: "profiling",
                table: "B_UserEntitlement",
                columns: new[] { "UserId", "Enabled", "ExpiresAtUtc" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_B_UserPreference_UserId",
                schema: "profiling",
                table: "B_UserPreference",
                column: "UserId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_S_AuthConsentVersion_Code",
                schema: "auth",
                table: "S_AuthConsentVersion",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_S_AuthConsentVersion_Guid",
                schema: "auth",
                table: "S_AuthConsentVersion",
                column: "Guid",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_S_AuthConsentVersion_Type_VersionId",
                schema: "auth",
                table: "S_AuthConsentVersion",
                columns: new[] { "Type", "VersionId" },
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_BR_BillingSubscription_B_User_UserId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "UserId",
                principalSchema: "profiling",
                principalTable: "B_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_BR_BillingSubscription_S_AccessProfile_AccessProfileId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "AccessProfileId",
                principalSchema: "profiling",
                principalTable: "S_AccessProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BR_BillingSubscription_B_User_UserId",
                schema: "billing",
                table: "BR_BillingSubscription"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_BR_BillingSubscription_S_AccessProfile_AccessProfileId",
                schema: "billing",
                table: "BR_BillingSubscription"
            );

            migrationBuilder.DropTable(name: "B_AuthAuditEvent", schema: "auth");

            migrationBuilder.DropTable(name: "B_AuthenticationIdentity", schema: "auth");

            migrationBuilder.DropTable(name: "B_AuthRegistrationIdempotency", schema: "auth");

            migrationBuilder.DropTable(name: "B_AuthOutboxMessage", schema: "auth");

            migrationBuilder.DropTable(name: "B_ConsentRecord", schema: "auth");

            migrationBuilder.DropTable(name: "B_RefreshSession", schema: "auth");

            migrationBuilder.DropTable(name: "B_UserEntitlement", schema: "profiling");

            migrationBuilder.DropTable(name: "B_UserPreference", schema: "profiling");

            migrationBuilder.DropTable(name: "S_AuthConsentVersion", schema: "auth");

            migrationBuilder.DropIndex(
                name: "IX_BR_BillingSubscription_UserId",
                schema: "billing",
                table: "BR_BillingSubscription"
            );

            migrationBuilder.DropColumn(name: "Capability", schema: "profiling", table: "S_Role");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "billing",
                table: "BR_BillingSubscription"
            );

            migrationBuilder.DropIndex(
                name: "IX_BR_BillingSubscription_AccessProfileId",
                schema: "billing",
                table: "BR_BillingSubscription"
            );

            migrationBuilder.DropColumn(
                name: "AccessProfileId",
                schema: "billing",
                table: "BR_BillingSubscription"
            );

            migrationBuilder.AddColumn<int>(
                name: "ResumeId",
                schema: "billing",
                table: "BR_BillingSubscription",
                type: "int",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_BR_BillingSubscription_ResumeId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "ResumeId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_BR_BillingSubscription_B_Resume_ResumeId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "ResumeId",
                principalSchema: "resume",
                principalTable: "B_Resume",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
