using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LogisticCompany.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePackagesStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "package_courier_id_fk",
                table: "package");

            migrationBuilder.DropForeignKey(
                name: "package_receiver_employee_id_fk",
                table: "package");

            migrationBuilder.DropForeignKey(
                name: "package_receiver_id_fk",
                table: "package");

            migrationBuilder.DropForeignKey(
                name: "package_sender_employee_id_fk",
                table: "package");

            migrationBuilder.DropForeignKey(
                name: "package_sender_id_fk",
                table: "package");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "package",
                newName: "company_name");

            migrationBuilder.AddColumn<int>(
                name: "package_status_id",
                table: "package",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "fragile_rate",
                table: "company_rate",
                type: "numeric(12,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "hazardous_rate",
                table: "company_rate",
                type: "numeric(12,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "PackageStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_package_package_status_id",
                table: "package",
                column: "package_status_id");

            migrationBuilder.AddForeignKey(
                name: "package_courier_id_fk",
                table: "package",
                column: "courier_id",
                principalTable: "employee",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "package_receiver_employee_id_fk",
                table: "package",
                column: "receiver_employee_id",
                principalTable: "employee",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "package_receiver_id_fk",
                table: "package",
                column: "receiver_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "package_sender_employee_id_fk",
                table: "package",
                column: "sender_employee_id",
                principalTable: "employee",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "package_sender_id_fk",
                table: "package",
                column: "sender_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "package_status_id_fk",
                table: "package",
                column: "package_status_id",
                principalTable: "PackageStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "package_courier_id_fk",
                table: "package");

            migrationBuilder.DropForeignKey(
                name: "package_receiver_employee_id_fk",
                table: "package");

            migrationBuilder.DropForeignKey(
                name: "package_receiver_id_fk",
                table: "package");

            migrationBuilder.DropForeignKey(
                name: "package_sender_employee_id_fk",
                table: "package");

            migrationBuilder.DropForeignKey(
                name: "package_sender_id_fk",
                table: "package");

            migrationBuilder.DropForeignKey(
                name: "package_status_id_fk",
                table: "package");

            migrationBuilder.DropTable(
                name: "PackageStatus");

            migrationBuilder.DropIndex(
                name: "IX_package_package_status_id",
                table: "package");

            migrationBuilder.DropColumn(
                name: "package_status_id",
                table: "package");

            migrationBuilder.DropColumn(
                name: "fragile_rate",
                table: "company_rate");

            migrationBuilder.DropColumn(
                name: "hazardous_rate",
                table: "company_rate");

            migrationBuilder.RenameColumn(
                name: "company_name",
                table: "package",
                newName: "status");

            migrationBuilder.AddForeignKey(
                name: "package_courier_id_fk",
                table: "package",
                column: "courier_id",
                principalTable: "employee",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "package_receiver_employee_id_fk",
                table: "package",
                column: "receiver_employee_id",
                principalTable: "employee",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "package_receiver_id_fk",
                table: "package",
                column: "receiver_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "package_sender_employee_id_fk",
                table: "package",
                column: "sender_employee_id",
                principalTable: "employee",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "package_sender_id_fk",
                table: "package",
                column: "sender_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
