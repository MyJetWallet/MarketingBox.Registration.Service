using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class Migration_Lead_Deploy_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "lead-service",
                table: "leads",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<long>(
                name: "BrandInfo_BrandId",
                schema: "lead-service",
                table: "leads",
                type: "bigint",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_deposits_TenantId_DepositId",
                schema: "lead-service",
                table: "deposits",
                columns: new[] { "TenantId", "DepositId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deposits",
                schema: "lead-service");

            migrationBuilder.DropColumn(
                name: "BrandInfo_BrandId",
                schema: "lead-service",
                table: "leads");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "lead-service",
                table: "leads",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");
        }
    }
}
