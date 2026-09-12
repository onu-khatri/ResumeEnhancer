using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeEnhancer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AccountRegistrationSessionBootstrapFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BR_BillingSubscription_S_AccessProfile_AccessProfileId",
                schema: "billing",
                table: "BR_BillingSubscription");

            migrationBuilder.DropTable(
                name: "B_UserEntitlement",
                schema: "profiling");

            migrationBuilder.DropIndex(
                name: "IX_BR_UserAccessProfile_UserId_AccessProfileId",
                schema: "profiling",
                table: "BR_UserAccessProfile");

            migrationBuilder.DropIndex(
                name: "IX_BR_BillingSubscription_AccessProfileId",
                schema: "billing",
                table: "BR_BillingSubscription");

            migrationBuilder.DropColumn(
                name: "BillingInterval",
                schema: "billing",
                table: "S_BillingPlan");

            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "billing",
                table: "S_BillingPlan");

            migrationBuilder.DropColumn(
                name: "AccessProfileId",
                schema: "billing",
                table: "BR_BillingSubscription");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "billing",
                table: "BR_BillingSubscription");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "billing",
                table: "B_BillingAccount");

            migrationBuilder.AddColumn<int>(
                name: "AccessProfileId",
                schema: "billing",
                table: "S_BillingPlan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BillingIntervalId",
                schema: "billing",
                table: "S_BillingPlan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "billing",
                table: "S_BillingPlan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccessProfileSourceId",
                schema: "profiling",
                table: "BR_UserAccessProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BillingSubscriptionId",
                schema: "profiling",
                table: "BR_UserAccessProfile",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                schema: "profiling",
                table: "BR_UserAccessProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTillUtc",
                schema: "profiling",
                table: "BR_UserAccessProfile",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                schema: "billing",
                table: "BR_BillingSubscription",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                schema: "billing",
                table: "B_BillingAccount",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseJson",
                schema: "auth",
                table: "B_AuthRegistrationIdempotency",
                type: "nvarchar(max)",
                maxLength: 12000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(12000)",
                oldMaxLength: 12000);

            migrationBuilder.CreateTable(
                name: "B_AccessProfileRevision",
                schema: "profiling",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RevisionKey = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BillingSubscriptionId = table.Column<int>(type: "int", nullable: false),
                    BillingPlanCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccessProfileId = table.Column<int>(type: "int", nullable: false),
                    RequestedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B_AccessProfileRevision", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_AccessProfileSource",
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
                    table.PrimaryKey("PK_S_AccessProfileSource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_BillingAccountStatus",
                schema: "billing",
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
                    table.PrimaryKey("PK_S_BillingAccountStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_BillingInterval",
                schema: "billing",
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
                    table.PrimaryKey("PK_S_BillingInterval", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_BillingSubscriptionStatus",
                schema: "billing",
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
                    table.PrimaryKey("PK_S_BillingSubscriptionStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_Currency",
                schema: "billing",
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
                    table.PrimaryKey("PK_S_Currency", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_S_BillingPlan_AccessProfileId",
                schema: "billing",
                table: "S_BillingPlan",
                column: "AccessProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_S_BillingPlan_BillingIntervalId",
                schema: "billing",
                table: "S_BillingPlan",
                column: "BillingIntervalId");

            migrationBuilder.CreateIndex(
                name: "IX_S_BillingPlan_CurrencyId",
                schema: "billing",
                table: "S_BillingPlan",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_UserAccessProfile_AccessProfileSourceId",
                schema: "profiling",
                table: "BR_UserAccessProfile",
                column: "AccessProfileSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_BR_UserAccessProfile_UserId_AccessProfileId_BillingSubscriptionId",
                schema: "profiling",
                table: "BR_UserAccessProfile",
                columns: new[] { "UserId", "AccessProfileId", "BillingSubscriptionId" },
                unique: true,
                filter: "[BillingSubscriptionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BR_UserAccessProfile_UserId_Enabled_ValidTillUtc",
                schema: "profiling",
                table: "BR_UserAccessProfile",
                columns: new[] { "UserId", "Enabled", "ValidTillUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BR_BillingSubscription_StatusId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_B_BillingAccount_StatusId",
                schema: "billing",
                table: "B_BillingAccount",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_B_AccessProfileRevision_ProcessedOnUtc_RequestedOnUtc",
                schema: "profiling",
                table: "B_AccessProfileRevision",
                columns: new[] { "ProcessedOnUtc", "RequestedOnUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_B_AccessProfileRevision_RevisionKey",
                schema: "profiling",
                table: "B_AccessProfileRevision",
                column: "RevisionKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_AccessProfileSource_Code",
                schema: "profiling",
                table: "S_AccessProfileSource",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_AccessProfileSource_Guid",
                schema: "profiling",
                table: "S_AccessProfileSource",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_BillingAccountStatus_Code",
                schema: "billing",
                table: "S_BillingAccountStatus",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_BillingAccountStatus_Guid",
                schema: "billing",
                table: "S_BillingAccountStatus",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_BillingInterval_Code",
                schema: "billing",
                table: "S_BillingInterval",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_BillingInterval_Guid",
                schema: "billing",
                table: "S_BillingInterval",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_BillingSubscriptionStatus_Code",
                schema: "billing",
                table: "S_BillingSubscriptionStatus",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_BillingSubscriptionStatus_Guid",
                schema: "billing",
                table: "S_BillingSubscriptionStatus",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_Currency_Code",
                schema: "billing",
                table: "S_Currency",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_Currency_Guid",
                schema: "billing",
                table: "S_Currency",
                column: "Guid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_B_BillingAccount_S_BillingAccountStatus_StatusId",
                schema: "billing",
                table: "B_BillingAccount",
                column: "StatusId",
                principalSchema: "billing",
                principalTable: "S_BillingAccountStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BR_BillingSubscription_S_BillingSubscriptionStatus_StatusId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "StatusId",
                principalSchema: "billing",
                principalTable: "S_BillingSubscriptionStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BR_UserAccessProfile_S_AccessProfileSource_AccessProfileSourceId",
                schema: "profiling",
                table: "BR_UserAccessProfile",
                column: "AccessProfileSourceId",
                principalSchema: "profiling",
                principalTable: "S_AccessProfileSource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_S_BillingPlan_S_AccessProfile_AccessProfileId",
                schema: "billing",
                table: "S_BillingPlan",
                column: "AccessProfileId",
                principalSchema: "profiling",
                principalTable: "S_AccessProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_S_BillingPlan_S_BillingInterval_BillingIntervalId",
                schema: "billing",
                table: "S_BillingPlan",
                column: "BillingIntervalId",
                principalSchema: "billing",
                principalTable: "S_BillingInterval",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_S_BillingPlan_S_Currency_CurrencyId",
                schema: "billing",
                table: "S_BillingPlan",
                column: "CurrencyId",
                principalSchema: "billing",
                principalTable: "S_Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_B_BillingAccount_S_BillingAccountStatus_StatusId",
                schema: "billing",
                table: "B_BillingAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_BR_BillingSubscription_S_BillingSubscriptionStatus_StatusId",
                schema: "billing",
                table: "BR_BillingSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_BR_UserAccessProfile_S_AccessProfileSource_AccessProfileSourceId",
                schema: "profiling",
                table: "BR_UserAccessProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_S_BillingPlan_S_AccessProfile_AccessProfileId",
                schema: "billing",
                table: "S_BillingPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_S_BillingPlan_S_BillingInterval_BillingIntervalId",
                schema: "billing",
                table: "S_BillingPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_S_BillingPlan_S_Currency_CurrencyId",
                schema: "billing",
                table: "S_BillingPlan");

            migrationBuilder.DropTable(
                name: "B_AccessProfileRevision",
                schema: "profiling");

            migrationBuilder.DropTable(
                name: "S_AccessProfileSource",
                schema: "profiling");

            migrationBuilder.DropTable(
                name: "S_BillingAccountStatus",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "S_BillingInterval",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "S_BillingSubscriptionStatus",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "S_Currency",
                schema: "billing");

            migrationBuilder.DropIndex(
                name: "IX_S_BillingPlan_AccessProfileId",
                schema: "billing",
                table: "S_BillingPlan");

            migrationBuilder.DropIndex(
                name: "IX_S_BillingPlan_BillingIntervalId",
                schema: "billing",
                table: "S_BillingPlan");

            migrationBuilder.DropIndex(
                name: "IX_S_BillingPlan_CurrencyId",
                schema: "billing",
                table: "S_BillingPlan");

            migrationBuilder.DropIndex(
                name: "IX_BR_UserAccessProfile_AccessProfileSourceId",
                schema: "profiling",
                table: "BR_UserAccessProfile");

            migrationBuilder.DropIndex(
                name: "IX_BR_UserAccessProfile_UserId_AccessProfileId_BillingSubscriptionId",
                schema: "profiling",
                table: "BR_UserAccessProfile");

            migrationBuilder.DropIndex(
                name: "IX_BR_UserAccessProfile_UserId_Enabled_ValidTillUtc",
                schema: "profiling",
                table: "BR_UserAccessProfile");

            migrationBuilder.DropIndex(
                name: "IX_BR_BillingSubscription_StatusId",
                schema: "billing",
                table: "BR_BillingSubscription");

            migrationBuilder.DropIndex(
                name: "IX_B_BillingAccount_StatusId",
                schema: "billing",
                table: "B_BillingAccount");

            migrationBuilder.DropColumn(
                name: "AccessProfileId",
                schema: "billing",
                table: "S_BillingPlan");

            migrationBuilder.DropColumn(
                name: "BillingIntervalId",
                schema: "billing",
                table: "S_BillingPlan");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "billing",
                table: "S_BillingPlan");

            migrationBuilder.DropColumn(
                name: "AccessProfileSourceId",
                schema: "profiling",
                table: "BR_UserAccessProfile");

            migrationBuilder.DropColumn(
                name: "BillingSubscriptionId",
                schema: "profiling",
                table: "BR_UserAccessProfile");

            migrationBuilder.DropColumn(
                name: "Enabled",
                schema: "profiling",
                table: "BR_UserAccessProfile");

            migrationBuilder.DropColumn(
                name: "ValidTillUtc",
                schema: "profiling",
                table: "BR_UserAccessProfile");

            migrationBuilder.DropColumn(
                name: "StatusId",
                schema: "billing",
                table: "BR_BillingSubscription");

            migrationBuilder.DropColumn(
                name: "StatusId",
                schema: "billing",
                table: "B_BillingAccount");

            migrationBuilder.AddColumn<string>(
                name: "BillingInterval",
                schema: "billing",
                table: "S_BillingPlan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "billing",
                table: "S_BillingPlan",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AccessProfileId",
                schema: "billing",
                table: "BR_BillingSubscription",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "billing",
                table: "BR_BillingSubscription",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "billing",
                table: "B_BillingAccount",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ResponseJson",
                schema: "auth",
                table: "B_AuthRegistrationIdempotency",
                type: "nvarchar(12000)",
                maxLength: 12000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 12000);

            migrationBuilder.CreateTable(
                name: "B_UserEntitlement",
                schema: "profiling",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessProfileId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    App_CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    App_CreateUserId = table.Column<int>(type: "int", nullable: true),
                    App_UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    App_UpdateUserId = table.Column<int>(type: "int", nullable: true),
                    App_Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    BillingSubscriptionId = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_B_UserEntitlement_B_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "profiling",
                        principalTable: "B_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_B_UserEntitlement_S_AccessProfile_AccessProfileId",
                        column: x => x.AccessProfileId,
                        principalSchema: "profiling",
                        principalTable: "S_AccessProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BR_UserAccessProfile_UserId_AccessProfileId",
                schema: "profiling",
                table: "BR_UserAccessProfile",
                columns: new[] { "UserId", "AccessProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BR_BillingSubscription_AccessProfileId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "AccessProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_B_UserEntitlement_AccessProfileId",
                schema: "profiling",
                table: "B_UserEntitlement",
                column: "AccessProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_B_UserEntitlement_BillingSubscriptionId",
                schema: "profiling",
                table: "B_UserEntitlement",
                column: "BillingSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_B_UserEntitlement_UserId_BillingSubscriptionId_AccessProfileId",
                schema: "profiling",
                table: "B_UserEntitlement",
                columns: new[] { "UserId", "BillingSubscriptionId", "AccessProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_B_UserEntitlement_UserId_Enabled_ExpiresAtUtc",
                schema: "profiling",
                table: "B_UserEntitlement",
                columns: new[] { "UserId", "Enabled", "ExpiresAtUtc" });

            migrationBuilder.AddForeignKey(
                name: "FK_BR_BillingSubscription_S_AccessProfile_AccessProfileId",
                schema: "billing",
                table: "BR_BillingSubscription",
                column: "AccessProfileId",
                principalSchema: "profiling",
                principalTable: "S_AccessProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
