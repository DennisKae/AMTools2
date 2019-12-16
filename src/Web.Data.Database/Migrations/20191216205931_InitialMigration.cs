using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMTools.Web.Data.Database.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alert",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    AlertTimestamp = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    AlertedSubscribers = table.Column<string>(nullable: true),
                    Xml = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false, defaultValue: true),
                    TimestampOfDeactivation = table.Column<DateTime>(nullable: true),
                    SysStampIn = table.Column<DateTime>(nullable: false, defaultValueSql: "datetime('now','localtime')"),
                    SysStampUp = table.Column<DateTime>(nullable: true),
                    SysDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alert", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Severity = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    ApplicationPart = table.Column<string>(nullable: true),
                    BatchCommand = table.Column<string>(nullable: true),
                    SysDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TableName = table.Column<string>(nullable: true),
                    Operation = table.Column<string>(nullable: true),
                    KeyValues = table.Column<string>(nullable: true),
                    OldValues = table.Column<string>(nullable: true),
                    NewValues = table.Column<string>(nullable: true),
                    SysStampIn = table.Column<DateTime>(nullable: false, defaultValueSql: "datetime('now','localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AvailabilityStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Issi = table.Column<string>(nullable: true),
                    Value = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    SysStampIn = table.Column<DateTime>(nullable: false, defaultValueSql: "datetime('now','localtime')"),
                    SysStampUp = table.Column<DateTime>(nullable: true),
                    SysDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailabilityStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    SysStampIn = table.Column<DateTime>(nullable: false, defaultValueSql: "datetime('now','localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriber",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Issi = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Qualification = table.Column<string>(nullable: true),
                    SysStampIn = table.Column<DateTime>(nullable: false, defaultValueSql: "datetime('now','localtime')"),
                    SysStampUp = table.Column<DateTime>(nullable: true),
                    SysDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriber", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserResponse",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlertId = table.Column<int>(nullable: false),
                    Issi = table.Column<string>(nullable: true),
                    Accept = table.Column<bool>(nullable: false),
                    Color = table.Column<string>(nullable: true),
                    Response = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    SysStampIn = table.Column<DateTime>(nullable: false, defaultValueSql: "datetime('now','localtime')"),
                    SysDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResponse", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alert_Enabled",
                table: "Alert",
                column: "Enabled");

            migrationBuilder.CreateIndex(
                name: "IX_Alert_Number",
                table: "Alert",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Alert_SysDeleted",
                table: "Alert",
                column: "SysDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Alert_Timestamp",
                table: "Alert",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AppLog_ApplicationPart",
                table: "AppLog",
                column: "ApplicationPart");

            migrationBuilder.CreateIndex(
                name: "IX_AppLog_Timestamp",
                table: "AppLog",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_TableName",
                table: "AuditLog",
                column: "TableName");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityStatus_Issi",
                table: "AvailabilityStatus",
                column: "Issi");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriber_Issi",
                table: "Subscriber",
                column: "Issi");

            migrationBuilder.CreateIndex(
                name: "IX_UserResponse_AlertId",
                table: "UserResponse",
                column: "AlertId");

            migrationBuilder.CreateIndex(
                name: "IX_UserResponse_Issi",
                table: "UserResponse",
                column: "Issi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alert");

            migrationBuilder.DropTable(
                name: "AppLog");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "AvailabilityStatus");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "Subscriber");

            migrationBuilder.DropTable(
                name: "UserResponse");
        }
    }
}
