using Microsoft.EntityFrameworkCore.Migrations;

namespace Haengma.GS.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BlackPlayer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhitePlayer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoardSize = table.Column<int>(type: "int", nullable: false),
                    Komi = table.Column<double>(type: "float", nullable: false),
                    Handicap = table.Column<int>(type: "int", nullable: false),
                    TimeSettingsType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainTimeInSeconds = table.Column<int>(type: "int", nullable: false),
                    ByoYomiPeriods = table.Column<int>(type: "int", nullable: false),
                    ByoYomiSeconds = table.Column<int>(type: "int", nullable: false),
                    Sgf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorDecision = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
