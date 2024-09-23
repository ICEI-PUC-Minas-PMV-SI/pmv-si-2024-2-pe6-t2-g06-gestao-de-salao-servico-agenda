using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities;

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
        public async Task<List<Agendamento>> GetAgendamentosByUsuarioOuProfissionalIdAsync(int usuarioId)
        {
            // reusando o mesmo metodo para ambos os timpos de usuario: usuario final ou profissional. contem mesma estrutura
            return await _context.Agendamentos
                .Include(a => a.Usuario)// Inclua detalhes do usuário, se necessário
                .Include(a => a.ServicoCategoria)
                .Include(a => a.ServicoSubCategoria)
                .Where(a => a.UsuarioId == usuarioId)// Filtrar por ID profissional
                .ToListAsync();
        }

        public async Task<List<Agendamento>> GetAgendamentosByDateAsync(DateTime date)
        {
            return await _context.Agendamentos
                .Include(a => a.Usuario)
                .Include(a => a.ServicoCategoria)
                .Include(a => a.ServicoSubCategoria)
                .Where(a => a.DataAgendamento.Date == date.Date) // Compare only the date part
                .ToListAsync();
        }
        public async Task<List<Agendamento>> GetAgendamentosBetweenDatesAsync(DateTime dataInicial, DateTime dataFinal)
        {
            return await _context.Agendamentos
                .Include(a => a.Usuario)
                .Include(a => a.ServicoCategoria)
                .Include(a => a.ServicoSubCategoria)
                .Where(a => a.DataAgendamento.Date >= dataInicial.Date && a.DataAgendamento.Date <= dataFinal.Date)
                .ToListAsync();
        }

    }
}
