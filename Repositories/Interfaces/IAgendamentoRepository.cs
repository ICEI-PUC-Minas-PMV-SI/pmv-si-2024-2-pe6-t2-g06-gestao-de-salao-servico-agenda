using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgendamentoService.Repositories.Interfaces
{
    public interface IAgendamentoRepository : IEntidadeBaseRepository<Agendamento>
    {
        Task<IEnumerable<Agendamento>> GetPorProfissionalIdAsync(Guid profissionalId);
        Task<IEnumerable<Agendamento>> GetPorClienteIdAsync(Guid clienteId);
        Task<IEnumerable<Agendamento>> GetPorDataRangeAsync(DateTime dataInicial, DateTime dataFinal);
        Task<bool> HorarioDisponivelAsync(Guid profissionalId, DateTime date);
    }
}
