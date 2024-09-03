namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.DTOs
{
    // Define os dados que são transferidos durante a criação e atualização de Agendamento.
    public class AgendamentoDto
    {
        public Guid Id { get; set; }
        public DateTime Data { get; set; }
        public string Status { get; set; }
        public Guid ProfissionalId { get; set; }
        public Guid ClienteId { get; set; }
    }

}
