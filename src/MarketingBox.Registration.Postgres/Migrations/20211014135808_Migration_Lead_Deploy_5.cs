using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class Migration_Lead_Deploy_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "lead-service",
                table: "leads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Approved",
                schema: "lead-service",
                table: "deposits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConvertionDate",
                schema: "lead-service",
                table: "deposits",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LeadId",
                schema: "lead-service",
                table: "deposits",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                schema: "lead-service",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "Approved",
                schema: "lead-service",
                table: "deposits");

            migrationBuilder.DropColumn(
                name: "ConvertionDate",
                schema: "lead-service",
                table: "deposits");

            migrationBuilder.DropColumn(
                name: "LeadId",
                schema: "lead-service",
                table: "deposits");
        }
    }
}
