using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseComparison.Data.Migrations.MySql
{
    public partial class MySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "heart_rate_monitors",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    registered_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    average = table.Column<int>(type: "int", nullable: false),
                    max = table.Column<int>(type: "int", nullable: false),
                    min = table.Column<int>(type: "int", nullable: false),
                    smart_watch_id = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_heart_rate_monitors", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_heart_rate_monitors_registered_at",
                table: "heart_rate_monitors",
                column: "registered_at");

            migrationBuilder.CreateIndex(
                name: "ix_heart_rate_monitors_smart_watch_id",
                table: "heart_rate_monitors",
                column: "smart_watch_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "heart_rate_monitors");
        }
    }
}
