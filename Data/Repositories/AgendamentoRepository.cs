using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities;
// conexao com dbcontext = banco de dados
namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data.Repositories
{
    // AgendamentosRepository.cs
    public class AgendamentosRepository : IAgendamentosRepository
    {
        private readonly ApplicationDbContext _context;

        public AgendamentosRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Agendamento> GetAgendamentoByIdNoTrackingAsync(int agendamentoId)
        {
            return await _context.Agendamentos
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == agendamentoId);
        }
        public async Task<IEnumerable<Agendamento>> GetAllAgendamentosAsync()
        {
            return await _context.Agendamentos
                .Include(a => a.Usuario)
                .Include(a => a.ServicoCategoria)
                .Include(a => a.ServicoSubCategoria)
                .ToListAsync();
        }

        public async Task<Agendamento> GetAgendamentoByIdAsync(int agendamentoId)
        {
            return await _context.Agendamentos
                .Include(a => a.Usuario)
                .Include(a => a.ServicoCategoria)
                .Include(a => a.ServicoSubCategoria)
                .FirstOrDefaultAsync(a => a.Id == agendamentoId);
        }
        public async Task CreateAgendamentoAsync(Agendamento agendamento)
        {
            await _context.Agendamentos.AddAsync(agendamento);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAgendamentoAsync(Agendamento agendamento)
        {
            _context.Agendamentos.Update(agendamento);
            await _context.SaveChangesAsync();
        }        
        public async Task DeleteAgendamentoAsync(Agendamento agendamento)
        {
            _context.Agendamentos.Remove(agendamento);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Agendamento>> GetAgendamentosByUsuarioIdAsync(int id)
        {
            return await _context.Agendamentos
                .Where(a => a.UsuarioId == id || a.ProfissionalId == id)
                .ToListAsync();
        }
        public async Task<List<Agendamento>> GetAgendamentosByProfissionalIdAsync(int id)
        {
            // Change the return type to List<Agendamento>
            return await _context.Agendamentos
                .Where(a => a.ProfissionalId == id)
                .ToListAsync();
        }
        public async Task<IEnumerable<Agendamento>> GetAgendamentosByDateAsync(DateTime data)
        {
            return await _context.Agendamentos
                .Where(a => a.DataAgendamento == data.Date) // Considera apenas a data, ignorando a hora
                .ToListAsync();
        }
        public async Task<IEnumerable<Agendamento>> GetAgendamentosBetweenDatesAsync(DateTime dataInicial, DateTime dataFinal)
        {
            return await _context.Agendamentos
                .Where(a => a.DataAgendamento >= dataInicial && a.DataAgendamento <= dataFinal)
                .ToListAsync();
        }

    }
}
