namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    // Representa um profissional, com detalhes adicionais como Especialização, DataNascimento, Genero e Numero de Licenca.
    // O serviço de Profissional herda do serviço base:
    public class Profissional : EntidadeBase
    {
        public string Especializacao { get; set; }
        public string? NumeroLicenca { get; set; }
        public string? Genero { get; set; }
        public DateTime DataNascimento { get; set; }
        public ICollection<Agendamento> Agendamentos { get; set; }
    }
}
