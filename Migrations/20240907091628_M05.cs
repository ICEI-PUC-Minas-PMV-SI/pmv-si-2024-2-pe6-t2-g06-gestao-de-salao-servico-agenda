using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Migrations
{
    /// <inheritdoc />
    public partial class M05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServicoSubCategorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubCategoria = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicoSubCategorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServicoCategoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServicoSubCategoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicoCategoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicoCategoria_ServicoSubCategorias_ServicoSubCategoriaId",
                        column: x => x.ServicoSubCategoriaId,
                        principalTable: "ServicoSubCategorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profissionais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataNascimento = table.Column<DateOnly>(type: "date", maxLength: 10, nullable: false),
                    Genero = table.Column<int>(type: "int", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServicoCategoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profissionais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profissionais_ServicoCategoria_ServicoCategoriaId",
                        column: x => x.ServicoCategoriaId,
                        principalTable: "ServicoCategoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataAgendamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoraAgendamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfissionalId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ServicoSubCategoriaId = table.Column<int>(type: "int", nullable: false),
                    AgendamentoAtivo = table.Column<bool>(type: "bit", nullable: false),
                    DataCancelamento = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Profissionais_ProfissionalId",
                        column: x => x.ProfissionalId,
                        principalTable: "Profissionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ProfissionalId",
                table: "Agendamentos",
                column: "ProfissionalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profissionais_ServicoCategoriaId",
                table: "Profissionais",
                column: "ServicoCategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicoCategoria_ServicoSubCategoriaId",
                table: "ServicoCategoria",
                column: "ServicoSubCategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "Profissionais");

            migrationBuilder.DropTable(
                name: "ServicoCategoria");

            migrationBuilder.DropTable(
                name: "ServicoSubCategorias");
        }
    }
}
