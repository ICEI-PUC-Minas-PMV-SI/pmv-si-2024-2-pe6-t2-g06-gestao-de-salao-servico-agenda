using Microsoft.AspNetCore.Mvc;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.DTOs;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    // Define os endpoints da API para gerenciamento de profissionais.
    [ApiController]
    [Route("api/v1/professionals")]
    public class ProfissionalController : EntidadeBaseController<Profissional, ProfissionalDto>
    {
        public ProfissionalController(EntidadeService<Profissional, ProfissionalDto> service)
            : base(service) { }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string specialization, string city)
        {
            var professionals = await _service.GetAllAsync(p =>
                p.Specialization.Contains(specialization) && p.City.Contains(city));
            return Ok(professionals);
        }
    }

}
