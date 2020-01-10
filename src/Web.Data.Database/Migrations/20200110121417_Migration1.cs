using Microsoft.EntityFrameworkCore.Migrations;

namespace AMTools.Web.Data.Database.Migrations
{
    public partial class Migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Reihenfolge",
                table: "Subscriber",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reihenfolge",
                table: "Subscriber");
        }
    }
}
