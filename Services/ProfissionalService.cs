using AgendamentoService.Repositories;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.DTOs;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;

// Contém a lógica de negócios para os profissionais.
public class ProfissionalService : EntidadeService<Profissional, ProfissionalDto>
{
    public ProfissionalService(AgendamentoService.Repositories.EntidadeBaseRepository<Profissional> repository)
        : base(repository) { }

    protected override Profissional MapToEntity(ProfissionalDto dto)
    {
        var entity = base.MapToEntity(dto) as Profissional;
        entity.Especializacao = dto.Especializacao;
        entity.NumeroLicenca = dto.NumeroLicenca;
        entity.DataNascimento = dto.DataNascimento;
        return entity;
    }

    protected override ProfissionalDto MapToDto(Profissional entity)
    {
        var dto = base.MapToDto(entity) as ProfissionalDto;
        dto.Especializacao = entity.Especializacao;
        dto.NumeroLicenca = entity.NumeroLicenca;
        dto.DataNascimento = entity.DataNascimento;
        return dto;
    }

    protected override void UpdateEntity(Profissional entity, ProfissionalDto dto)
    {
        base.UpdateEntity(entity, dto);
        entity.Especializacao = dto.Especializacao;
        entity.NumeroLicenca = dto.NumeroLicenca;
        entity.DataNascimento = dto.DataNascimento;
    }
}
