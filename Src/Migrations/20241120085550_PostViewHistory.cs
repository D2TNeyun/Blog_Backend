using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Src.Migrations
{
    /// <inheritdoc />
    public partial class PostViewHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_pageViews",
                table: "pageViews");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "255215df-d4ab-41bb-a4ed-55c0896dd57b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "526fe916-4e73-44ec-9e8f-a97fa919758c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dd4b8f4f-71f8-4d1a-8584-902eb7b0b0c6");

            migrationBuilder.RenameTable(
                name: "pageViews",
                newName: "PageViews");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PageViews",
                table: "PageViews",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PostViewHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostViewHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostViewHistories_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PostViewHistories_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "41c1a612-30e6-4744-a906-1c7cb20652d8", null, "Admin", "ADMIN" },
                    { "41c5659d-73ee-40d4-85ff-41c50c10e1c4", null, "Employee", "EMPLOYEE" },
                    { "bca89461-0cc1-4786-bde1-940e60aa3e18", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostViewHistories_AppUserId",
                table: "PostViewHistories",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostViewHistories_PostId",
                table: "PostViewHistories",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostViewHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PageViews",
                table: "PageViews");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "41c1a612-30e6-4744-a906-1c7cb20652d8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "41c5659d-73ee-40d4-85ff-41c50c10e1c4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bca89461-0cc1-4786-bde1-940e60aa3e18");

            migrationBuilder.RenameTable(
                name: "PageViews",
                newName: "pageViews");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pageViews",
                table: "pageViews",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "255215df-d4ab-41bb-a4ed-55c0896dd57b", null, "User", "USER" },
                    { "526fe916-4e73-44ec-9e8f-a97fa919758c", null, "Admin", "ADMIN" },
                    { "dd4b8f4f-71f8-4d1a-8584-902eb7b0b0c6", null, "Employee", "EMPLOYEE" }
                });
        }
    }
}
