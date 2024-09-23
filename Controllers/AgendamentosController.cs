using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data.Repositories;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.DTOs;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities;
using System.Security.Claims;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    // AgendamentosController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class AgendamentosController : ControllerBase
    {
        private readonly IAgendamentosRepository _repository;

        public AgendamentosController(IAgendamentosRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retorna todo os agendamentos do salão.
        /// </summary>
        /// <returns>Uma lista de agendamentos.</returns>
        /// <remarks> Somente Administradores tem permissao para ver todos os agendamentos.</remarks>

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var agendamentos = await _repository.GetAllAgendamentosAsync();
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao obter os agendamentos.", details = ex.Message });
            }
        }

        // Get a single agendamento by id
        /// <summary>
        /// Retorna um agendamento pelo seu ID.
        /// </summary>
        /// <param name="id">o ID do agendamento.</param>
        /// <returns>O agendamento com o ID específico.</returns>
        /// <remarks> Somente Administradores e Profissionais tem permissao para ver agendamentos pelo Id, porem, profissionais poderao ver somente seus proprios agendamentos.</remarks>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Profissional")]  // Somente Administradores (Role 1) e profissionaos (Role 2) podem acessar esse método
        public async Task<ActionResult> GetAgendamentoById(int id)
        {
            try
            {
                // Recupera agendamento pelo ID usando o repositorio
                var model = await _repository.GetAgendamentoByIdAsync(id);

                // Retorna NotFound o agendamento não é encontrado
                if (model == null)
                    return NotFound(new { message = "Agendamento não encontrado." });

                // Pega o ID usuario atual e seu perfil pelo token claims
                var usuarioAtualId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfilUsuarioAtual = User.FindFirst(ClaimTypes.Role)?.Value;

                // Checa se o usuario atual é um Administrador
                if (perfilUsuarioAtual == "Administrador") // Role 1: Administrador
                {
                    // Administradores podem acessar qualquer agendamento
                    GerarLinks(model);
                    return Ok(model);
                }
                else if (perfilUsuarioAtual == "Profissional") // Role 2: Profissional
                {
                    // Profissionais podem somente acessar agendamentos relacionados ao seu proprio ID
                    var profissionalId = int.Parse(usuarioAtualId);

                    // Checa se o profissional pode acessar esse agendamento
                    if (model.ProfissionalId != profissionalId)
                    {
                        return Forbid("Funcao Invalida.");
                    }

                    // Se o ID do profissional da match, returna o agendamento
                    GerarLinks(model);
                    return Ok(model);
                }

                // Se o perfil do usuario é inválido, returna Unauthorized
                return Unauthorized(new { message = "Você não tem permissão para acessar este agendamento." });
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
        /// <param name="agendamento">Os detalhes do agendamento a ser criado.</param>
        /// <returns>O agendamento Criado.</returns>
        /// <remarks> Administradores e Profissionais podem criar agendamentos para outros usuarios, mas um usuario final nao pode criar agendamento para outro usuario, mas somente para o seu proprio login</remarks>
        [HttpPost]
        [Authorize(Roles = "Administrador,Profissional,Usuario")] // Permite todos os perfis (1: Usuario, 2: Profissional, 3: Administrador)
        public async Task<ActionResult> CreateAgendamento([FromBody] Agendamento agendamento)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get the current user's ID and role from the token claims
                var usuarioAtualId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfilUsuarioAtual = User.FindFirst(ClaimTypes.Role)?.Value;

                // Check if the current user is a regular user (Role 1)
                if (perfilUsuarioAtual == "Usuario") // Role 1: Usuario
                {
                    // Ensure the user can only create an appointment for their own ID
                    if (agendamento.UsuarioId.ToString() != usuarioAtualId)
                    {
                        return new ContentResult
                        {
                            StatusCode = 403,
                            Content = "Você não tem permissao para criar agendamentos para outros usuários."
                        };
                    }
                }

                // If the current user is a Professional (Role 2) or Administrator (Role 3),
                // they can create appointments for any user, without restriction.

                // Use the repository to add the new agendamento
                await _repository.CreateAgendamentoAsync(agendamento);

                // Return status created with the newly created resource ID
                return CreatedAtAction(nameof(GetAgendamentoById), new { id = agendamento.Id }, agendamento);
            }
            catch (Exception ex)
            {
                // Capture the details of the exception and return a 500 error
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
        /// <remarks> Administradores e Profissionais podem atualizar agendamentos para outros usuarios, mas um usuario final nao pode atualizar agendamento para outro usuario, mas somente para o seu proprio login</remarks>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Profissional,Usuario")] // Permite todos os perfis (1: Usuario, 2: Profissional, 3: Administrador)
        public async Task<ActionResult> UpdateAgendamento(int id, Agendamento model)
        {
            if (id != model.Id) return BadRequest("O ID do agendamento não corresponde.");

            try
            {
                // Pega o ID do usuario atual e seu perfil pelo token claims
                var usuarioAtualId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfilUsuarioAtual = User.FindFirst(ClaimTypes.Role)?.Value;

                // Retorna o agendamento atual
                var agendamentoExistente = await _repository.GetAgendamentoByIdNoTrackingAsync(id);

                if (agendamentoExistente == null) return NotFound(new { message = "Agendamento não encontrado." });

                // Se o usuario atual é um usuario comum (Role 1), checa se eles estao atualizando o seu proprio agendamento
                if (perfilUsuarioAtual == "Usuario") // Role 1: Usuario
                {
                    if (agendamentoExistente.UsuarioId.ToString() != usuarioAtualId)
                    {
                        return Forbid("Você não pode atualizar agendamentos de outros usuários.");
                    }
                }

                // Atualiza o agendamento com novos detalhes
                
                await _repository.UpdateAgendamentoAsync(model);

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
        /// <remarks> Administradores e Profissionais podem deletar agendamentos para outros usuarios, mas um usuario final nao pode deletar agendamento para outro usuario, mas somente para o seu proprio login</remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Profissional,Usuario")] // Permite todos os perfis (1: Usuario, 2: Profissional, 3: Administrador)
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Obtenha o ID e a função do usuário atual nas declarações de token
                var usuarioAtualId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfilUsuarioAtual = User.FindFirst(ClaimTypes.Role)?.Value;

                // Encontre o agendamento por ID usando o repositório
                var model = await _repository.GetAgendamentoByIdNoTrackingAsync(id);

                if (model == null) return NotFound();

                // Caso o usuário atual seja um usuário regular (Role 1), ele só poderá excluir seu próprio agendamento
                if (perfilUsuarioAtual == "Usuario") // Role 1: User
                {
                    if (model.UsuarioId.ToString() != usuarioAtualId)
                    {
                        return Forbid("Você não pode deletar agendamentos de outros usuários.");
                    }
                }

                // Remove o agendamento usando o repositório
                await _repository.DeleteAgendamentoAsync(model);

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
        [Authorize("Administrador,Profissional,Usuario")] // Somente usuarios autenticados podem acessar esse methodo
        public async Task<ActionResult> GetAgendamentoByUsuarioId(int id)
        {
            try
            {
                // Obtém o ID e a função do usuário atual das declarações do token
                var usuarioAtualId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfilUsuarioAtual = User.FindFirst(ClaimTypes.Role)?.Value;

                // Verifica as funções e controle de acesso
                if (perfilUsuarioAtual == "Administrador") // Administrador (Role 1)
                {
                    // Os administradores podem acessar os agendamentos de qualquer usuário, sem necessidade de restrições

                }
                else if (perfilUsuarioAtual == "Profissional") // Profissional (Role 2)
                {
                    // Profissional só pode acessar agendamentos relacionados ao seu próprio ID (ProfissionalId)
                    var profissionalId = int.Parse(usuarioAtualId);

                    // Caso o ID do profissional não seja igual ao RG solicitado, proibir o acesso
                    if (profissionalId != id)
                    {
                        return Forbid("Você não tem permissão para acessar agendamentos de outro profissional.");
                    }
                }
                else if (perfilUsuarioAtual == "Usuario") // Usuario (Role 3)
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
                var agendamentos = await _repository.GetAgendamentosByUsuarioOuProfissionalIdAsync(id);

                // Se nenhum agendamento for encontrado, retorna um resultado NotFound
                if (agendamentos == null || agendamentos.Any()) 
                {
                    return NotFound(new { message = "Nenhum agendamento encontrado para este usuário." });
                }

                // Retorna a lista de agendamentos
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                // Retorna um erro interno do servidor 500 com os detalhes da exceção
                return StatusCode(500, new { message = "Ocorreu um erro ao obter os agendamentos.", details = ex.Message });
            }
        }

        /// <summary>
        /// Recupera agendamentos por identificação profissional.
        /// </summary>
        /// <param name="id">A identificação profissional para filtrar agendamentos.</param>
        /// <returns>Uma lista de agendamentos para o profissional especificado.</returns>
        /// <remarks> Administradores podem ver agendamentos de um determinado profissional pelo seu id. Profissionais tambem podem ver somente seus proprios agendamentos</remarks>
        [HttpGet("profissional/{id}")]
        [Authorize(Roles = "Administrador,Profissional")] // Permitir que profissionais e administradores acessem este endpoint
        public async Task<ActionResult> GetAgendamentoByProfessionalId(int id)
        {
            try
            {
                // Buscar agendamentos relacionados à carteira profissional especificada
                var agendamentos = await _repository.GetAgendamentosByUsuarioOuProfissionalIdAsync(id);

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
        [Authorize(Roles = "Administrador,Profissional")] // Permitir que profissionais e administradores atualizem o status
        public async Task<ActionResult> UpdateAgendamentoStatus(int id, [FromBody] string novoStatus)
        {
            try
            {
                // Valide o status de entrada
                if (string.IsNullOrWhiteSpace(novoStatus))
                {
                    return BadRequest(new { message = "O status fornecido é inválido." });
                }

                // Buscar o agendamento por ID
                var agendamento = await _repository.GetAgendamentoByIdNoTrackingAsync(id);

                if (agendamento == null)
                {
                    return NotFound(new { message = "Agendamento não encontrado." });
                }

                // Atualizar o status do agendamento
                agendamento.Status = novoStatus;

                // Marcar a entidade como modificada
                _repository.UpdateAgendamentoAsync(agendamento);

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
        [Authorize(Roles = "Administrador,Profissional,Usuario")] // Permitir que usuários (para seus próprios compromissos), profissionais e administradores cancelem compromissos
        public async Task<ActionResult> CancelAgendamento(int id)
        {
            try
            {
                // Defina o status de cancelamento
                const string cancellationStatus = "Cancelado"; // Altere para qualquer status que indique cancelamento

                // Busca o agendamento pelo seu ID
                var agendamento = await _repository.GetAgendamentoByIdNoTrackingAsync(id);
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
                _repository.UpdateAgendamentoAsync(agendamento);

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
        [Authorize(Roles = "Administrador,Profissional")] //Permitir que profissionais e administradores acessem este endpoint
        public async Task<ActionResult> GetAgendamentoByDate(DateTime data)
        {
            try
            {
                // Buscar agendamentos para a data especificada
                var agendamentos = await _repository.GetAgendamentosByDateAsync(data);

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
        [Authorize(Roles = "Administrador,Profissional")] // Permitir que profissionais e administradores acessem este endpoint
        public async Task<ActionResult> GetBetweenDates([FromQuery] DateTime dataInicial, [FromQuery] DateTime dataFinal)
        {
            try
            {
                // Buscar agendamentos dentro do intervalo de datas especificado
                var agendamentos = await _repository.GetAgendamentosBetweenDatesAsync(dataInicial, dataFinal);

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
    }
}
