using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVet.Data;
using ApiVet.DTO;
using ApiVet.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiVet.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Authorize(Roles = "Veterinario")]
    public class VeterinariosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public VeterinariosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        /// <summary>
        /// Inclui um novo veterinário
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///
        ///     POST /api/v1/Veterinarios/Criar
        ///     {
        ///      "nome": "Carlos",
        ///      "codCrmv": 000000
        ///     }
        /// </remarks>
        /// <param name="veterinarioTemp"></param>
        /// <returns>Retorna a inclusão de um novo veterinário</returns>
        [HttpPost]
        public async Task<ActionResult> Post(VeterinarioDTO veterinarioTemp)
        {
            try
            {
                if (veterinarioTemp is null) return BadRequest();
                var pesquisaVet = await _context.Veterinarios.FirstOrDefaultAsync(vet => vet.CodCrmv == veterinarioTemp.CodCrmv);
                if (pesquisaVet != null) return BadRequest(new { msg = "Este CRVM já está cadastrado em nosso Sistema" });

                Veterinario veterinario = _mapper.Map<Veterinario>(veterinarioTemp);

                await _context.Veterinarios.AddAsync(veterinario);
                await _context.SaveChangesAsync();
                return new CreatedAtRouteResult("ObterVeterinário", new { id = veterinario.Id }, veterinario);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Exibe todos os veterinários cadastrados
        /// </summary>
        /// <returns>Retorna todos os veterinários Ativos</returns>
        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<VeterinarioDTO>>> Get()
        {
            try
            {
                var veterinarios = await _context.Veterinarios.Where(vet => vet.IsAtivo == true).AsNoTracking().ToListAsync();
                return veterinarios.Any() ? Ok(new { msg = "Veterinários:", veterinarios }) : BadRequest("Veterinários não encontrados!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Pesquisa um veterinário pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna o veterinário</returns>
        [HttpGet("Veterinario/{id:int}", Name = "ObterVeterinário")]
        public async Task<ActionResult<VeterinarioDTO>> GetById(int id)
        {
            try
            {
                var veterinario = await _context.Veterinarios.FirstOrDefaultAsync(vet => vet.Id == id);
                return veterinario is null ? BadRequest($"Veterinário com o Id: {id} não encontrado!") : Ok(new { msg = "Veterinário solicitado:", veterinario });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Pesquisa um veterinário pelo seu Crmv
        /// </summary>
        /// <param name="Crmv"></param>
        /// <returns>Retorna o veterinário</returns>
        [HttpGet("Veterinario/CRMV{Crmv}")]  // retorna Todas as consultas dos cachorros de um mesmo dono de forma decrescente;
        public async Task<ActionResult<VeterinarioDTO>> GetByCrmv(int Crmv)
        {
            try
            {
                var veterinario = await _context.Veterinarios.Where(vet => vet.CodCrmv == Crmv).ToListAsync();
                return veterinario.Any() ? Ok(new { msg = "Veterinário solicitado:", veterinario }) : BadRequest($"Código CRMV: {Crmv} não encontrado!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Atualiza informações do veterinário pelo seu Id
        /// </summary>
        ///  <remarks>
        /// O que não será alterado, basta apagar!
        /// 
        /// Exemplo de request:
        ///
        ///     PATCH /api/v1/Veterinarios/Atualizar
        ///     {
        ///      "nome": "Carlos",
        ///      "codCrmv": 000013,
        ///      "isAtivo": true
        ///     }
        /// </remarks>
        /// /// <param name="id"></param>
        /// <param name="veterinarioDto"></param>
        /// <returns>Retorna o veterinário pesquisado atualizado </returns>
        [HttpPatch("Atualizar/{id:int}")]
        public IActionResult Patch(int id, VeterinarioUpdateDTO veterinarioDto)
        {
            if (id > 0)
            {
                try
                {
                    var veterinario = _context.Veterinarios.FirstOrDefault(vet => vet.Id == id);
                    if (veterinario != null)
                    {

                        veterinario.Nome = veterinarioDto.Nome ?? veterinario.Nome;
                        veterinario.CodCrmv = veterinarioDto.CodCrmv != 0 ? veterinarioDto.CodCrmv : veterinario.CodCrmv;
                        veterinario.IsAtivo = veterinarioDto.IsAtivo == true ? veterinarioDto.IsAtivo : veterinario.IsAtivo;

                        var pesquisaVet = _context.Veterinarios.FirstOrDefault(vet => vet.CodCrmv == veterinarioDto.CodCrmv && vet.Id != veterinario.Id);
                        if (pesquisaVet != null) return BadRequest(new { msg = "Este CRVM já está cadastrado em nosso Sistema" });

                        _context.SaveChanges();
                        return Ok(new { msg = "Veterinário alterado:", veterinario });
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new { msg = "Veterinário não encontrado" });
                    }
                }
                catch
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Veterinário não encontrado" });
                }
            }
            else
            {

                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O Id informado é inválido" });
            }
        }

        /// <summary>
        /// Desativa um veterinário pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna o veterinário inativo</returns>
        [HttpDelete("Deletar/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Veterinario veterinario = _context.Veterinarios.FirstOrDefault(vet => vet.Id == id);
                if (veterinario is null) return BadRequest($"Veterinário com o Id: {id} não encontrado!");
                if (veterinario.IsAtivo == false) return BadRequest($"Veterinário com o Id: {id} já se encontra excluído no sistema!");
                veterinario.IsAtivo = false;
                _context.Veterinarios.Update(veterinario);
                _context.SaveChanges();
                return Ok($"Veterinário com o Id: {id} Excluído com sucesso!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");

            }
        }


    }
}