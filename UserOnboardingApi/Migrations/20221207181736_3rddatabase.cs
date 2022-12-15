using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserOnboardingApi.Migrations
{
    /// <inheritdoc />
    public partial class _3rddatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
