using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMTools.Web.Data.Database.Migrations
{
    public partial class Migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Alert",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimestampOfDeactivation",
                table: "Alert",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alert_SysDeleted",
                table: "Alert",
                column: "SysDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Alert_SysDeleted",
                table: "Alert");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Alert");

            migrationBuilder.DropColumn(
                name: "TimestampOfDeactivation",
                table: "Alert");
        }
    }
}
