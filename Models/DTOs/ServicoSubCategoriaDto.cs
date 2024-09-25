using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.DTOs
{
    public class ServicoSubCategoriaDto
    {
        [JsonPropertyOrder(1)]
        public new int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        public TimeSpan Duracao { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Valor { get; set; }

        //Relacionamento virtual - FK
        [Required]
        public int ServicoCategoriaId { get; set; }
        public ServicoCategoria ServicoCategoria { get; set; }
    }
}
