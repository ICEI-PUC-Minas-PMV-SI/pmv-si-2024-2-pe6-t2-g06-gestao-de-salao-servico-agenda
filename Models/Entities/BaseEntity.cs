using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.DTOs;
using System.ComponentModel.DataAnnotations;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities
{
    public class BaseEntity : LinksHATEOS
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
    }
}
