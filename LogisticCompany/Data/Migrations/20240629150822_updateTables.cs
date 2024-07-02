using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticCompany.Migrations
{
    /// <inheritdoc />
    public partial class updateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "package_receiver_employee_id_fk",
                table: "package");

            migrationBuilder.DropIndex(
                name: "IX_package_receiver_employee_id",
                table: "package");

            migrationBuilder.DropColumn(
                name: "receiver_employee_id",
                table: "package");

            migrationBuilder.RenameColumn(
                name: "sender_employee_id",
                table: "package",
                newName: "registrar_employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_package_sender_employee_id",
                table: "package",
                newName: "IX_package_registrar_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "registrar_employee_id",
                table: "package",
                newName: "sender_employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_package_registrar_employee_id",
                table: "package",
                newName: "IX_package_sender_employee_id");

            migrationBuilder.AddColumn<Guid>(
                name: "receiver_employee_id",
                table: "package",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_package_receiver_employee_id",
                table: "package",
                column: "receiver_employee_id");

            migrationBuilder.AddForeignKey(
                name: "package_receiver_employee_id_fk",
                table: "package",
                column: "receiver_employee_id",
                principalTable: "employee",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
