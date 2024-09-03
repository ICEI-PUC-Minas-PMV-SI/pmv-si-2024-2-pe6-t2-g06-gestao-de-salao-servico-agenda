using AgendamentoService.Data;
using AgendamentoService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Métodos específicos para Agendamento podem ser adicionados aqui
namespace AgendamentoService.Repositories
{
    public class AgendamentoRepository : EntidadeBaseRepository<Agendamento>, IAgendamentoRepository
    {
        public AgendamentoRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Agendamento>> GetByProfessionalIdAsync(Guid profissionalId)
        {
            return await _context.Agendamentos
                .Include(a => a.Cliente)
                .Where(a => a.ProfissionalId == profissionalId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> GetByClientIdAsync(Guid clientId)
        {
            return await _context.Agendamentos
                .Include(a => a.Profissional)
                .Where(a => a.ClienteId == clientId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> GetByDateRangeAsync(DateTime dataInicial, DateTime dataFinal)
        {
            return await _context.Agendamentos
                .Include(a => a.Profissional)
                .Include(a => a.Cliente)
                .Where(a => a.Data >= dataInicial && a.Data <= dataFinal)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> IsTimeSlotAvailableAsync(Guid professionalId, DateTime date)
        {
            return !await _context.Agendamentos
                .AnyAsync(a => a.ProfissionalId == professionalId && a.Data == date);
        }
    }
}
