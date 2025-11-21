using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG6212_ST10435542_POE.Migrations
{
    /// <inheritdoc />
    public partial class AddFileSizeToClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                schema: "Identity",
                table: "MonthlyClaims",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "Users",
                keyColumn: "Id",
                keyValue: "hr-user-id-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f9bdd5fb-b8bd-43c3-ba11-30189fd8de0d", "AQAAAAIAAYagAAAAEOKbWXIrCmLpdW7a3i9RJ3vwBh+axDlaU2geU0xMayVLoh9dgpA7hbTfBHaOdXBV7A==" });

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "Users",
                keyColumn: "Id",
                keyValue: "lecturer-user-id-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "50e61ff7-8c62-4896-8469-f093fe6f83cb", "AQAAAAIAAYagAAAAEPOeQFjfl28novYTeNOaaHBSAUgN+CsXVGJXlrMjfXksw9lx9OI+oZtmQE71tvOU7Q==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSize",
                schema: "Identity",
                table: "MonthlyClaims");

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "Users",
                keyColumn: "Id",
                keyValue: "hr-user-id-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "17dff33f-07ef-4eac-9228-d6863fa774c0", "AQAAAAIAAYagAAAAEKl5gdKLHQPrwWOR1/WE3VgFOgka45xzmrMm+O7fNBR9SC+xpsnZomLaDv/igccVJQ==" });

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "Users",
                keyColumn: "Id",
                keyValue: "lecturer-user-id-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "13f195a3-e2c9-4a39-85be-b28175098391", "AQAAAAIAAYagAAAAEKmnJbhAuhxweFuegKkK6kjeMwPytJM5sb4MyWD23r/qa6mhaDFqLcxxumthrAIF5A==" });
        }
    }
}
