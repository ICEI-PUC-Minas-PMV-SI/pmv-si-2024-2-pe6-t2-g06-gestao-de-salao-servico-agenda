﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models
{
    public class Agendamento
    {
        [Key]
        [ScaffoldColumn(false)]
        [Required]
        public int Id { get; set; }
        [Required]
        public DateOnly DataAgendamento { get; set; }

        [Required]
        public TimeOnly HoraAgendamento { get; set; }

        // Um agendamento possui somente um usuário e um profissional - relacionamente 1 - 1
        // Relacionamento virtual para agendamentos para carregar informacoes da especializacao associados a esse profissional
        public ICollection<Profissional> Profissionais { get; set; } //-n

    }
}
