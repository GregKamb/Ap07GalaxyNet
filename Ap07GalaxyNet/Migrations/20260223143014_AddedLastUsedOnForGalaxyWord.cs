using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ap07GalaxyNet.Migrations
{
    /// <inheritdoc />
    public partial class AddedLastUsedOnForGalaxyWord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedOn",
                table: "GalaxyWords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUsedOn",
                table: "GalaxyWords");
        }
    }
}
