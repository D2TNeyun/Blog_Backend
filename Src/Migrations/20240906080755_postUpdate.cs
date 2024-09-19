using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Src.Migrations
{
    /// <inheritdoc />
    public partial class postUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "11f75b96-eee9-443b-90b0-fb30132415df");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2ea47739-03af-41c0-9458-fbec2061dac9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a4546d5e-1ad7-4ba3-8aed-a79135cd5cfc");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "39bfc8b4-99a2-46a6-8186-d6f1725d34bc", null, "Employee", "EMPLOYEE" },
                    { "9d83db27-4400-4a51-bcb2-adec6c491beb", null, "Admin", "ADMIN" },
                    { "bc4a0b26-09da-494e-a8ec-c4830e036ce5", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39bfc8b4-99a2-46a6-8186-d6f1725d34bc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9d83db27-4400-4a51-bcb2-adec6c491beb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bc4a0b26-09da-494e-a8ec-c4830e036ce5");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Posts");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "11f75b96-eee9-443b-90b0-fb30132415df", null, "User", "USER" },
                    { "2ea47739-03af-41c0-9458-fbec2061dac9", null, "Admin", "ADMIN" },
                    { "a4546d5e-1ad7-4ba3-8aed-a79135cd5cfc", null, "Employee", "EMPLOYEE" }
                });
        }
    }
}
