using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgendamentosController : ControllerBase
    {
        // primeiro, configurar a variavel que irá acessar o banco
        private readonly ApplicationDbContext _context;

        //segundo, fazer con que o controler receba / solicite acesso ao banco - abrir connexao
        public AgendamentosController(ApplicationDbContext context)
        {
            _context = context;
        }


        // Configuracao das rotas:        
        // ActionResult ira configurar o formato retornado
        // async = tempo de resposta assincrona ou tempo de espera
        // Task = tarefa/thread

        // retornar todos os agendamentos - GET
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Agendamentos.ToListAsync();
            return Ok(model);
        }

        // retornar um unico agendamento por meio do id
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Agendamentos.FirstOrDefaultAsync(c => c.Id == id);

            if(model == null) return NotFound();
            return Ok(model);
        }

        // Criar um agendamento
        [HttpPost]
        public async Task<ActionResult> Create(Agendamento model)
        {
            try
            {
                if (model.ProfissionalId <= 0 || model.UsuarioId <= 0 || model.ServicoSubCategoriaId <= 0)
                {
                    return BadRequest(new { message = "UsuarioId, ProfissionalId e ServicoSubCategoriaId são campos obrigatórios" });
                }

                _context.Agendamentos.Add(model);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetById", new {id = model.Id}, model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao processar a solicitação.", details = ex.Message });
            }

        }

        // atualizar um agendamento pelo id
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Agendamento model)
        {
            if (id != model.Id) return BadRequest();

            var modelo = await _context.Agendamentos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (modelo == null) return NotFound();

            _context.Agendamentos.Update(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // deletar um unico agendamento por meio do id
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _context.Agendamentos.FindAsync(id);

            if (model == null) return NotFound();

            _context.Agendamentos.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();

        }

    }
}
