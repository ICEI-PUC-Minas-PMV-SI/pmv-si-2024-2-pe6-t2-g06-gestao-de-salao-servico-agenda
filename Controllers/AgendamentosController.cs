using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;
using System.Security.Claims;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    //[Authorize]
    /// <summary>
    /// Controller for managing agendamentos (appointments).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AgendamentosController : ControllerBase
    {
        // primeiro, configurar a variavel que irá acessar o banco
        private readonly ApplicationDbContext _context;

        //segundo, fazer con que o controler receba / solicite acesso ao banco - abrir connexao
        /// <summary>
        /// Inicializa uma nova instância de <see cref="AgendamentosController"/> class.
        /// </summary>
        /// <param name="context">O contexto do banco de dados.</param>
        public AgendamentosController(ApplicationDbContext context)
        {
            _context = context;
        }


        // Configuracao das rotas:        
        // ActionResult ira configurar o formato retornado
        // async = tempo de resposta assincrona ou tempo de espera
        // Task = tarefa/thread


        // retornar todos os agendamentos - GET
        /// <summary>
        /// Retorna todo os agendamentos do salão.
        /// </summary>
        /// <returns>Uma lista de agendamentos.</returns>
        [HttpGet]
        [Authorize]
        [Authorize(Roles = "3")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var model = await _context.Agendamentos
                    .Include(t => t.Usuario)
                    .Include(t => t.ServicoCategoria)
                    .Include(t => t.ServicoSubCategoria)
                    .ToListAsync();

                return Ok(model);
            }
            catch (Exception ex)
            {
                // Log the error if you have logging in place
                // Log.Error(ex, "Erro ao obter agendamentos");

                return StatusCode(500, new { message = "Ocorreu um erro ao obter os agendamentos.", details = ex.Message });
            }
        }

        // Get a single agendamento by id
        /// <summary>
        /// Retorna um agendamento pelo seu ID.
        /// </summary>
        /// <param name="id">o ID do agendamento.</param>
        /// <returns>O agendamento com o ID específico.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "1,2")]  // Somente Administradores (Role 1) e profissionaos (Role 2) podem acessar esse método
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                // Dá um Fetch o agendamento pelo seu ID
                var model = await _context.Agendamentos
                    .Include(t => t.Usuario)
                    .Include(t => t.ServicoCategoria)
                    .Include(t => t.ServicoSubCategoria)
                    .FirstOrDefaultAsync(c => c.Id == id);

                // Retorna NotFound o agendamento não é encontrado
                if (model == null)
                    return NotFound(new { message = "Agendamento não encontrado." });

                // Pega o ID usuario atual e seu perfil pelo token claims
                var usuarioAtualId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfilUsuarioAtual = User.FindFirst(ClaimTypes.Role)?.Value;

                // Checa se o usuario atual é um Administrador
                if (perfilUsuarioAtual == "1") // Role 1: Administrador
                {
                    // Administradores podem acessar qualquer agendamento
                    GerarLinks(model);
                    return Ok(model);
                }
                else if (perfilUsuarioAtual == "2") // Role 2: Profissional
                {
                    // Profissionais podem somente acessar agendamentos relacionados ao seu proprio ID
                    var profissionalId = int.Parse(usuarioAtualId);

                    // Checa se o profissional pode acessar esse agendamento
                    if (model.UsuarioId != profissionalId)
                    {
                        return Forbid("Você não tem permissão para acessar este agendamento.");
                    }

                    // Se o ID do profissional da match, returna o agendamento
                    GerarLinks(model);
                    return Ok(model);
                }

                // Se o perfil do usuario é inválido, returna Unauthorized
                return Unauthorized(new { message = "Função de usuário inválida." });
            }
            catch (Exception ex)
            {
                // Loga o erro (log opcional)
                // _logger.LogError(ex, "Erro ao obter agendamento com id {id}", id);

                // Returna 500 Internal Server Error con detalhes
                return StatusCode(500, new { message = "Erro ao obter o agendamento.", details = ex.Message });
            }
        }

        // Criar um novo agendamento
        /// <summary>
        /// Cria um novo agendamento.
        /// </summary>
        /// <param name="model">Os detalhes do agendamento a ser criado.</param>
        /// <returns>O agendamento Criado.</returns>
        [HttpPost]
        [Authorize(Roles = "1,2,3")] // Permite todos os perfis (1: Usuario, 2: Profissional, 3: Administrador)
        public async Task<ActionResult> Create(Agendamento model)
        {
            try
            {
                // Pega o usuario atual e seu perfil pelo token claims
                var usuarioAtualId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfilUsuarioAtual = User.FindFirst(ClaimTypes.Role)?.Value;

                // Checa se o usuario atual é um usuario (Role 1)
                if (perfilUsuarioAtual == "1") // Role 1: Usuario
                {
                    // Determina que o usuario pode criar agendamento somente para seu proprio ID e perfil
                    if (model.UsuarioId.ToString() != usuarioAtualId)
                    {
                        return Forbid("Você não pode criar agendamentos para outros usuários.");
                    }
                }

                // Se o usuario atual é um Profissional (Role 2) ou Administrador (Role 3),
                // eles podem criar agendamentos para qualquer usuario, sem restricao
                // Adiciona um novo agendamento no context e salva no banco
                _context.Agendamentos.Add(model);
                await _context.SaveChangesAsync();

                // Returna o status criado com o mais novo ID de recurso
                return CreatedAtAction("GetById", new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                // Captura os detalhes da excecao e returna codigo de erro 500
                var exceptionMessage = ex.Message;
                var innerExceptionMessage = ex.InnerException?.Message;

                return StatusCode(500, new
                {
                    message = "Ocorreu um erro ao processar a solicitação.",
                    details = exceptionMessage,
                    innerDetails = innerExceptionMessage
                });
            }
        }

        /// <summary>
        /// Atualiza um agendamento agendamento existente pelo seu ID.
        /// </summary>
        /// <param name="id">o ID do agendamento.</param>
        /// <param name="model">A atualização dos detalhes do agendamento.</param>
        /// <returns>Se update foi bem sucedido, não retorna nada.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "1,2,3")] // Permite todos os perfis (1: Usuario, 2: Profissional, 3: Administrador)
        public async Task<ActionResult> Update(int id, Agendamento model)
        {
            if (id != model.Id) return BadRequest();

            try
            {
                // Pega o ID do usuario atual e seu perfil pelo token claims
                var usuarioAtualId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfilUsuarioAtual = User.FindFirst(ClaimTypes.Role)?.Value;

                // Retorna o agendamento atual
                var agendamentoExistente = await _context.Agendamentos.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (agendamentoExistente == null) return NotFound();

                // Se o usuario atual é um usuario comum (Role 1), checa se eles estao atualizando o seu proprio agendamento
                if (perfilUsuarioAtual == "1") // Role 1: Usuario
                {
                    if (agendamentoExistente.UsuarioId.ToString() != usuarioAtualId)
                    {
                        return Forbid("Você não pode atualizar agendamentos de outros usuários.");
                    }
                }

                // Atualiza o agendamento com novos detalhes
                _context.Agendamentos.Update(model);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log de erro e retorna resposta de erro 500
                return StatusCode(500, new { message = "Erro ao atualizar o agendamento.", details = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um agendamento pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do agendamento.</param>
        /// <returns>Se a ação Delete é bem sucedida, não retorna nada.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "1,2,3")] // Permite todos os perfis (1: Usuario, 2: Profissional, 3: Administrador)
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Obtenha o ID e a função do usuário atual nas declarações de token
                var usuarioAtualId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfilUsuarioAtual = User.FindFirst(ClaimTypes.Role)?.Value;

                // Encontre o agendamento por ID
                var model = await _context.Agendamentos.FindAsync(id);

                if (model == null) return NotFound();

                // Caso o usuário atual seja um usuário regular (Role 1), ele só poderá excluir seu próprio agendamento
                if (perfilUsuarioAtual == "1") // Role 1: User
                {
                    if (model.UsuarioId.ToString() != usuarioAtualId)
                    {
                        return Forbid("Você não pode deletar agendamentos de outros usuários.");
                    }
                }

                // Remove o agendamento
                _context.Agendamentos.Remove(model);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Registre o erro e retorne uma resposta de erro 500
                return StatusCode(500, new { message = "Erro ao deletar o agendamento.", details = ex.Message });
            }
        }

        // GET: /api/agendamentos/usuario/{id}
        /// <summary>
        /// Retorna os agendamentos de um usuario pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do usuario para filtrar agendamentos.</param>
        /// <returns>Uma lista de agendamentos para um usuário específico.</returns>
        [HttpGet("usuario/{id}")]
        [Authorize("1,2,3")] // Somente usuarios autenticados podem acessar esse methodo
        public async Task<ActionResult> GetByUsuarioId(int id)
        {
            try
            {
                // Obtém o ID e a função do usuário atual das declarações do token
                var usuarioAtualId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfilUsuarioAtual = User.FindFirst(ClaimTypes.Role)?.Value;

                // Verifica as funções e controle de acesso
                if (perfilUsuarioAtual == "1") // Administrador (Role 1)
                {
                    // Os administradores podem acessar os agendamentos de qualquer usuário, sem necessidade de restrições

                }
                else if (perfilUsuarioAtual == "2") // Profissional (Role 2)
                {
                    // Profissional só pode acessar agendamentos relacionados ao seu próprio ID (ProfissionalId)
                    var profissionalId = int.Parse(usuarioAtualId);

                    // Caso o ID do profissional não seja igual ao RG solicitado, proibir o acesso
                    if (profissionalId != id)
                    {
                        return Forbid("Você não tem permissão para acessar agendamentos de outro profissional.");
                    }
                }
                else if (perfilUsuarioAtual == "3") // Usuario (Role 3)
                {
                    // Usuario regular só pode acessar seus próprios agendamentos
                    if (usuarioAtualId != id.ToString())
                    {
                        return Forbid("Você não tem permissão para acessar os agendamentos de outro usuário.");
                    }
                }
                else
                {
                    return Unauthorized("Função de usuário inválida.");
                }

                // Busca os agendamentos para o ID de usuário informado
                var agendamentos = await _context.Agendamentos
                    .Include(a => a.Usuario)
                    .Include(a => a.ServicoCategoria)
                    .Include(a => a.ServicoSubCategoria)
                    .Where(a => a.UsuarioId == id)
                    .ToListAsync();

                // Se nenhum agendamento for encontrado, retorna um resultado NotFound
                if (agendamentos == null || !agendamentos.Any())
                {
                    return NotFound(new { message = "Nenhum agendamento encontrado para este usuário." });
                }

                // Retorna a lista de agendamentos
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                // Log the exception (optional logging)
                // _logger.LogError(ex, "Error while retrieving agendamentos for user {id}", id);

                // Retorna um erro interno do servidor 500 com os detalhes da exceção
                return StatusCode(500, new { message = "Ocorreu um erro ao obter os agendamentos.", details = ex.Message });
            }
        }

        /// <summary>
        /// Recupera agendamentos por identificação profissional.
        /// </summary>
        /// <param name="id">A identificação profissional para filtrar agendamentos.</param>
        /// <returns>Uma lista de agendamentos para o profissional especificado.</returns>
        [HttpGet("profissional/{id}")]
        [Authorize(Roles = "2,3")] // Permitir que profissionais e administradores acessem este endpoint
        public async Task<ActionResult> GetByProfessionalId(int id)
        {
            try
            {
                // Buscar agendamentos relacionados à carteira profissional especificada
                var agendamentos = await _context.Agendamentos
                    .Include(t => t.Usuario) // Inclua detalhes do usuário, se necessário
                    .Include(t => t.ServicoCategoria)
                    .Include(t => t.ServicoSubCategoria)
                    .Where(a => a.UsuarioId == id) // Filtrar por ID profissional
                    .ToListAsync();

                // Verifique se algum agendamento foi encontrado
                if (agendamentos == null || !agendamentos.Any())
                {
                    return NotFound(new { message = "Nenhum agendamento encontrado para o profissional especificado." });
                }

                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                // Registre o erro e retorne uma resposta de erro 500
                return StatusCode(500, new { message = "Erro ao obter os agendamentos do profissional.", details = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza o status de um agendamento.
        /// </summary>
        /// <param name="id">O ID do agendamento.</param>
        /// <param name="novoStatus">O novo status a ser aplicado.</param>
        /// <returns>Nenhum conteúdo se a atualização for bem sucedida.</returns>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "2,3")] // Permitir que profissionais e administradores atualizem o status
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] string novoStatus)
        {
            try
            {
                // Valide o status de entrada
                if (string.IsNullOrWhiteSpace(novoStatus))
                {
                    return BadRequest(new { message = "O status fornecido é inválido." });
                }

                // Buscar o agendamento por ID
                var agendamento = await _context.Agendamentos.FindAsync(id);
                if (agendamento == null)
                {
                    return NotFound(new { message = "Agendamento não encontrado." });
                }

                // Atualizar o status do agendamento
                agendamento.Status = novoStatus;

                // Marcar a entidade como modificada
                _context.Agendamentos.Update(agendamento);
                await _context.SaveChangesAsync();

                return NoContent(); // Retornar 204 Sem conteúdo em atualização bem sucedida
            }
            catch (Exception ex)
            {
                // Registre o erro e retorne uma resposta de erro 500
                return StatusCode(500, new { message = "Erro ao atualizar o status do agendamento.", details = ex.Message });
            }
        }

        /// <summary>
        /// Cancela um agendamento pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do agendamento.</param>
        /// <returns>Nenhum conteúdo se o cancelamento for bem sucedido.</returns>
        [HttpPatch("{id}/cancelar")]
        [Authorize(Roles = "1,2,3")] // Permitir que usuários (para seus próprios compromissos), profissionais e administradores cancelem compromissos
        public async Task<ActionResult> CancelAgendamento(int id)
        {
            try
            {
                // Defina o status de cancelamento
                const string cancellationStatus = "Cancelado"; // Altere para qualquer status que indique cancelamento

                // Busca o agendamento pelo seu ID
                var agendamento = await _context.Agendamentos.FindAsync(id);
                if (agendamento == null)
                {
                    return NotFound(new { message = "Agendamento não encontrado." });
                }

                // Checa se o usuario tem ermissao para cancelar o agendamento
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (User.IsInRole("1")) // Perfil do usuario
                {
                    // Certifique-se de que o usuário esteja cancelando apenas seu próprio compromisso
                    if (int.Parse(userId) != agendamento.UsuarioId)
                    {
                        return Forbid(); // O usuário não tem permissão para cancelar este compromisso
                    }
                }

                // Atualizar o status para cancelado
                agendamento.Status = cancellationStatus;

                // Marcar a entidade como modificada
                _context.Agendamentos.Update(agendamento);
                await _context.SaveChangesAsync();

                return NoContent(); // Retornar 204 Sem conteúdo em atualização bem-sucedida
            }
            catch (Exception ex)
            {
                // Registre o erro e retorne uma resposta de erro 500
                return StatusCode(500, new { message = "Erro ao cancelar o agendamento.", details = ex.Message });
            }
        }

        /// <summary>
        /// Recupera agendamentos para uma data específica.
        /// </summary>
        /// <param name="data">A data para filtrar agendamentos.</param>
        /// <returns>Uma lista de agendamentos para a data especificada.</returns>
        [HttpGet("by-date/{data}")]
        [Authorize(Roles = "2,3")] //Permitir que profissionais e administradores acessem este endpoint
        public async Task<ActionResult> GetByDate(DateTime data)
        {
            try
            {
                // Buscar agendamentos para a data especificada
                var agendamentos = await _context.Agendamentos
                    .Include(t => t.Usuario)
                    .Include(t => t.ServicoCategoria)
                    .Include(t => t.ServicoSubCategoria)
                    .Where(a => a.DataAgendamento.Date == data.Date) // Certifique-se de que a comparação de datas seja apenas na data, não na hora
                    .ToListAsync();

                if (!agendamentos.Any())
                {
                    return NotFound(new { message = "Nenhum agendamento encontrado para a data especificada." });
                }

                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                // Registre o erro e retorne uma resposta de erro 500
                return StatusCode(500, new { message = "Erro ao obter agendamentos para a data especificada.", details = ex.Message });
            }
        }

        /// <summary>
        /// Recupera agendamentos dentro de um intervalo de datas.
        /// </summary>
        /// <param name="dataInicial">A data de início do intervalo.</param>
        /// <param name="dataFinal">A data de término do intervalo.</param>
        /// <returns>Uma lista de agendamentos dentro do intervalo de datas especificado.</returns>
        [HttpGet("between-dates")]
        [Authorize(Roles = "2,3")] // Permitir que profissionais e administradores acessem este endpoint
        public async Task<ActionResult> GetBetweenDates([FromQuery] DateTime dataInicial, [FromQuery] DateTime dataFinal)
        {
            try
            {
                // Buscar agendamentos dentro do intervalo de datas especificado
                var agendamentos = await _context.Agendamentos
                    .Include(t => t.Usuario)
                    .Include(t => t.ServicoCategoria)
                    .Include(t => t.ServicoSubCategoria)
                    .Where(a => a.DataAgendamento.Date >= dataInicial.Date && a.DataAgendamento.Date <= dataFinal.Date)
                    .ToListAsync();

                if (!agendamentos.Any())
                {
                    return NotFound(new { message = "Nenhum agendamento encontrado para o intervalo de datas especificado." });
                }

                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                // Registre o erro e retorne uma resposta de erro 500
                return StatusCode(500, new { message = "Erro ao obter agendamentos para o intervalo de datas especificado.", details = ex.Message });
            }
        }

        // passando as rotas para cada verbo
        private void GerarLinks(Agendamento model)
        {
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "self", metodo: "GET"));
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "update", metodo: "PUT"));
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "delete", metodo: "DELETE"));

        }


        //n-n nao se aplica nesse caso. Deixar comentado abaixo para quando for usar

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
