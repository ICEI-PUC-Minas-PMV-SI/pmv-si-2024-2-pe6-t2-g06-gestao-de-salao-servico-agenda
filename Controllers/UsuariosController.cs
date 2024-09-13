using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
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
                Perfil = usuario.Perfil
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
    }
}
