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
        public string SubCategoria { get; set; }

        // Relacionamento virtual para agendamentos para carregar informacoes do profissional associados a essa especializacao
        //FK
    }
}
