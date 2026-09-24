using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioAccessFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoAcesso",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermissoesNav",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoAcesso",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PermissoesNav",
                table: "AspNetUsers");
        }
    }
}
