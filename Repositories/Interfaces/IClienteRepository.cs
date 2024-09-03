using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;
using System.Threading.Tasks;

namespace AgendamentoService.Repositories.Interfaces
{
    public interface IClienteRepository : IEntidadeBaseRepository<Cliente>
    {
        Task<Cliente> GetByEmailAsync(string email);
    }
}
