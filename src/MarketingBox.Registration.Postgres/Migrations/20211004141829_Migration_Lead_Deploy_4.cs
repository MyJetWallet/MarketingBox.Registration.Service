using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class Migration_Lead_Deploy_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "lead-service");

            migrationBuilder.CreateTable(
                name: "deposits",
                schema: "lead-service",
                columns: table => new
                {
                    DepositId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    AffiliateId = table.Column<long>(type: "bigint", nullable: false),
                    CampaignId = table.Column<long>(type: "bigint", nullable: false),
                    BoxId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposits", x => x.DepositId);
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
                    BrandRegistrationInfo_AffiliateId = table.Column<long>(type: "bigint", nullable: true),
                    BrandRegistrationInfo_CampaignId = table.Column<long>(type: "bigint", nullable: true),
                    BrandRegistrationInfo_BoxId = table.Column<long>(type: "bigint", nullable: true),
                    BrandRegistrationInfo_Brand = table.Column<string>(type: "text", nullable: true),
                    BrandRegistrationInfo_BrandId = table.Column<long>(type: "bigint", nullable: true),
                    BrandRegistrationInfo_CustomerId = table.Column<string>(type: "text", nullable: true),
                    BrandRegistrationInfo_BrandResponse = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_So = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub1 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub2 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub3 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub4 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub5 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub6 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub7 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub8 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub9 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub10 = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leads", x => x.LeadId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_deposits_TenantId_DepositId",
                schema: "lead-service",
                table: "deposits",
                columns: new[] { "TenantId", "DepositId" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_TenantId_LeadId",
                schema: "lead-service",
                table: "leads",
                columns: new[] { "TenantId", "LeadId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deposits",
                schema: "lead-service");

            migrationBuilder.DropTable(
                name: "leads",
                schema: "lead-service");
        }
    }
}
