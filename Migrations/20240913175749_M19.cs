using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Migrations
{
    /// <inheritdoc />
    public partial class M19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_ServicoSubCategorias_ServicoSubCategoriaId",
                table: "Agendamentos");

            migrationBuilder.AddColumn<string>(
                name: "Senha",
                table: "Usuarios",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_ServicoSubCategorias_ServicoSubCategoriaId",
                table: "Agendamentos",
                column: "ServicoSubCategoriaId",
                principalTable: "ServicoSubCategorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_ServicoSubCategorias_ServicoSubCategoriaId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "Senha",
                table: "Usuarios");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_ServicoSubCategorias_ServicoSubCategoriaId",
                table: "Agendamentos",
                column: "ServicoSubCategoriaId",
                principalTable: "ServicoSubCategorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
