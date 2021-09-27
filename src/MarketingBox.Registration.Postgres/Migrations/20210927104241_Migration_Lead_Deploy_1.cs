using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class Migration_Lead_Deploy_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Sequence",
                schema: "lead-service",
                table: "leads",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UniqueId",
                schema: "lead-service",
                table: "leads",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                schema: "lead-service",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "UniqueId",
                schema: "lead-service",
                table: "leads");
        }
    }
}
