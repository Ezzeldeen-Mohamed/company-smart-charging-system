using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanySmartChargingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addIsNewFieldToRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isNew",
                table: "RefreshToken",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isNew",
                table: "RefreshToken");
        }
    }
}
