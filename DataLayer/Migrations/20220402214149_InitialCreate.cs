using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameResults",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PlayerHostName = table.Column<string>(type: "text", nullable: false),
                    PlayerHostHealth = table.Column<int>(type: "integer", nullable: false),
                    PlayerGuestName = table.Column<string>(type: "text", nullable: false),
                    PlayerGuestHealth = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameResults", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameResults");
        }
    }
}
