using AgendamentoService.Data;
using AgendamentoService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgendamentoService.Repositories
{
    public class ProfissionalRepository : EntidadeBaseRepository<Profissional>, IProfissionalRepository
    {
        public ProfissionalRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Profissional>> GetBySpecializationAsync(string especializacao)
        {
            return await _context.Profissionais
                .Where(p => p.Especializacao.Contains(especializacao))
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<IEnumerable<Profissional>> GetAvailableProfessionalsAsync(DateTime data)
        {
            var busyProfissionais = await _context.Agendamentos
                .Where(a => a.Data.Date == data.Date)
                .Select(a => a.ProfissionalId)
                .Distinct()
                .ToListAsync();

            return await _context.Profissionais
                .Where(p => !busyProfissionais.Contains(p.Id))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
