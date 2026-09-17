using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddChamadoClienteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClienteNome",
                table: "Chamados",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complemento",
                table: "Chamados",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Chamados",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Chamados",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClienteNome",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "Complemento",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Chamados");
        }
    }
}
