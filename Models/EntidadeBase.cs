namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    // Entidade base que contém os campos comuns
    public class EntidadeBase
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string? Email { get; set; }
        public string Telefone { get; set; }        
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Cep { get; set; }
    }
}
