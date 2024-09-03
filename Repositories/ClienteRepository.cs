using AgendamentoService.Data;
using AgendamentoService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;
using System.Threading.Tasks;

namespace AgendamentoService.Repositories
{
    public class ClienteRepository : EntidadeBaseRepository<Cliente>, IClienteRepository
    {
        public ClienteRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Cliente> GetByEmailAsync(string email)
        {
            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}
