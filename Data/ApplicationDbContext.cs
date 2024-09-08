using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;
using System.Reflection.Emit;

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

        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Profissional> Profissionais { get; set; }
        public DbSet<ServicoCategoria> ServicoCategorias { get; set; }
        public DbSet<ServicoSubCategoria> ServicoSubCategorias { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração de Agendamento
            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.Agendamentos)
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Profissional)
                .WithMany(p => p.Agendamentos)
                .HasForeignKey(a => a.ProfissionalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.ServicoCategoria)
                .WithMany(sc => sc.Agendamentos)
                .HasForeignKey(a => a.ServicoCategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.ServicoSubCategoria)
                .WithMany(ssc => ssc.Agendamentos)
                .HasForeignKey(a => a.ServicoSubCategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração de precisão para a propriedade decimal "Valor" em ServicoSubCategoria
            modelBuilder.Entity<ServicoSubCategoria>()
                .Property(ssc => ssc.Valor)
                .HasPrecision(18, 2);
        }
    }
}

