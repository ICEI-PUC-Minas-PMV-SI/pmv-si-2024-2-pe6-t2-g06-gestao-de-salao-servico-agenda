using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    public class Especializacao
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required]
        [DisplayName("Especialização")]
        public string TipoEspecializacao { get; set; }

        // Relacionamento virtual para agendamentos para carregar informacoes do profissional associados a essa especializacao
        public ICollection<Profissional> Profissionais { get; set; }// -n

        //[Column(TypeName="decimal(18,2)")] -- exemplo de configuracao de decimais
        // add-migration M00 --- para dar um build nos arquivos de migracao para serem usados para criar tabela no banco
        // Remove-Migration -- remove os arquivos de migracao
        // update-database - comando para criar a tabela no banco
    }
}
