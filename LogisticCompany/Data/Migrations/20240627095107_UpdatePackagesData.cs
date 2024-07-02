using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticCompany.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePackagesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "price",
                table: "package",
                type: "numeric(12,4)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price",
                table: "package");
        }
    }
}
