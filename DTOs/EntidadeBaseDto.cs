namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.DTOs
{
    // Define os dados que são transferidos durante a criação e atualização de dados em comum de pessoas seja cliente ou profissional.
    public abstract class EntidadeBaseDto
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
