using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Migrations
{
    /// <inheritdoc />
    public partial class M09 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duracao",
                table: "Agendamento");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duracao",
                table: "ServicoSubCategorias",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataNascimento",
                table: "Profissionais",
                type: "datetime2",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldMaxLength: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duracao",
                table: "ServicoSubCategorias");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DataNascimento",
                table: "Profissionais",
                type: "date",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duracao",
                table: "Agendamento",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
