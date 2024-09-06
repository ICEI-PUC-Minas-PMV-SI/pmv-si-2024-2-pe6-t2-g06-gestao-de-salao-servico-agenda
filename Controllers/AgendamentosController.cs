using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgendamentosController : ControllerBase
    {
        // primeiro, configurar a variavel que irá acessar o banco
        private readonly ApplicationDbContext _context;

        //segundo, fazer con que o controler receba / solicite acesso ao banco
        public AgendamentosController(ApplicationDbContext context)
        {
            _context = context;                
        }


        // Configuracao das rotas:
        // Rota da index para mostrar todos os agendamentos
        // ActionResult ira configurar o formato retornado
        // async = tempo de resposta assincrona ou tempo de espera
        // Task = tarefa/thread
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Agendamentos.ToListAsync();
            return Ok(model);
        }

    }
}
