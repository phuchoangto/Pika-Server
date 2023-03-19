using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PikaServer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Passbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PassbookBalance",
                table: "Accounts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassbookBalance",
                table: "Accounts");
        }
    }
}
