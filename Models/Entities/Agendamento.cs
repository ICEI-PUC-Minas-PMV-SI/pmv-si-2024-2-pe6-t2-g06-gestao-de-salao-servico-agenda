using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.DTOs;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities
{
    [Table("Agendamentos")]
    public class Agendamento : BaseEntity 

    {
        //[Key]
        //[ScaffoldColumn(false)]
        //public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataAgendamento { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan HoraAgendamento { get; set; }

        // Status do agendamento (Pendente, Confirmado, Concluído, Cancelado)
        [Required]
        public string Status { get; set; } = "Pendente";

        [MaxLength(500)]
        public string Observacoes { get; set; }

        // FK para ServicoCategoria
        [Required]
        public int ServicoCategoriaId { get; set; }

        [ForeignKey("ServicoCategoriaId")]
        public ServicoCategoria ServicoCategoria { get; set; }

        // FK para ServicoSubCategoria
        [Required]
        public int ServicoSubCategoriaId { get; set; }

        [ForeignKey("ServicoSubCategoriaId")]
        public ServicoSubCategoria ServicoSubCategoria { get; set; }

        // FK for Usuario (Cliente)
        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        [InverseProperty("AgendamentosComoCliente")]
        public Usuario Usuario { get; set; }

        // FK for Usuario (Profissional)
        [Required]
        public int ProfissionalId { get; set; }

        [ForeignKey("ProfissionalId")]
        [InverseProperty("AgendamentosComoProfissional")]
        public Usuario Profissional { get; set; }


        //public ICollection<AgendamentoUsuarios> Usuarios { get; set; }
    }
}
