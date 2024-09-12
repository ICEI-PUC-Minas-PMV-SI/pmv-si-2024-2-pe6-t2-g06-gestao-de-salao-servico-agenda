using System.ComponentModel.DataAnnotations.Schema;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    [Table("AgendamentoUsuarios")]
    public class AgendamentoUsuarios
    {
        public int AgendamentoId { get; set; }
        public Agendamento Agendamento { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario{ get; set; }
    }
}
