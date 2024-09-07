using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    public class ServicoSubCategoria
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        public TimeSpan Duracao { get; set; }

        //Relacionamento virtual - FK
        [Required]
        public int ServicoCategoriaId { get; set; }
        public ServicoCategoria ServicoCategoria { get; set; }

        // Relacionamento com agendamentos
        public ICollection<Agendamento> Agendamentos { get; set; }
    }
}
