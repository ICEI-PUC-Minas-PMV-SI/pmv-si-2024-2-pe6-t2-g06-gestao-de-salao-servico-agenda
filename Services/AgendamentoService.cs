using AgendamentoService.Repositories;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.DTOs;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Services
{
    public class AgendamentoService : BaseService<Agendamento, AgendamentoDto>
    {
        public AgendamentoService(AgendamentoService<Agendamento> repository) : base(repository) { }

        protected override Agendamento MapToEntity(AgendamentoDto dto)
        {
            return new Agendamento
            {
                Id = Guid.NewGuid(),
                Data = dto.Data,
                Status = dto.Status,
                ProfissionalId = dto.ProfissionalId,
                ClienteId = dto.ClienteId
            };
        }

        protected override AgendamentoDto MapToDto(Agendamento entity)
        {
            return new AgendamentoDto
            {
                Id = entity.Id,
                Data = entity.Data,
                Status = entity.Status,
                ProfissionalId = entity.ProfissionalId,
                ClienteId = entity.ClienteId
            };
        }

        protected override void UpdateEntity(Agendamento entity, AgendamentoDto dto)
        {
            entity.Data = dto.Data;
            entity.Status = dto.Status;
            entity.ProfissionalId = dto.ProfissionalId;
            entity.ClienteId = dto.ClienteId;
        }
    }
}