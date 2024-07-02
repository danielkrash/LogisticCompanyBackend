using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticCompany.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOfice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_office_address",
                table: "office",
                column: "address",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_office_address",
                table: "office");
        }
    }
}
