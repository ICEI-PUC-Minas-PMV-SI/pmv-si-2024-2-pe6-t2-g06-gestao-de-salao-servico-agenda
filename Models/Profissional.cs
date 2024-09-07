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
        public string Nome { get; set; }
        [Required]
        [DisplayName("Data de Nascimento")]
        [Range(0, 10, ErrorMessage = "Data De Nacimento deve conter apenas 10 digitos incluindo traços")]
        [StringLength(10)]
        public DateOnly DataNascimento { get; set; }
        [Required]
        [DisplayName("Gênero")]
        public GeneroTipo Genero { get; set; }
        [Required]
        public string Telefone { get; set; }
        [Required]
        [DisplayName("Endereço")]
        public string Endereco { get; set; }
        [Required]
        public string Cidade { get; set; }
        [Required]
        public string Estado { get; set; }
        [Required]
        public string Cep { get; set; }

        // um profissional pode ter 1 ou n especializacao - 1-n
        // FK
        [Required]
        public int ServicoCategoriaId { get; set; }

        // Relacionamento virtual para profissional para carregar informacoes da especializacao associados a esse profissional
        public ServicoCategoria ServicoCategoria { get; set; }// -1

        // um profissional possui varios agendamentos - 1-n
        // FK
        [Required]
        //public int AgendamentoId { get; set; } // -1

        // Relacionamento virtual para profissional para carregar informacoes do agendamento associados a esse profissional
        public Agendamento Agendamento { get; set; }


    }
}
