using AgendamentoService.Repositories;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.DTOs;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;

public class ClienteeService : EntidadeService<Cliente, ClientDto>
{
    public ClienteeService(AgendamentoService.Repositories.EntidadeBaseRepository<Cliente> repository)
        : base(repository) { }

    protected override Cliente MapToEntity(ClientDto dto)
    {
        var entity = base.MapToEntity(dto) as Cliente;
        entity.DataNascimento = dto.DataNascimento;
        entity.Genero = dto.Genero;
        return entity;
    }

    protected override ClientDto MapToDto(Cliente entity)
    {
        var dto = base.MapToDto(entity) as ClientDto;
        dto.DataNascimento = entity.DataNascimento;
        dto.Genero = entity.Genero;
        return dto;
    }

    protected override void UpdateEntity(Cliente entity, ClientDto dto)
    {
        base.UpdateEntity(entity, dto);
        entity.DataNascimento = dto.DataNascimento;
        entity.Genero = dto.Genero;
    }
}
