using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace E_Clubs.Migrations
{
    /// <inheritdoc />
    public partial class Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "254cf5ba-461f-415c-ba6a-c8b5cac47e83", null, "Teacher", "TEACHER" },
                    { "770c020b-89d2-44d2-996b-a2a7c32ba4f8", null, "Director", "DIRECTOR" },
                    { "7fb4345e-a973-4ac8-97d0-2f74716766eb", null, "Admin", "ADMIN" },
                    { "b850cf54-0e5f-40df-bdec-db270f33a5c2", null, "Student", "STUDENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "254cf5ba-461f-415c-ba6a-c8b5cac47e83");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "770c020b-89d2-44d2-996b-a2a7c32ba4f8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fb4345e-a973-4ac8-97d0-2f74716766eb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b850cf54-0e5f-40df-bdec-db270f33a5c2");
        }
    }
}
