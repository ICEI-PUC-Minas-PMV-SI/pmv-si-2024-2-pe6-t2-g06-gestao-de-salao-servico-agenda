using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data
{
    public class ApplicationDbContext : DbContext

    {
        // Construtor - Recebe DbContextOptions (Injecao de dependencia) e para option para base "classe pai = DbContext"
        // Abaixo todas as configuracoes do DbContext (Injecao de dependencia)
        // Execucao dessa configuracao sera na classe "Program.cs"
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {                
        }

        // Configuracao dos meus objetos - tabelas (entidades) que vao ser criadas no banco de dados
        public DbSet<Especializacao> Especializacoes { get; set; }
        public DbSet<Profissional> Profissionais { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }


    }
}
