using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    public class Especializacao
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required]
        [DisplayName("Especialização")]
        public string TipoEspecializacao { get; set; }

        // Relacionamento virtual para agendamentos para carregar informacoes do profissional associados a essa especializacao
        public ICollection<Profissional> Profissionais { get; set; }// -n
    }
}
