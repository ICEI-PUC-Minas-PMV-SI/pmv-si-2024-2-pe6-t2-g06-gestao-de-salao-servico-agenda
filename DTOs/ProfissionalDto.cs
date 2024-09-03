namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.DTOs
{
    // Define os dados que são transferidos durante a criação e atualização de profissionais.
    public class ProfissionalDto : EntidadeBaseDto
    {
        public string Especializacao { get; set; }
        public string? NumeroLicenca { get; set; }
        public string? Genero { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
