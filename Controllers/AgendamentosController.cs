using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data.Repositories;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.DTOs;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgendamentosController : ControllerBase
    {
        private readonly IAgendamentoService _agendamentoService;

        public AgendamentosController(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }

        /// <summary>
        /// Retorna todos os agendamentos do salão de beleza.
        /// Apenas usuários com papel de Administrador podem acessar.
        /// </summary>
        /// <remarks>
        /// - **Authorization**: JWT Token no formato `Bearer {token}`
        /// - **Content-Type**: application/json
        /// </remarks>
        /// /// <response code="200">Lista de agendamentos retornada com sucesso.</response>
        /// <response code="500">Erro ao buscar os agendamentos.</response>
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        [SwaggerOperation(
        Summary = "Obter todos os agendamentos",
        Description = "Retorna todos os agendamentos. Somente administradores podem acessar.",
        OperationId = "GetAllAgendamentos"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Lista de agendamentos retornada com sucesso", typeof(Agendamento[]))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao buscar os agendamentos")]

        public async Task<ActionResult> GetAllAgendamentos()
        {
            try
            {
                // Chama o serviço para obter o agendamento
                var agendamentos = await _agendamentoService.GetAllAgendamentosAsync();
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao obter os agendamentos.", details = ex.Message });
            }
        }

        /// <summary>
        /// Retorna um agendamento específico pelo seu ID.
        /// Apenas administradores e profissionais podem acessar.
        /// </summary>
        /// <remarks>
        /// - **Authorization**: JWT Token no formato `Bearer {token}`
        /// - **Content-Type**: application/json
        /// </remarks>
        /// <param name="id">ID do agendamento</param>
        /// <response code="200">Agendamento retornado com sucesso.</response>
        /// <response code="404">Agendamento não encontrado.</response>
        /// <response code="403">Acesso negado.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Profissional")]
        [SwaggerOperation(
        Summary = "Obter agendamento por ID",
        Description = "Retorna um agendamento específico. Somente administradores e profissionais podem acessar.",
        OperationId = "GetAgendamentoById")]
        [SwaggerResponse(StatusCodes.Status200OK, "Agendamento retornado com sucesso", typeof(Agendamento))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Agendamento não encontrado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado")]
        public async Task<ActionResult> GetAgendamentoById(int id)
        {
            try
            {
                // Pega o ID do usuário atual e seu perfil pelo token claims
                var usuarioAtualId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfilUsuarioAtual = User.FindFirst(ClaimTypes.Role)?.Value;

                // Chama o serviço para obter o agendamento
                var model = await _agendamentoService.GetAgendamentoByIdAsync(id, usuarioAtualId, perfilUsuarioAtual);

                // Se o agendamento não for encontrado, retorna NotFound
                if (model == null)
                    return NotFound(new { message = "Agendamento não encontrado." });

                // Adiciona links ou outras operações antes de retornar
                GerarLinks(model);

                return Ok(model);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Se o usuário não tiver permissão, retorna 403 com uma mensagem customizada
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            catch (Exception ex)
            {
                // Loga o erro e retorna 500
                return StatusCode(500, new { message = "Erro ao obter o agendamento.", details = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo agendamento.
        /// Administradores, profissionais e usuários podem criar agendamentos.
        /// </summary>
        /// <remarks>
        /// - **Authorization**: JWT Token no formato `Bearer {token}`
        /// - **Content-Type**: application/json
        /// </remarks>
        /// <param name="agendamento">Objeto com os detalhes do novo agendamento</param>
        /// <response code="201">Agendamento criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="500">Erro ao criar o agendamento.</response>
        [HttpPost]
        [Authorize(Roles = "Administrador,Profissional,Usuario")]
        [SwaggerOperation(
            Summary = "Criar um novo agendamento",
            Description = "Cria um novo agendamento. Administradores, profissionais e usuários autenticados podem criar.",
            OperationId = "CreateAgendamento"
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Agendamento criado com sucesso", typeof(Agendamento))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao criar o agendamento")]
        public async Task<ActionResult> CreateAgendamento([FromBody] AgendamentoDto agendamento)
        {
            try
            {
                // passar o usuario dto ao inves do usuario. essa configuracao garante que a senha tambem sera passada
                Agendamento novoAgendamento = new Agendamento()
                {
                    DataAgendamento = agendamento.DataAgendamento,
                    HoraAgendamento = agendamento.HoraAgendamento,
                    Status = agendamento.Status,
                    Observacoes = agendamento.Observacoes,
                    ServicoCategoriaId = agendamento.ServicoCategoriaId,
                    ServicoSubCategoriaId = agendamento.ServicoSubCategoriaId,
                    ProfissionalId = agendamento.ProfissionalId,
                    UsuarioId = agendamento.UsuarioId                    

                };

                // Chama o service para criar o agendamento
                var result = await _agendamentoService.CreateAgendamentoAsync(novoAgendamento, User);

                // Se o resultado for um erro de autorização ou dados inválidos, retorna o código adequado
                if (!result.Sucesso)
                {
                    return StatusCode(result.StatusCode, result.Mensagem);
                }

                // Retorna status 201 Created com o ID do recurso criado
                return CreatedAtAction(nameof(GetAgendamentoById), new { id = novoAgendamento.Id }, novoAgendamento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Ocorreu um erro ao processar a solicitação.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Atualiza um agendamento existente pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do agendamento.</param>
        /// <param name="model">Os detalhes do agendamento a serem atualizados.</param>
        /// <returns>Se a atualização for bem sucedida, retorna NoContent.</returns>
        /// <remarks>
        /// - **Authorization**: JWT Token no formato `Bearer {token}` é necessário
        /// - **Content-Type**: application/json
        /// - **Regras**: Administradores e profissionais podem atualizar agendamentos para outros usuários, enquanto os usuários finais só podem atualizar seus próprios agendamentos.
        /// </remarks>
        /// <response code="204">Agendamento atualizado com sucesso.</response>
        /// <response code="400">Requisição inválida (dados incorretos ou faltantes).</response>
        /// <response code="403">Acesso negado (usuário não tem permissão).</response>
        /// <response code="404">Agendamento não encontrado.</response>
        /// <response code="500">Erro ao processar a solicitação.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Profissional,Usuario")]
        [SwaggerOperation(
            Summary = "Atualiza um agendamento",
            Description = "Atualiza um agendamento existente. Administradores e profissionais podem atualizar qualquer agendamento, enquanto usuários finais só podem atualizar seus próprios agendamentos."
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Agendamento atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Agendamento não encontrado.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação.")]
        public async Task<ActionResult> UpdateAgendamento(int id, Agendamento model)
        {
            try
            {
                // Chama o service para atualizar o agendamento
                var result = await _agendamentoService.UpdateAgendamentoAsync(id, model, User);

                // Se o resultado for um erro de autorização ou dados inválidos, retorna o código adequado
                if (!result.Sucesso)
                {
                    return StatusCode(result.StatusCode, result.Mensagem);
                }

                // Retorna NoContent em caso de sucesso
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao atualizar o agendamento.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Deleta um agendamento pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do agendamento.</param>
        /// <returns>Se a exclusão for bem sucedida, retorna NoContent.</returns>
        /// <remarks>
        /// - **Authorization**: JWT Token no formato `Bearer {token}` é necessário
        /// - **Regras**: Administradores e profissionais podem deletar agendamentos de qualquer usuário. Usuários finais só podem deletar seus próprios agendamentos.
        /// </remarks>
        /// <response code="204">Agendamento deletado com sucesso.</response>
        /// <response code="403">Acesso negado (usuário não tem permissão).</response>
        /// <response code="404">Agendamento não encontrado.</response>
        /// <response code="500">Erro ao processar a solicitação.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Profissional,Usuario")]
        [SwaggerOperation(
            Summary = "Deleta um agendamento",
            Description = "Deleta um agendamento pelo seu ID. Administradores e profissionais podem deletar qualquer agendamento, enquanto usuários finais só podem deletar os seus."
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Agendamento deletado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Agendamento não encontrado.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação.")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Chama o serviço para deletar o agendamento
                var result = await _agendamentoService.DeleteAgendamentoAsync(id, User);

                // Verifica se houve sucesso ou erro na operação e retorna o código adequado
                if (!result.Sucesso)
                {
                    return StatusCode(result.StatusCode, result.Mensagem);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao deletar o agendamento.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Retorna os agendamentos de um usuário pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do usuário.</param>
        /// <returns>Uma lista de agendamentos do usuário especificado.</returns>
        /// <remarks>
        /// - **Authorization**: JWT Token no formato `Bearer {token}` é necessário
        /// </remarks>
        /// <response code="200">Agendamentos retornados com sucesso.</response>
        /// <response code="403">Acesso negado (usuário não tem permissão).</response>
        /// <response code="404">Nenhum agendamento encontrado para o usuário especificado.</response>
        /// <response code="500">Erro ao processar a solicitação.</response>
        [HttpGet("usuario/{id}")]
        [Authorize(Roles = "Administrador,Profissional,Usuario")]
        [SwaggerOperation(
            Summary = "Retorna agendamentos por usuário",
            Description = "Retorna todos os agendamentos de um usuário pelo seu ID. Administradores e profissionais podem acessar agendamentos de qualquer usuário, enquanto usuários finais só podem acessar os seus próprios."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Agendamentos retornados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhum agendamento encontrado.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação.")]
        public async Task<ActionResult> GetAgendamentoByUsuarioId(int id)
        {
            try
            {
                // Chama o serviço para obter agendamentos por usuário
                var result = await _agendamentoService.GetAgendamentosByUsuarioIdAsync(id, User);

                // Verifica se houve sucesso e retorna a resposta apropriada
                if (!result.Sucesso)
                {
                    return StatusCode(result.StatusCode, result.Mensagem);
                }

                return Ok(result.Agendamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Ocorreu um erro ao obter os agendamentos.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Retorna os agendamentos de um profissional pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do profissional.</param>
        /// <returns>Uma lista de agendamentos do profissional especificado.</returns>
        /// <remarks>
        /// - **Authorization**: JWT Token no formato `Bearer {token}` é necessário
        /// - **Regras**: Administradores podem acessar os agendamentos de qualquer profissional, enquanto profissionais só podem acessar os seus próprios agendamentos.
        /// </remarks>
        /// <response code="200">Agendamentos retornados com sucesso.</response>
        /// <response code="403">Acesso negado (usuário não tem permissão).</response>
        /// <response code="404">Nenhum agendamento encontrado para o profissional especificado.</response>
        /// <response code="500">Erro ao processar a solicitação.</response>
        [HttpGet("profissional/{id}")]
        [Authorize(Roles = "Administrador,Profissional")]
        [SwaggerOperation(
            Summary = "Retorna agendamentos por profissional",
            Description = "Retorna todos os agendamentos de um profissional pelo seu ID. Administradores podem ver todos os agendamentos de qualquer profissional, enquanto profissionais só podem ver os seus próprios."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Agendamentos retornados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhum agendamento encontrado.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação.")]
        public async Task<ActionResult> GetAgendamentoByProfessionalId(int id)
        {
            try
            {
                // Chama o serviço para obter agendamentos por profissional
                var result = await _agendamentoService.GetAgendamentosByProfissionalIdAsync(id, User);

                // Verifica o resultado e retorna a resposta apropriada
                if (!result.Sucesso)
                {
                    return StatusCode(result.StatusCode, result.Mensagem);
                }

                return Ok(result.Agendamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao obter os agendamentos do profissional.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Atualiza o status de um agendamento.
        /// </summary>
        /// <param name="id">O ID do agendamento.</param>
        /// <param name="novoStatus">O novo status a ser aplicado.</param>
        /// <returns>Retorna NoContent se a atualização for bem sucedida.</returns>
        /// <remarks>
        /// - **Authorization**: JWT Token no formato `Bearer {token}` é necessário
        /// - **Content-Type**: application/json
        /// </remarks>
        /// <response code="204">Status do agendamento atualizado com sucesso.</response>
        /// <response code="400">Requisição inválida.</response>
        /// <response code="403">Acesso negado.</response>
        /// <response code="404">Agendamento não encontrado.</response>
        /// <response code="500">Erro ao processar a solicitação.</response>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Administrador,Profissional")]
        [SwaggerOperation(
            Summary = "Atualiza o status de um agendamento",
            Description = "Permite a atualização do status de um agendamento. Administradores e profissionais podem realizar esta ação."
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Status do agendamento atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Requisição inválida.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Agendamento não encontrado.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação.")]
        public async Task<ActionResult> UpdateAgendamentoStatus(int id, [FromBody] string novoStatus)
        {
            try
            {
                // Chama o serviço para atualizar o status do agendamento
                var result = await _agendamentoService.UpdateAgendamentoStatusAsync(id, novoStatus, User);

                // Verifica o resultado e retorna a resposta apropriada
                if (!result.Sucesso)
                {
                    return StatusCode(result.StatusCode, result.Mensagem);
                }

                return NoContent(); // Retornar 204 Sem conteúdo em atualização bem-sucedida
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao atualizar o status do agendamento.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Cancela um agendamento pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do agendamento.</param>
        /// <returns>Nenhum conteúdo se o cancelamento for bem sucedido.</returns>
        /// <remarks>
        /// - **Authorization**: JWT Token no formato `Bearer {token}` é necessário
        /// </remarks>
        /// <response code="204">Agendamento cancelado com sucesso.</response>
        /// <response code="403">Acesso negado (usuário não tem permissão).</response>
        /// <response code="404">Agendamento não encontrado.</response>
        /// <response code="500">Erro ao processar a solicitação.</response>
        [HttpPatch("{id}/cancelar")]
        [Authorize(Roles = "Administrador,Profissional,Usuario")]
        [SwaggerOperation(
            Summary = "Cancela um agendamento",
            Description = "Permite cancelar um agendamento. Usuários podem cancelar seus próprios compromissos, enquanto administradores e profissionais podem cancelar qualquer compromisso."
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Agendamento cancelado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Agendamento não encontrado.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação.")]
        public async Task<ActionResult> CancelAgendamento(int id)
        {
            try
            {
                // Chama o serviço para cancelar o agendamento
                var result = await _agendamentoService.CancelAgendamentoAsync(id, User);

                // Verifica o resultado e retorna a resposta apropriada
                if (!result.Sucesso)
                {
                    return StatusCode(result.StatusCode, result.Mensagem);
                }

                return NoContent(); // Retornar 204 Sem conteúdo em atualização bem-sucedida
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao cancelar o agendamento.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Recupera agendamentos para uma data específica.
        /// </summary>
        /// <param name="data">A data para filtrar agendamentos.</param>
        /// <returns>Uma lista de agendamentos para a data especificada.</returns>
        /// <remarks>
        /// - **Authorization**: JWT Token no formato `Bearer {token}` é necessário
        /// - **Regras**: Administradores e profissionais podem visualizar agendamentos de uma data específica.
        /// </remarks>
        /// <response code="200">Agendamentos retornados com sucesso.</response>
        /// <response code="403">Acesso negado (usuário não tem permissão).</response>
        /// <response code="404">Nenhum agendamento encontrado para a data especificada.</response>
        /// <response code="500">Erro ao processar a solicitação.</response>
        [HttpGet("by-date/{data}")]
        [Authorize(Roles = "Administrador,Profissional")]
        [SwaggerOperation(
            Summary = "Recupera agendamentos por data",
            Description = "Retorna todos os agendamentos para uma data específica. Apenas administradores e profissionais têm permissão para acessar este endpoint."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Agendamentos retornados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhum agendamento encontrado para a data especificada.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação.")]
        public async Task<ActionResult> GetAgendamentoByDate(DateTime data)
        {
            try
            {
                // Chama o serviço para obter os agendamentos por data
                var result = await _agendamentoService.GetAgendamentosByDateAsync(data);

                // Verifica o resultado e retorna a resposta apropriada
                if (!result.Sucesso)
                {
                    return StatusCode(result.StatusCode, result.Mensagem);
                }

                return Ok(result.Agendamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao obter agendamentos para a data especificada.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Recupera agendamentos dentro de um intervalo de datas.
        /// </summary>
        /// <param name="dataInicial">A data de início do intervalo.</param>
        /// <param name="dataFinal">A data de término do intervalo.</param>
        /// <returns>Uma lista de agendamentos dentro do intervalo de datas especificado.</returns>
        /// <remarks>
        /// - **Authorization**: JWT Token no formato `Bearer {token}` é necessário
        /// - **Regras**: Apenas administradores e profissionais podem acessar agendamentos entre um intervalo de datas.
        /// </remarks>
        /// <response code="200">Agendamentos retornados com sucesso.</response>
        /// <response code="403">Acesso negado (usuário não tem permissão).</response>
        /// <response code="404">Nenhum agendamento encontrado dentro do intervalo especificado.</response>
        /// <response code="500">Erro ao processar a solicitação.</response>
        [HttpGet("between-dates")]
        [Authorize(Roles = "Administrador,Profissional")]
        [SwaggerOperation(
            Summary = "Recupera agendamentos entre datas",
            Description = "Retorna todos os agendamentos que ocorreram dentro de um intervalo de datas. Apenas administradores e profissionais podem acessar esta informação."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Agendamentos retornados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhum agendamento encontrado dentro do intervalo especificado.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação.")]
        public async Task<ActionResult> GetBetweenDates([FromQuery] DateTime dataInicial, [FromQuery] DateTime dataFinal)
        {
            try
            {
                // Chama o serviço para obter os agendamentos entre as datas
                var result = await _agendamentoService.GetAgendamentosBetweenDatesAsync(dataInicial, dataFinal);

                // Verifica o resultado e retorna a resposta apropriada
                if (!result.Sucesso)
                {
                    return StatusCode(result.StatusCode, result.Mensagem);
                }

                return Ok(result.Agendamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao obter agendamentos para o intervalo de datas especificado.",
                    details = ex.Message
                });
            }
        }

        // passando as rotas para cada verbo
        /// <summary>
        /// Gera os links relevantes para um agendamento específico.
        /// </summary>
        /// <param name="model">O modelo de agendamento para o qual os links serão gerados.</param>
        /// <remarks>
        /// - Gera links para as operações de leitura, atualização e exclusão.
        /// </remarks>
        private void GerarLinks(Agendamento model)
        {
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "self", metodo: "GET"));
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "update", metodo: "PUT"));
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "delete", metodo: "DELETE"));

        }        
    }
}
