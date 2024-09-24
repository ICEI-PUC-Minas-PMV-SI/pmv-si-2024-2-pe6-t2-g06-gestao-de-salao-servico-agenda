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
using System.Security.Claims;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    // AgendamentosController.cs
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
        /// Retorna todo os agendamentos do salão.
        /// </summary>
        /// <returns>Uma lista de agendamentos.</returns>
        /// <remarks> Somente Administradores tem permissao para ver todos os agendamentos.</remarks>
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> GetAllAgendamentos()
        {
            try
            {
                var agendamentos = await _agendamentoService.GetAllAgendamentosAsync();
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
                // Chama o service para criar o agendamento
                var result = await _agendamentoService.CreateAgendamentoAsync(agendamento, User);

                // Se o resultado for um erro de autorização ou dados inválidos, retorna o código adequado
                if (!result.Sucesso)
                {
                    return StatusCode(result.StatusCode, result.Mensagem);
                }

                // Retorna status 201 Created com o ID do recurso criado
                return CreatedAtAction(nameof(GetAgendamentoById), new { id = agendamento.Id }, agendamento);
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
        /// Atualiza um agendamento agendamento existente pelo seu ID.
        /// </summary>
        /// <param name="id">o ID do agendamento.</param>
        /// <param name="model">A atualização dos detalhes do agendamento.</param>
        /// <returns>Se update foi bem sucedido, não retorna nada.</returns>
        /// <remarks> Administradores e Profissionais podem atualizar agendamentos para outros usuarios, mas um usuario final nao pode atualizar agendamento para outro usuario, mas somente para o seu proprio login</remarks>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Profissional,Usuario")]
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
        /// <returns>Se a ação Delete é bem sucedida, não retorna nada.</returns>
        /// <remarks> Administradores e Profissionais podem deletar agendamentos para outros usuarios, mas um usuario final nao pode deletar agendamento para outro usuario, mas somente para o seu proprio login</remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Profissional,Usuario")] // Permite todos os perfis (1: Usuario, 2: Profissional, 3: Administrador)
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

        // GET: /api/agendamentos/usuario/{id}
        /// <summary>
        /// Retorna os agendamentos de um usuario pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do usuario para filtrar agendamentos.</param>
        /// <returns>Uma lista de agendamentos para um usuário específico.</returns>
        [HttpGet("usuario/{id}")]
        [Authorize(Roles = "Administrador,Profissional,Usuario")] // Somente usuarios autenticados podem acessar esse methodo
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
        /// <returns>Nenhum conteúdo se a atualização for bem sucedida.</returns>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Administrador,Profissional")] // Permitir que profissionais e administradores atualizem o status
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
        [HttpPatch("{id}/cancelar")]
        [Authorize(Roles = "Administrador,Profissional,Usuario")] // Permitir que usuários (para seus próprios compromissos), profissionais e administradores cancelem compromissos
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
        [HttpGet("by-date/{data}")]
        [Authorize(Roles = "Administrador,Profissional")] //Permitir que profissionais e administradores acessem este endpoint
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
        [HttpGet("between-dates")]
        [Authorize(Roles = "Administrador,Profissional")] // Permitir que profissionais e administradores acessem este endpoint
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
        private void GerarLinks(Agendamento model)
        {
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "self", metodo: "GET"));
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "update", metodo: "PUT"));
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "delete", metodo: "DELETE"));

        }        
    }
}
