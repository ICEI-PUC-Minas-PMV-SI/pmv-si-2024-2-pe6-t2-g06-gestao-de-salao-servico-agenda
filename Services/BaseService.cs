using AgendamentoService.Repositories;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.DTOs;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;

public abstract class BaseService<TEntity, TDto> where TEntity : EntidadeBase where TDto : EntidadeBaseDto
{
    protected readonly AgendamentoService.Repositories.EntidadeBaseRepository<TEntity> _repository;

    public BaseService(AgendamentoService.Repositories.EntidadeBaseRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(e => MapToDto(e)).ToList();
    }

    public virtual async Task<TDto> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : MapToDto(entity);
    }

    public virtual async Task<TDto> CreateAsync(TDto dto)
    {
        var entity = MapToEntity(dto);
        await _repository.AddAsync(entity);
        return MapToDto(entity);
    }

    public virtual async Task<TDto> UpdateAsync(Guid id, TDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return null;

        UpdateEntity(entity, dto);
        await _repository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return false;

        await _repository.DeleteAsync(entity);
        return true;
    }

    protected abstract TEntity MapToEntity(TDto dto);
    protected abstract TDto MapToDto(TEntity entity);
    protected abstract void UpdateEntity(TEntity entity, TDto dto);
}
