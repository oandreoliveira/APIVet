using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiVet.Data;
using ApiVet.DTO;
using ApiVet.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace ApiVet.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class UsuariosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        ///  <remarks>
        /// Exemplo de request:
        ///
        ///     POST /api/v1/Usuarios/Registro
        ///     {
        ///     "email": "carlos@example.com",
        ///     "senha": "carlos15",
        ///      "isCliente": true
        ///     }
        /// </remarks>
        /// <param name="usuarioTemp"></param>
        /// <returns>Retorna a inclusão de um novo usuário</returns>
        [HttpPost("Registro")]
        [Authorize(Roles = "Veterinario")]
        public IActionResult Registro([FromBody] UsuarioDTO usuarioTemp)
        {
            try
            {
                if (usuarioTemp is null) return BadRequest();
                var pesquisaUsuario = _context.Usuarios.FirstOrDefault(usuario => usuario.Email == usuarioTemp.Email);
                if (pesquisaUsuario != null) return BadRequest(new { msg = "Este Email já está cadastrado em nosso Sistema" });

                Usuario usuario = _mapper.Map<Usuario>(usuarioTemp);

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                return Ok(new { msg = "Usuário cadastrado com sucesso", usuario });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }

        }
        /// <summary>
        /// Realiza o Login de um usuário
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///
        ///     POST /api/v1/Usuarios/Login
        ///     {
        ///     "email": "carlos@example.com",
        ///     "senha": "carlos15",
        ///     }
        /// </remarks>
        /// <param name="email"></param>
        /// <param name="senha"></param>
        /// <returns>Loga o usuário no sistema</returns>

        [HttpPost("Login")]
        public IActionResult Login(string email, string senha)
        {

            try
            {
                Usuario usuario = _context.Usuarios.FirstOrDefault(user => user.Email.Equals(email));
                if (usuario != null)
                {
                    if (usuario.Senha.Equals(senha) && usuario.IsCliente == false)
                    {

                        string chaveDeSeguranca = "ApiVetGftBrasil1705";
                        var chaveSimetica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetica, SecurityAlgorithms.HmacSha256Signature);

                        var claims = new List<Claim>();
                        claims.Add(new Claim("id", usuario.Id.ToString()));
                        claims.Add(new Claim("email", usuario.Email));
                        claims.Add(new Claim(ClaimTypes.Role, "Veterinario"));

                        var JWT = new JwtSecurityToken(
                            issuer: "ApiVet.com",
                            expires: DateTime.Now.AddHours(1),
                            audience: "usuario_comum",
                            signingCredentials: credenciaisDeAcesso,
                            claims: claims
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));

                    }
                    else if (usuario.Senha.Equals(senha) && usuario.IsCliente == true)
                    {
                        string chaveDeSeguranca = "ApiVetGftBrasil1705";
                        var chaveSimetica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetica, SecurityAlgorithms.HmacSha256Signature);

                        var claims = new List<Claim>();
                        claims.Add(new Claim("id", usuario.Id.ToString()));
                        claims.Add(new Claim("email", usuario.Email));
                        claims.Add(new Claim(ClaimTypes.Role, "Cliente"));

                        var JWT = new JwtSecurityToken(
                            issuer: "ApiVet.com",
                            expires: DateTime.Now.AddHours(1),
                            audience: "usuario_comum",
                            signingCredentials: credenciaisDeAcesso,
                            claims: claims

                        );
                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));

                    }
                    else
                    {
                        Response.StatusCode = 401;
                        return new ObjectResult("");
                    }

                }
                else
                {
                    Response.StatusCode = 401;
                    return new ObjectResult("");
                }

            }
            catch (Exception)
            {
                Response.StatusCode = 401;
                return new ObjectResult("");
            }
        }
        /// <summary>
        /// Exibe todos os usuários cadastrados
        /// </summary>
        /// <returns>Retorna todos os usuários Ativos</returns>
        [HttpGet("Listar")]
        [Authorize(Roles = "Veterinario")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> Get()
        {
            try
            {
                var usuarios = await _context.Usuarios.Where(vet => vet.IsAtivo == true).AsNoTracking().ToListAsync();
                return usuarios.Any() ? Ok(new { msg = "Usuários:", usuarios }) : BadRequest("Usuários não encontrados!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Pesquisa um usuário pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna o usuário</returns>
        [HttpGet("Usuario/{id:int}")]
        public async Task<ActionResult<VeterinarioDTO>> GetById(int id)
        {
            try
            {
                var usuarios = await _context.Usuarios.FirstOrDefaultAsync(vet => vet.Id == id);
                return usuarios is null ? BadRequest($"Usuário com o Id: {id} não encontrado!") : Ok(new { msg = "Usuário solicitado:", usuarios });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Atualiza informações do usuário pelo seu Id
        /// </summary>
        /// <remarks>
        /// O que não será alterado, basta apagar!
        /// 
        /// Exemplo de request:
        ///
        ///     PATCH /api/v1/Usuários
        ///     {
        ///     "email": "carlos",
        ///      "senha": 000013,
        ///     "isAtivo": true
        ///     }
        /// </remarks>
        /// /// <param name="id"></param>
        /// <param name="usuarioDto"></param>
        /// <returns>Retorna o veterinário pesquisado atualizado </returns>
        [HttpPatch("Atualizar/{id:int}")]
        [Authorize(Roles = "Veterinario")]
        public IActionResult Patch(int id, UsuarioUpdateDTO usuarioDto)
        {
            if (id > 0)
            {
                try
                {
                    var usuario = _context.Usuarios.FirstOrDefault(usuario => usuario.Id == id);
                    if (usuario != null)
                    {

                        usuario.Email = usuarioDto.Email ?? usuario.Email;
                        usuario.Senha = usuarioDto.Senha ?? usuario.Senha;
                        usuario.IsAtivo = usuarioDto.IsAtivo == true ? usuarioDto.IsAtivo : usuario.IsAtivo;

                        var pesquisaUsuario = _context.Usuarios.FirstOrDefault(usuario => usuario.Email == usuarioDto.Email && usuario.Id != usuario.Id);
                        if (pesquisaUsuario != null) return BadRequest(new { msg = "Este email já está cadastrado em nosso Sistema" });

                        _context.SaveChanges();
                        return Ok(new { msg = "Usuário alterado:", usuario });
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new { msg = "Usuário não encontrado" });
                    }
                }
                catch
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Usuário não encontrado" });
                }
            }
            else
            {

                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O Id informado é inválido" });
            }
        }

        /// <summary>
        /// Desativa um usuario pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna o usuario inativo</returns>
        [HttpDelete("Deletar/{id}")]
        [Authorize(Roles = "Veterinario")]
        public IActionResult Delete(int id)
        {
            try
            {
                Usuario usuario = _context.Usuarios.FirstOrDefault(usuario => usuario.Id == id);
                if (usuario is null) return BadRequest($"Usuário com o Id: {id} não encontrado!");
                if (usuario.IsAtivo == false) return BadRequest($"Usuário com o Id: {id} já se encontra excluído no sistema!");
                usuario.IsAtivo = false;
                _context.Usuarios.Update(usuario);
                _context.SaveChanges();
                return Ok($"Usuário com o Id: {id} Excluído com sucesso!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");

            }
        }

    }


}