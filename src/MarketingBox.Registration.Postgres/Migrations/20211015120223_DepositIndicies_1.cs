using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class DepositIndicies_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ConvertionDate",
                schema: "lead-service",
                table: "deposits",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_deposits_AffiliateId_LeadId",
                schema: "lead-service",
                table: "deposits",
                columns: new[] { "AffiliateId", "LeadId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_deposits_AffiliateId_LeadId",
                schema: "lead-service",
                table: "deposits");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ConvertionDate",
                schema: "lead-service",
                table: "deposits",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
