using Microsoft.AspNetCore.Mvc;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    [ApiController]
    public abstract class EntidadeBaseController<TEntity, TDto> : ControllerBase
    where TEntity : BaseEntity
    where TDto : EntityDto
    {
        protected readonly EntidadeService<TEntity, TDto> _service;

        public EntityController(EntidadeService<TEntity, TDto> service)
        {
            _service = service;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(TDto dto)
        {
            var entity = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(Guid id, TDto dto)
        {
            var updatedEntity = await _service.UpdateAsync(id, dto);
            if (updatedEntity == null) return NotFound();
            return Ok(updatedEntity);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var entities = await _service.GetAllAsync();
            return Ok(entities);
        }
    }

}
