using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data.Repositories
{
    // IAgendamentosRepository.cs
    public interface IAgendamentosRepository
    {
        // Método para obter agendamentos paginados
        Task<List<Agendamento>> GetPagedAgendamentosAsync(int skip, int take);

        // Método para obter o número total de agendamentos
        //Task<IEnumerable<Agendamento>> GetAllAgendamentosAsync();
        Task<Agendamento> GetAgendamentoByIdAsync(int agendamentoId);
        Task CreateAgendamentoAsync(Agendamento agendamento);
        Task UpdateAgendamentoAsync(Agendamento agendamento);
        Task DeleteAgendamentoAsync(Agendamento agendamento);
        Task<Agendamento> GetAgendamentoByIdNoTrackingAsync(int agendamentoId); // Recupera para update        
        Task<List<Agendamento>> GetAgendamentosByUsuarioIdAsync(int usuarioId);
        Task<List<Agendamento>> GetAgendamentosByProfissionalIdAsync(int id);
        Task<IEnumerable<Agendamento>> GetAgendamentosByDateAsync(DateTime data);
        Task<IEnumerable<Agendamento>> GetAgendamentosBetweenDatesAsync(DateTime dataInicial, DateTime dataFinal);

        // Proposito da Interface: 
    }
}



