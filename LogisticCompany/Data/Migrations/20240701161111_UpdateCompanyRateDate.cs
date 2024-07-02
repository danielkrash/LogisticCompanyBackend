using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticCompany.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompanyRateDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "date",
                table: "company_revenue",
                type: "date",
                nullable: false,
                defaultValueSql: "CURRENT_DATE",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_DATE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date",
                table: "company_revenue",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_DATE",
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValueSql: "CURRENT_DATE");
        }
    }
}
