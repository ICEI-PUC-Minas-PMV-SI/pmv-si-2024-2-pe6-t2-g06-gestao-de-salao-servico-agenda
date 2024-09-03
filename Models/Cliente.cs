namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    // Representa um cliente, com detalhes adicionais como Genero e DataNascimento.
    // O serviço de Cliente herda do serviço base:
    public class Cliente : EntidadeBase
    {
        public string? Genero { get; set; }
        public DateTime DataNascimento { get; set; }
        public ICollection<Agendamento> Agendamentos { get; set; }
    }

}
