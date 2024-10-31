using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.DTOs;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Text;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaloesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        // Construtor onde o IConfiguration e o ApplicationDbContext são injetados
        public SaloesController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: api/Saloes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Salao>>> GetAll()
        {
            return await _context.Saloes.ToListAsync();
        }

        // GET: api/Saloes/123-456
        [HttpGet("{cnpj}")]
        public async Task<ActionResult<Salao>> GetById(string cnpj)
        {
            var salaoEncontrado = await _context.Saloes.FindAsync(cnpj);

            if (salaoEncontrado == null)
            {
                return NotFound();
            }

            return salaoEncontrado;
        }
    }
}
