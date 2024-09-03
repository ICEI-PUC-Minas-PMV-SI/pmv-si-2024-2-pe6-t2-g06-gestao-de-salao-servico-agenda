using AgendamentoService.Repositories;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.DTOs;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;

public class EntidadeService<TEntity, TDto> : BaseService<TEntity, TDto>
    where TEntity : EntidadeBase
    where TDto : EntidadeBaseDto
{
    public EntidadeService(AgendamentoService.Repositories.EntidadeBaseRepository<TEntity> repository)
        : base(repository) { }

    protected override TEntity MapToEntity(TDto dto)
    {
        var entity = Activator.CreateInstance<TEntity>();
        entity.Id = dto.Id != Guid.Empty ? dto.Id : Guid.NewGuid();
        entity.Nome = dto.Nome;
        entity.Email = dto.Email;
        entity.Telefone = dto.Telefone;
        entity.Endereco = dto.Endereco;
        entity.Cidade = dto.Cidade;
        entity.Estado = dto.Estado;
        entity.Cep = dto.Cep;
        return entity;
    }

    protected override TDto MapToDto(TEntity entity)
    {
        var dto = Activator.CreateInstance<TDto>();
        dto.Id = entity.Id;
        dto.Nome = entity.Nome;
        dto.Email = entity.Email;
        dto.Telefone = entity.Telefone;
        dto.Endereco = entity.Endereco;
        dto.Cidade = entity.Cidade;
        dto.Estado = entity.Estado;
        dto.Cep = entity.Cep;
        return dto;
    }

    protected override void UpdateEntity(TEntity entity, TDto dto)
    {
        entity.Nome = dto.Nome;
        entity.Email = dto.Email;
        entity.Telefone = dto.Telefone;
        entity.Endereco = dto.Endereco;
        entity.Cidade = dto.Cidade;
        entity.Estado = dto.Estado;
        entity.Cep = dto.Cep;
    }
}
