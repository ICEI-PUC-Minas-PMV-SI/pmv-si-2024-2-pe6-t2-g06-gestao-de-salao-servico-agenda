using Microsoft.AspNetCore.Mvc;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.DTOs;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;

// Define os endpoints da API para gerenciamento de Agendamentos.
namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    [ApiController]
    [Route("api/v1/appointments")]
    public class AgentamentoController : EntidadeBaseController<Agendamento, AgendamentoDto>
    {
        public AgentamentoController(EntidadeBaseService<Agendamento, AgendamentoDto> service)
            : base(service) { }

        [HttpGet("Profissional/{professionalId}")]
        public async Task<IActionResult> GetByProfessionalId(Guid professionalId)
        {
            var appointments = await _service.GetAllAsync(a => a.ProfessionalId == professionalId);
            return Ok(appointments);
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetByClientId(Guid clientId)
        {
            var appointments = await _service.GetAllAsync(a => a.ClientId == clientId);
            return Ok(appointments);
        }

        [HttpGet("date")]
        public async Task<IActionResult> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var appointments = await _service.GetAllAsync(a =>
                a.Date >= startDate && a.Date <= endDate);
            return Ok(appointments);
        }
    }
}
