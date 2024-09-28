using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data.Repositories;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities;
using System.Security.Claims;
// Logica de negocios
namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Services

{
    public class AgendamentosService : IAgendamentoService
    {
        private readonly IAgendamentosRepository _repository;

        public AgendamentosService(IAgendamentosRepository repository)
        {
            _repository = repository;
        }

        // Implementação para obter os agendamentos paginados
        public async Task<List<Agendamento>> GetAllAgendamentosAsync(int pageNumber, int pageSize)
        {
            // Calcular o número de itens a serem ignorados
            int skip = (pageNumber - 1) * pageSize;

            // Chama o repositório para obter os agendamentos paginados
            var agendamentos = await _repository.GetPagedAgendamentosAsync(skip, pageSize);

            return agendamentos;
        }
        //public async Task<IEnumerable<Agendamento>> GetAllAgendamentosAsync()
        //{
        //    try
        //    {
        //        return await _repository.GetAllAgendamentosAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Optionally, log the exception
        //        throw new Exception("Erro ao obter os agendamentos", ex);
        //    }
        //}
        public async Task<Agendamento> GetAgendamentoByIdAsync(int id, string usuarioAtualId, string perfilUsuarioAtual)
        {
            try
            {
                // Recupera o agendamento pelo ID usando o repositório
                var agendamento = await _repository.GetAgendamentoByIdAsync(id);

                // Retorna null se o agendamento não for encontrado
                if (agendamento == null)
                    return null;

                // Verifica se o usuário atual é "Administrador"
                if (perfilUsuarioAtual == "Administrador")
                {
                    // Administradores podem acessar qualquer agendamento
                    return agendamento;
                }

                // Se o usuário for "Profissional", verifica se o ID do profissional coincide
                else if (perfilUsuarioAtual == "Profissional")
                {
                    var profissionalId = int.Parse(usuarioAtualId);

                    if (agendamento.ProfissionalId != profissionalId)
                    {
                        throw new UnauthorizedAccessException("Função inválida: Profissional não autorizado.");
                    }

                    return agendamento;
                }

                // Se o perfil do usuário for inválido, lança uma exceção
                throw new UnauthorizedAccessException("Você não tem permissão para acessar este agendamento.");
            }
            catch (Exception ex)
            {
                // Opcional: logar o erro
                // _logger.LogError(ex, "Erro ao obter o agendamento com id {id}", id);
                throw new Exception($"Erro ao obter o agendamento: {ex.Message}", ex);
            }
        }
        public async Task<(bool Sucesso, string Mensagem, int StatusCode)> CreateAgendamentoAsync(Agendamento agendamento, ClaimsPrincipal usuarioAtual)
        {
            // Validação de estado do modelo (pode ser mantida aqui ou no Controller)
            if (agendamento == null)
            {
                return (false, "Dados do agendamento inválidos.", 400); // BadRequest
            }

            // Obtém o ID do usuário e o perfil (role) do token JWT
            var usuarioAtualId = usuarioAtual.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfilUsuarioAtual = usuarioAtual.FindFirst(ClaimTypes.Role)?.Value;

            // Regra para usuários comuns: só podem criar agendamento para eles mesmos
            if (perfilUsuarioAtual == "Usuario")
            {
                if (agendamento.UsuarioId.ToString() != usuarioAtualId)
                {
                    return (false, "Você não tem permissão para criar agendamentos para outros usuários.", 403); // Forbidden
                }
            }

            // Outros perfis (Administrador e Profissional) podem criar agendamentos sem restrição

            try
            {
                // Adiciona o agendamento no repositório
                await _repository.CreateAgendamentoAsync(agendamento);

                // Sucesso: retorno sem erro
                return (true, null, 201); // Created
            }
            catch (Exception ex)
            {
                // Em caso de erro, captura a exceção e retorna o status 500
                return (false, $"Erro ao criar agendamento: {ex.Message}", 500); // Internal Server Error
            }
        }
        public async Task<(bool Sucesso, string Mensagem, int StatusCode)> UpdateAgendamentoAsync(int id, Agendamento agendamento, ClaimsPrincipal usuarioAtual)
        {
            // Valida se o ID do agendamento corresponde
            if (id != agendamento.Id)
            {
                return (false, "O ID do agendamento não corresponde.", 400); // BadRequest
            }

            // Obtém o ID e o perfil do usuário a partir dos claims
            var usuarioAtualId = usuarioAtual.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfilUsuarioAtual = usuarioAtual.FindFirst(ClaimTypes.Role)?.Value;

            // Busca o agendamento existente (sem rastreamento para evitar conflitos de estado)
            var agendamentoExistente = await _repository.GetAgendamentoByIdNoTrackingAsync(id);

            if (agendamentoExistente == null)
            {
                return (false, "Agendamento não encontrado.", 404); // NotFound
            }

            // Verifica as permissões de acordo com o perfil do usuário
            if (perfilUsuarioAtual == "Usuario")
            {
                // Usuários comuns só podem atualizar seus próprios agendamentos
                if (agendamentoExistente.UsuarioId.ToString() != usuarioAtualId)
                {
                    return (false, "Você não pode atualizar agendamentos de outros usuários.", 403); // Forbidden
                }
            }

            // Atualiza o agendamento
            try
            {
                await _repository.UpdateAgendamentoAsync(agendamento);
                return (true, null, 204); // NoContent em caso de sucesso
            }
            catch (Exception ex)
            {
                // Captura a exceção e retorna o status 500 com a mensagem de erro
                return (false, $"Erro ao atualizar agendamento: {ex.Message}", 500); // Internal Server Error
            }
        }
        public async Task<(bool Sucesso, string Mensagem, int StatusCode)> DeleteAgendamentoAsync(int id, ClaimsPrincipal usuarioAtual)
        {
            // Obtém o ID e o perfil do usuário a partir dos claims
            var usuarioAtualId = usuarioAtual.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfilUsuarioAtual = usuarioAtual.FindFirst(ClaimTypes.Role)?.Value;

            // Busca o agendamento por ID
            var agendamento = await _repository.GetAgendamentoByIdNoTrackingAsync(id);

            if (agendamento == null)
            {
                return (false, "Agendamento não encontrado.", 404); // NotFound
            }

            // Verifica as permissões de acordo com o perfil do usuário
            if (perfilUsuarioAtual == "Usuario")
            {
                // Usuários comuns só podem deletar seus próprios agendamentos
                if (agendamento.UsuarioId.ToString() != usuarioAtualId)
                {
                    return (false, "Você não pode deletar agendamentos de outros usuários.", 403); // Forbidden
                }
            }

            // Tenta remover o agendamento
            try
            {
                await _repository.DeleteAgendamentoAsync(agendamento);
                return (true, null, 204); // NoContent em caso de sucesso
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao deletar agendamento: {ex.Message}", 500); // Internal Server Error
            }
        }
        public async Task<(bool Sucesso, string Mensagem, int StatusCode, IEnumerable<Agendamento> Agendamentos)> GetAgendamentosByUsuarioIdAsync(int id, ClaimsPrincipal usuarioAtual)
        {
            // Obtém o ID e o perfil do usuário a partir dos claims
            var usuarioAtualId = usuarioAtual.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfilUsuarioAtual = usuarioAtual.FindFirst(ClaimTypes.Role)?.Value;

            // Verifica permissões de acordo com o perfil do usuário
            if (perfilUsuarioAtual == "Administrador")
            {
                // Administrador pode acessar agendamentos de qualquer usuário
            }
            else if (perfilUsuarioAtual == "Profissional")
            {
                var profissionalId = int.Parse(usuarioAtualId);

                if (profissionalId != id)
                {
                    return (false, "Você não tem permissão para acessar agendamentos de outro profissional.", 403, null);
                }
            }
            else if (perfilUsuarioAtual == "Usuario")
            {
                if (usuarioAtualId != id.ToString())
                {
                    return (false, "Você não tem permissão para acessar os agendamentos de outro usuário.", 403, null);
                }
            }
            else
            {
                return (false, "Função de usuário inválida.", 401, null); // Unauthorized
            }

            // Busca os agendamentos para o usuário ou profissional
            var agendamentos = await _repository.GetAgendamentosByUsuarioIdAsync(id);

            if (agendamentos == null || !agendamentos.Any())
            {
                return (false, "Nenhum agendamento encontrado para este usuário.", 404, null); // NotFound
            }

            return (true, null, 200, agendamentos); // Retorna com sucesso e a lista de agendamentos
        }

        public async Task<(bool Sucesso, string Mensagem, int StatusCode, IEnumerable<Agendamento> Agendamentos)> GetAgendamentosByProfissionalIdAsync(int id, ClaimsPrincipal usuarioAtual)
        {
            // Obtém o perfil do usuário atual a partir dos claims
            var perfilUsuarioAtual = usuarioAtual.FindFirst(ClaimTypes.Role)?.Value;
            var usuarioAtualId = usuarioAtual.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verifica se o usuário é um profissional ou administrador
            if (perfilUsuarioAtual == "Profissional")
            {
                // Verifica se o profissional está tentando acessar seus próprios agendamentos
                if (usuarioAtualId != id.ToString())
                {
                    return (false, "Você não tem permissão para acessar os agendamentos de outro profissional.", 403, null); // Forbidden
                }
            }
            else if (perfilUsuarioAtual != "Administrador")
            {
                return (false, "Função de usuário inválida.", 401, null); // Unauthorized
            }

            // Busca os agendamentos para o profissional informado
            var agendamentos = await _repository.GetAgendamentosByProfissionalIdAsync(id);

            if (agendamentos == null || !agendamentos.Any())
            {
                return (false, "Nenhum agendamento encontrado para o profissional especificado.", 404, null); // NotFound
            }

            return (true, null, 200, agendamentos); // Success
        }

        public async Task<(bool Sucesso, string Mensagem, int StatusCode)> UpdateAgendamentoStatusAsync(int id, string novoStatus, ClaimsPrincipal usuarioAtual)
        {
            // Valida o status de entrada
            if (string.IsNullOrWhiteSpace(novoStatus))
            {
                return (false, "O status fornecido é inválido.", 400); // Bad Request
            }

            // Busca o agendamento por ID
            var agendamento = await _repository.GetAgendamentoByIdNoTrackingAsync(id);

            if (agendamento == null)
            {
                return (false, "Agendamento não encontrado.", 404); // Not Found
            }

            // Verifica o perfil do usuário atual
            var perfilUsuarioAtual = usuarioAtual.FindFirst(ClaimTypes.Role)?.Value;

            // Permite apenas Administradores e Profissionais para atualizar o status
            if (perfilUsuarioAtual != "Administrador" && perfilUsuarioAtual != "Profissional")
            {
                return (false, "Você não tem permissão para atualizar o status do agendamento.", 403); // Forbidden
            }

            // Atualiza o status do agendamento
            agendamento.Status = novoStatus;

            // Marca a entidade como modificada
            await _repository.UpdateAgendamentoAsync(agendamento);

            return (true, null, 204); // Success, No Content
        }

        public async Task<(bool Sucesso, string Mensagem, int StatusCode)> CancelAgendamentoAsync(int id, ClaimsPrincipal usuarioAtual)
        {
            const string cancellationStatus = "Cancelado"; // Define o status de cancelamento

            // Busca o agendamento por ID
            var agendamento = await _repository.GetAgendamentoByIdNoTrackingAsync(id);

            if (agendamento == null)
            {
                return (false, "Agendamento não encontrado.", 404); // Not Found
            }

            var userId = usuarioAtual.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verifica se o usuário atual é um "Usuario" (Role 1)
            if (usuarioAtual.IsInRole("Usuario")) // Role 1: Usuario
            {
                // Verifica se o usuário está cancelando seu próprio compromisso
                if (int.Parse(userId) != agendamento.UsuarioId)
                {
                    return (false, "Você não tem permissão para cancelar este compromisso.", 403); // Forbidden
                }
            }

            // Atualiza o status para cancelado
            agendamento.Status = cancellationStatus;

            // Marca a entidade como modificada
            await _repository.UpdateAgendamentoAsync(agendamento);

            return (true, null, 204); // Success, No Content
        }
        public async Task<(bool Sucesso, string Mensagem, int StatusCode, IEnumerable<Agendamento> Agendamentos)> GetAgendamentosByDateAsync(DateTime data)
        {
            // Busca os agendamentos para a data especificada
            var agendamentos = await _repository.GetAgendamentosByDateAsync(data);

            if (!agendamentos.Any())
            {
                return (false, "Nenhum agendamento encontrado para a data especificada.", 404, null); // Not Found
            }

            return (true, null, 200, agendamentos); // Sucesso
        }
        public async Task<(bool Sucesso, string Mensagem, int StatusCode, IEnumerable<Agendamento> Agendamentos)> GetAgendamentosBetweenDatesAsync(DateTime dataInicial, DateTime dataFinal)
        {
            // Busca os agendamentos dentro do intervalo de datas especificado
            var agendamentos = await _repository.GetAgendamentosBetweenDatesAsync(dataInicial, dataFinal);

            if (!agendamentos.Any())
            {
                return (false, "Nenhum agendamento encontrado para o intervalo de datas especificado.", 404, null); // Not Found
            }

            return (true, null, 200, agendamentos); // Sucesso
        }
    }
}
