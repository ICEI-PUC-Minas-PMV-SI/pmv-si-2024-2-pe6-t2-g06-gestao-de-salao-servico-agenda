using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.DTOs
{
    public class ServicoCategoriaDto
    {
        public int? Id { get; set; }  // Reescrevendo para garantir a ordem no JSON

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }
    }
}
