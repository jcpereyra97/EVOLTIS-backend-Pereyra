using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertiesToDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaActualizacion",
                table: "Usuarios",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Domicilio",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaActualizacion",
                table: "Domicilio",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaUltimaActualizacion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Domicilio");

            migrationBuilder.DropColumn(
                name: "FechaUltimaActualizacion",
                table: "Domicilio");
        }
    }
}
