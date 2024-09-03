namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    // Representa um agendamento, com detalhes adicionais sobre o agendamento, relacionado com o profissional e cliente.
    public class Agendamento
    {
        public Guid Id { get; set; }
        public DateTime Data { get; set; }
        public string Status { get; set; } // e.g., Marcado, Completado, Cancelado
        public Guid ProfissionalId { get; set; }
        public Profissional Profissional { get; set; }
        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }

}
