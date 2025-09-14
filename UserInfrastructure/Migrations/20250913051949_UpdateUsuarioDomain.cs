using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsuarioDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Domicilio_Usuarios_UsuarioID", // <-- nombre actual en tu BD
                table: "Domicilio");

            migrationBuilder.DropIndex(
                name: "IX_Domicilio_UsuarioID",
                table: "Domicilio");

            migrationBuilder.CreateIndex(
                name: "IX_Domicilio_UsuarioID",
                table: "Domicilio",
                column: "UsuarioID");

            migrationBuilder.AddForeignKey(
                name: "FK_Domicilio_Usuarios_UsuarioID",
                table: "Domicilio",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
               name: "FK_Domicilio_Usuarios_UsuarioID",
               table: "Domicilio");


            migrationBuilder.DropIndex(
                name: "IX_Domicilio_UsuarioID",
                table: "Domicilio");

            migrationBuilder.CreateIndex(
                name: "IX_Domicilio_UsuarioID",
                table: "Domicilio",
                column: "UsuarioID",
                unique: true);
        }
    }
}
