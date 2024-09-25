using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities
{
    [Table("ServicoCategorias")]
    public class ServicoCategoria //: BaseEntity
    {
        //// O campo Id é herdado de BaseEntity e o JsonPropertyOrder com define sua posição no JSON
        //[JsonPropertyOrder(1)]
        //public new int Id { get; set; }  // Reescrevendo para garantir a ordem no JSON
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        // Relacionamento com subcategorias e agendamentos
        [JsonIgnore]
        public ICollection<ServicoSubCategoria> ServicoSubCategorias { get; set; }
        [JsonIgnore]
        public ICollection<Agendamento> Agendamentos { get; set; }

    }
}
