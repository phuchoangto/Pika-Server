using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PikaServer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FCM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Accounts");
        }
    }
}
