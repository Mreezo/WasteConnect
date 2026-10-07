using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasteConnect.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyRegistrationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyRegistrationCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyRegistrationCode",
                table: "AspNetUsers");
        }
    }
}
