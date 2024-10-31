using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceCallSystem6.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueType",
                table: "MaintenanceCalls");

            migrationBuilder.AddColumn<string>(
                name: "IssueTypeEnum",
                table: "MaintenanceCalls",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueTypeEnum",
                table: "MaintenanceCalls");

            migrationBuilder.AddColumn<string>(
                name: "IssueType",
                table: "MaintenanceCalls",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
