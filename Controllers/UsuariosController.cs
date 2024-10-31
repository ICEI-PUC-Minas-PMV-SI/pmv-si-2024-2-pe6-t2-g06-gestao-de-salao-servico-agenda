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
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        // Construtor onde o IConfiguration e o ApplicationDbContext são injetados
        public UsuariosController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetById(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> Create(UsuarioDto usuario)
        {
            // passar o usuario dto ao inves do usuario. essa configuracao garante que a senha tambem sera passada
            Usuario novoUsuario = new Usuario()
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha), //criptografia da senha quando salva no banco
                Telefone = usuario.Telefone,
                Endereco = usuario.Email,
                Cidade = usuario.Cidade,
                Estado = usuario.Estado,
                Cep = usuario.Cep,
                DataNascimento = usuario.DataNascimento,
                Genero = usuario.Genero,
                Perfil = usuario.Perfil,
                Cnpj = usuario.Cnpj,
            };

            // passar a camada de dto
            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = novoUsuario.Id }, novoUsuario);
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UsuarioDto usuario)
        {
            if (id != usuario.Id) return BadRequest();

            var modeloDb = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (modeloDb == null) return NotFound();

            modeloDb.Nome = usuario.Nome;
            modeloDb.Email = usuario.Email;
            modeloDb.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha); //criptografia da senha quando salva no banco
            modeloDb.Telefone = usuario.Telefone;
            modeloDb.Endereco = usuario.Email;
            modeloDb.Cidade = usuario.Cidade;
            modeloDb.Estado = usuario.Estado;
            modeloDb.Cep = usuario.Cep;
            modeloDb.DataNascimento = usuario.DataNascimento;
            modeloDb.Genero = usuario.Genero;
            modeloDb.Perfil = usuario.Perfil;
            modeloDb.Cnpj = usuario.Cnpj;

            _context.Usuarios.Update(modeloDb);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        [AllowAnonymous]
        [HttpPost("authentication")]
        public async Task<ActionResult> Authenticate(AuthenticateDto model)
        {
            var modelUsuarioDb = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);


            if (modelUsuarioDb == null || model.Email != modelUsuarioDb.Email || !BCrypt.Net.BCrypt.Verify(model.Senha, modelUsuarioDb.Senha))
                return Unauthorized();

            var jwt = GenerateJwtToken(modelUsuarioDb);
            return Ok(new
            {
                jwtToken = jwt,                
                userId = modelUsuarioDb.Id 
            });
        }

        private string GenerateJwtToken(Usuario model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Usando _configuration corretamente

            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, model.Nome.ToString()),
                new Claim(ClaimTypes.Role, model.Perfil.ToString())
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(8),
                // Usando _configuration para obter o Audience
                //Audience = _configuration["Jwt:Audience"], // Defina o público do token aqui
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
