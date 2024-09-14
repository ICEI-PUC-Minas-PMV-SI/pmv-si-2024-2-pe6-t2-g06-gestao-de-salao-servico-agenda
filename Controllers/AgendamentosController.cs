using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    [Authorize]
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
            try
            {
                var model = await _context.Agendamentos
                    .Include(t => t.Usuario)//.ThenInclude(t => t.Usuario)
                    .Include(t => t.ServicoCategoria)
                    .Include(t => t.ServicoSubCategoria)
                    .ToListAsync();
                
                return Ok(model);
            }
            catch (Exception ex)
            {
                // Log do erro (se tiver um sistema de log)
                // Log.Error(ex, "Erro ao obter agendamentos");

                return StatusCode(500, new { message = "Ocorreu um erro ao obter os agendamentos.", details = ex.Message });
            }
        }

        // retornar um unico agendamento por meio do id
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Agendamentos
                .Include(t => t.Usuario)//.ThenInclude(t => t.Usuario)
                .Include(t => t.ServicoCategoria)
                .Include(t => t.ServicoSubCategoria)
                .FirstOrDefaultAsync(c => c.Id == id);

            if(model == null) return NotFound();
            GerarLinks(model);
            return Ok(model);
        }

        // Criar um agendamento
        [HttpPost]
        public async Task<ActionResult> Create(Agendamento model)
        {
            try
            {
                // Add the new model to the context and save it
                _context.Agendamentos.Add(model);
                await _context.SaveChangesAsync();

                // Return the created status with the newly created resource's ID
                return CreatedAtAction("GetById", new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                // Capture both the main exception and inner exception, if any
                var exceptionMessage = ex.Message;
                var innerExceptionMessage = ex.InnerException?.Message;

                // Return a 500 error code with detailed exception messages
                return StatusCode(500, new
                {
                    message = "Ocorreu um erro ao processar a solicitação.",
                    details = exceptionMessage,
                    innerDetails = innerExceptionMessage
                });
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

        // passando as rotas para cada verbo
        private void GerarLinks(Agendamento model)
        {
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "self", metodo: "GET"));
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "update", metodo: "PUT"));
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "delete", metodo: "DELETE"));

        }

        //// Metodo: Operacao para salvar o usuario que esta linkado ao agendamento na tabela n-n
        //// associacao do usuario no agendamento
        //[HttpPost("{id}/usuarios")]
        //public async Task<ActionResult> AddUsuario(int id, AgendamentoUsuariosDto model)
        //{


        //    // passar o usuario dto ao inves do usuario. essa configuracao garante que a senha tambem sera passada
        //    AgendamentoUsuarios novoAgendamento = new AgendamentoUsuarios()
        //    {
        //        AgendamentoId = model.AgendamentoId,
        //        Agendamento = model.Agendamento,
        //        UsuarioId = model.UsuarioId,
        //        Usuario = model.Usuario
        //    };

        //    if (novoAgendamento == null || id != novoAgendamento.UsuarioId) return BadRequest(ModelState);
        //    // passar a camada de dto

        //    _context.AgendamentoUsuarios.Add(novoAgendamento);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction("GetById", new { id = novoAgendamento.UsuarioId}, novoAgendamento);
        //}

        //[HttpDelete("{id}/usuarios")]
        //public async Task<ActionResult> DeleteUsuario(int agendamentoId, int usuarioId)
        //{
        //    var model = await _context.AgendamentoUsuarios
        //        .Where(c => c.AgendamentoId == agendamentoId && c.UsuarioId == usuarioId)
        //        .FirstOrDefaultAsync();

        //    if (model == null) return NotFound();

        //    _context.AgendamentoUsuarios.Remove(model);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}


        
    }
}
