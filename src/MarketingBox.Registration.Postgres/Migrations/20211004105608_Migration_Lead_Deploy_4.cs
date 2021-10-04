using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class Migration_Lead_Deploy_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrandRegistrationInfo_BrandResponse",
                schema: "lead-service",
                table: "leads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandRegistrationInfo_CustomerId",
                schema: "lead-service",
                table: "leads",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandRegistrationInfo_BrandResponse",
                schema: "lead-service",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "BrandRegistrationInfo_CustomerId",
                schema: "lead-service",
                table: "leads");
        }
    }
}
