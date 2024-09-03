using Microsoft.AspNetCore.Mvc;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.DTOs;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    [ApiController]
    [Route("api/v1/clients")]
    public class ClientController : EntityController<Client, ClientDto>
    {
        public ClientController(EntidadeService<Client, ClientDto> service)
            : base(service) { }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string name, string email)
        {
            var clients = await _service.GetAllAsync(c =>
                c.Name.Contains(name) && c.Email.Contains(email));
            return Ok(clients);
        }
    }

}
