using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities
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
    [Table("Usuarios")]
    public class Usuario //: LinksHATEOS //BaseEntity
    {

        //// O campo Id é herdado de BaseEntity e o JsonPropertyOrder com define sua posição no JSON
        //[JsonPropertyOrder(1)]
        //public new int Id { get; set; }  // Reescrevendo para garantir a ordem no JSON
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Key]
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }        

        [Required]
        [JsonIgnore]
        public string Senha { get; set; }

        [Required]
        [Phone]
        public string Telefone { get; set; }

        [Required]
        [MaxLength(200)]
        public string Endereco { get; set; }

        [Required]
        [MaxLength(200)]
        public string Cidade { get; set; }

        [Required]
        [MaxLength(200)]
        public string Estado { get; set; }

        [Required]
        [MaxLength(200)]
        public string Cep { get; set; }

        // Outros detalhes, como data de nascimento, podem ser adicionados conforme necessário        
        [Required]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        [Required]
        public GeneroTipo? Genero { get; set; }
        [Required]
        public Perfil Perfil { get; set; }
        [JsonIgnore]
        public ICollection<Agendamento> AgendamentosComoCliente { get; set; }
        [JsonIgnore]
        public ICollection<Agendamento> AgendamentosComoProfissional { get; set; }
        //public ICollection<Agendamento> Agendamentos { get; set; }

        //public List<LinkDto> Links { get; set; } = new List<LinkDto>();
        // Foreign key to Salão
        [ForeignKey("Cnpj")] // Specify the navigation property here
        public string Cnpj { get; set; }

        public virtual Salao Salao { get; set; } // Navigation property

    }

    public enum GeneroTipo
    {
        Masculino, //0
        Feminino,  //1
        Outro      //2
    }
    public enum Perfil
    {
        [Display(Name = "Administrador")] //0
        Administrador,
        [Display(Name = "Profissional")]  //1
        Profissional,
        [Display(Name = "Usuario")]       //2
        Usuario
    }


}
