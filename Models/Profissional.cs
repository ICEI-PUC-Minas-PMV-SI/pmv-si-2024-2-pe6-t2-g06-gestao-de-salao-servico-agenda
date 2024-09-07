using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    public class Profissional
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [Phone]
        public string Telefone { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Data De Nacimento deve conter apenas 10 digitos incluindo separadores")]
        [StringLength(10)]
        public DateTime DataNascimento { get; set; }

        [Required]
        public GeneroTipo Genero { get; set; }

        [Required]
        [MaxLength(200)]
        public string Especialidade { get; set; }

        [Required]
        public string Endereco { get; set; }

        [Required]
        public string Cidade { get; set; }

        [Required]
        public string Estado { get; set; }

        [Required]
        public string Cep { get; set; }

        // Relacionamentos
        public ICollection<Agendamento> Agendamentos { get; set; }

    }
}
