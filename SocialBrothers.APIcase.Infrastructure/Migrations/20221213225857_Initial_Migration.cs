using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialBrothers.APIcase.Infrastructure.Migrations
{
    public partial class Initial_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Street = table.Column<string>(type: "TEXT", nullable: true),
                    HouseNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ZipCode = table.Column<int>(type: "INTEGER", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_City",
                table: "Addresses",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Country",
                table: "Addresses",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Street",
                table: "Addresses",
                column: "Street");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ZipCode",
                table: "Addresses",
                column: "ZipCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
