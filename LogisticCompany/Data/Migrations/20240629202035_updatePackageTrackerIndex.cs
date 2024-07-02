using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticCompany.Migrations
{
    /// <inheritdoc />
    public partial class updatePackageTrackerIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_package_package_status_id",
                table: "package");

            migrationBuilder.CreateIndex(
                name: "IX_package_package_status_id",
                table: "package",
                column: "package_status_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_package_package_status_id",
                table: "package");

            migrationBuilder.CreateIndex(
                name: "IX_package_package_status_id",
                table: "package",
                column: "package_status_id");
        }
    }
}
