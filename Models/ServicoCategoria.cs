using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    public class ServicoCategoria
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required]
        public string Categoria { get; set; }

        // Relacionamento virtual para agendamentos para carregar informacoes do profissional associados a essa especializacao

        //[Column(TypeName="decimal(18,2)")] -- exemplo de configuracao de decimais
        //FK
        [Required]
        public int ServicoSubCategoriaId { get; set; } // -1

        // Relacionamento virtual para profissional para carregar informacoes do agendamento associados a esse profissional
        public ServicoSubCategoria SubCategoria { get; set; }

    }
}
