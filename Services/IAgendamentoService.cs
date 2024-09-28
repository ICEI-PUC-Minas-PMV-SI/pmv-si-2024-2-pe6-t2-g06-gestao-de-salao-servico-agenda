using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities;
using System.Security.Claims;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Services
{
    public interface IAgendamentoService
    {
        Task<List<Agendamento>> GetAllAgendamentosAsync(int pageNumber, int pageSize);
        //Task<IEnumerable<Agendamento>> GetAllAgendamentosAsync();
        Task<Agendamento> GetAgendamentoByIdAsync(int id, string usuarioAtualId, string perfilUsuarioAtual);
        Task<(bool Sucesso, string Mensagem, int StatusCode)> CreateAgendamentoAsync(Agendamento agendamento, ClaimsPrincipal usuarioAtual);
        Task<(bool Sucesso, string Mensagem, int StatusCode)> UpdateAgendamentoAsync(int id, Agendamento agendamento, ClaimsPrincipal usuarioAtual);
        Task<(bool Sucesso, string Mensagem, int StatusCode)> DeleteAgendamentoAsync(int id, ClaimsPrincipal usuarioAtual);
        Task<(bool Sucesso, string Mensagem, int StatusCode, IEnumerable<Agendamento> Agendamentos)> GetAgendamentosByUsuarioIdAsync(int id, ClaimsPrincipal usuarioAtual);
        Task<(bool Sucesso, string Mensagem, int StatusCode, IEnumerable<Agendamento> Agendamentos)> GetAgendamentosByProfissionalIdAsync(int id, ClaimsPrincipal usuarioAtual);
        Task<(bool Sucesso, string Mensagem, int StatusCode)> UpdateAgendamentoStatusAsync(int id, string novoStatus, ClaimsPrincipal usuarioAtual);
        Task<(bool Sucesso, string Mensagem, int StatusCode)> CancelAgendamentoAsync(int id, ClaimsPrincipal usuarioAtual);
        Task<(bool Sucesso, string Mensagem, int StatusCode, IEnumerable<Agendamento> Agendamentos)> GetAgendamentosByDateAsync(DateTime data);
        Task<(bool Sucesso, string Mensagem, int StatusCode, IEnumerable<Agendamento> Agendamentos)> GetAgendamentosBetweenDatesAsync(DateTime dataInicial, DateTime dataFinal);
    }
}
