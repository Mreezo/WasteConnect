using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasteConnect.Migrations
{
    /// <inheritdoc />
    public partial class AddCouncillorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccountActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PositionTitle",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardNumber",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccountActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PositionTitle",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WardNumber",
                table: "AspNetUsers");
        }
    }
}
