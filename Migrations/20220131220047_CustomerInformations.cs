using Microsoft.EntityFrameworkCore.Migrations;

namespace FrontToBack.Migrations
{
    public partial class CustomerInformations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress",
                table: "Sales",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress2",
                table: "Sales",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Sales",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Sales",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSurname",
                table: "Sales",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerZipCode",
                table: "Sales",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CustomerAddress2",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CustomerSurname",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CustomerZipCode",
                table: "Sales");
        }
    }
}
