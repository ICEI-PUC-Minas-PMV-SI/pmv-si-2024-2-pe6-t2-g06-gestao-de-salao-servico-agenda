using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    //  O modelo contém propriedades que representam as características de um usuario.
    //  O modelo é usado para passar dados na API Web e persistir opções de usuario no armazenamento de dados.

    // ------------
    // DataAnnotatios:
    // Required – Indicates that the property is a required field
    // DisplayName – Defines the text to use on form fields and validation messages
    // StringLength – Defines a maximum length for a string field
    // Range – Gives a maximum and minimum value for a numeric field
    // Bind – Lists fields to exclude or include when binding parameter or form values to model properties
    // ScaffoldColumn – Allows hiding fields from editor forms
    // [Bind(Exclude = "AlbumId")]
    // ------------

    // Fixa o nome da tabela a ser criada no banco de dados
    [Table("Usuario")]
    public class Usuario
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

        // Um usuario possui varios agendamentos - relacionamento 1 - n
        [Required]
        //public int AgendamentoId { get; set; } // -1

        // Relacionamento virtual para profissional para carregar informacoes do agendamento associados a esse profissional
        public Agendamento Agendamento { get; set; }

    }

    public enum GeneroTipo
    {
        Masculino, 
        Feminino,
        Outro
    }
}
