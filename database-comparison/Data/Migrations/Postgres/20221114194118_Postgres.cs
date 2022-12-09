﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseComparison.Data.Migrations.Postgres
{
    public partial class Postgres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "heart_rate_monitors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    registered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    average = table.Column<int>(type: "integer", nullable: false),
                    max = table.Column<int>(type: "integer", nullable: false),
                    min = table.Column<int>(type: "integer", nullable: false),
                    smart_watch_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_heart_rate_monitors", x => x.id);
                });

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
