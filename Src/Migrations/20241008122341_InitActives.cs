using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Src.Migrations
{
    /// <inheritdoc />
    public partial class InitActives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ad48c91-0986-4e31-a1cf-514d489d6e95");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a6e8fcde-c05e-48b7-a672-316785c5750c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e4413de9-3433-4e30-9c5c-9c79567981ae");

            migrationBuilder.CreateTable(
                name: "Actives",
                columns: table => new
                {
                    ActivesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actives", x => x.ActivesID);
                    table.ForeignKey(
                        name: "FK_Actives_AspNetUsers_AppUserID",
                        column: x => x.AppUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4db48814-924c-44f4-8f0b-2b9cdf5f3432", null, "Admin", "ADMIN" },
                    { "6ff47899-4cba-4ecc-8476-5b80f38ad8ca", null, "Employee", "EMPLOYEE" },
                    { "d8497fa3-691a-4a39-ba96-f2e3d718ac19", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actives_AppUserID",
                table: "Actives",
                column: "AppUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actives");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4db48814-924c-44f4-8f0b-2b9cdf5f3432");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6ff47899-4cba-4ecc-8476-5b80f38ad8ca");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d8497fa3-691a-4a39-ba96-f2e3d718ac19");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1ad48c91-0986-4e31-a1cf-514d489d6e95", null, "Employee", "EMPLOYEE" },
                    { "a6e8fcde-c05e-48b7-a672-316785c5750c", null, "User", "USER" },
                    { "e4413de9-3433-4e30-9c5c-9c79567981ae", null, "Admin", "ADMIN" }
                });
        }
    }
}
