using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgendamentoService.Repositories.Interfaces
{
    public interface IProfissionalRepository : IEntidadeBaseRepository<Profissional>
    {
        Task<IEnumerable<Profissional>> GetBySpecializationAsync(string especializacao);
        Task<IEnumerable<Profissional>> GetAvailableProfessionalsAsync(DateTime data);
    }
}
