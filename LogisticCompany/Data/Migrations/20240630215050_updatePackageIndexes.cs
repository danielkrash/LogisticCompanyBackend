using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticCompany.Migrations
{
    /// <inheritdoc />
    public partial class updatePackageIndexes : Migration
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
                column: "package_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_package_tracker_number",
                table: "package",
                column: "tracker_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_package_package_status_id",
                table: "package");

            migrationBuilder.DropIndex(
                name: "IX_package_tracker_number",
                table: "package");

            migrationBuilder.CreateIndex(
                name: "IX_package_package_status_id",
                table: "package",
                column: "package_status_id",
                unique: true);
        }
    }
}
