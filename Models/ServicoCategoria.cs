using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    [Table("ServicoCategorias")]
    public class ServicoCategoria
    {
        [Key]
        //[ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        // Relacionamento com subcategorias e agendamentos
        public ICollection<ServicoSubCategoria> ServicoSubCategorias { get; set; }
        public ICollection<Agendamento> Agendamentos { get; set; }

    }
}
