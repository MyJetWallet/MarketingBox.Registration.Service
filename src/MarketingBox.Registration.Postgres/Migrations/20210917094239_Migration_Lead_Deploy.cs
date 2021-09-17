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
                name: "leads",
                schema: "lead-service",
                columns: table => new
                {
                    LeadId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Ip = table.Column<string>(type: "text", nullable: true),
                    GeneralInfo_AffiliateId = table.Column<long>(type: "bigint", nullable: true),
                    GeneralInfo_CampaignId = table.Column<long>(type: "bigint", nullable: true),
                    GeneralInfo_BoxId = table.Column<long>(type: "bigint", nullable: true),
                    AdditionalInfo_So = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_Sub = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_MPC_1 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_MPC_2 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_MPC_3 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_MPC_4 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_MPC_5 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_MPC_6 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_MPC_7 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_MPC_8 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_MPC_9 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo_MPC_10 = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
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
                name: "leads",
                schema: "lead-service");
        }
    }
}
