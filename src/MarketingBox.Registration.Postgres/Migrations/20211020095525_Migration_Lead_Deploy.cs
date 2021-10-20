using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class Migration_Lead_Deploy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "lead-service");

            migrationBuilder.CreateTable(
                name: "leadidgenerator",
                schema: "lead-service",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    GeneratorId = table.Column<string>(type: "text", nullable: false),
                    LeadId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leadidgenerator", x => new { x.TenantId, x.GeneratorId });
                });

            migrationBuilder.CreateTable(
                name: "leads",
                schema: "lead-service",
                columns: table => new
                {
                    LeadId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Ip = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    RouteInfoAffiliateId = table.Column<long>(type: "bigint", nullable: false),
                    RouteInfoCampaignId = table.Column<long>(type: "bigint", nullable: false),
                    RouteInfoBoxId = table.Column<long>(type: "bigint", nullable: false),
                    RouteInfoBrand = table.Column<string>(type: "text", nullable: true),
                    RouteInfoBrandId = table.Column<long>(type: "bigint", nullable: false),
                    AdditionalInfoSo = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub1 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub2 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub3 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub4 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub5 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub6 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub7 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub8 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub9 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub10 = table.Column<string>(type: "text", nullable: true),
                    CustomerInfoCustomerId = table.Column<string>(type: "text", nullable: true),
                    CustomerInfoToken = table.Column<string>(type: "text", nullable: true),
                    CustomerInfoLoginUrl = table.Column<string>(type: "text", nullable: true),
                    CustomerInfoBrand = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CrmStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DepositDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ConversionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leads", x => x.LeadId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_leads_TenantId_LeadId",
                schema: "lead-service",
                table: "leads",
                columns: new[] { "TenantId", "LeadId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "leadidgenerator",
                schema: "lead-service");

            migrationBuilder.DropTable(
                name: "leads",
                schema: "lead-service");
        }
    }
}
