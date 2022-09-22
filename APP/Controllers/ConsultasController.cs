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
    public class ConsultasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public ConsultasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Cadastra uma vova consulta
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///
        ///     POST /api/v1/Consultas/Criar
        ///     {
        ///     "pesoAtual": 10,
        ///     "sintomas": "Informar sintomas do animal",
        ///     "diagnostico": "Informar diagnóstico",
        ///     "comentarios": "Informar Tratamento",
        ///     "veterinarioId": 1,
        ///     "cachorroId": 2,
        ///     "clienteID": 1    
        ///     }
        /// </remarks>
        /// <param name="consultaTemp"></param>
        /// <returns>Retorna a inclusão de uma nova consulta</returns>
        [Authorize(Roles = "Veterinario")]
        [HttpPost("Criar")]
        public ActionResult Post(ConsultaDTO consultaTemp)
        {
            try
            {
                if (consultaTemp is null) return BadRequest();

                Consulta consulta = _mapper.Map<Consulta>(consultaTemp);

                _context.Consultas.Add(consulta);
                _context.SaveChanges();
                //return StatusCode(201, consulta);
                return new CreatedAtRouteResult("ObterConsulta", new { id = consulta.Id }, consulta);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Exibe todos as consultas cadastradas
        /// </summary>
        /// <returns>Retorna todos as consultas</returns>
        [Authorize(Roles = "Veterinario")]
        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<ConsultaDTO>>> Get()
        {
            try
            {
                var consultas = await _context.Consultas.Include(dog => dog.Cachorro).Include(vet => vet.Veterinario).Include(cli => cli.Cliente).Where(consulta => consulta.IsAtivo == true).AsNoTracking().ToListAsync();
                return consultas.Any() ? Ok(new { msg = "Consultas:", consultas }) : BadRequest("Consultas não encontradas!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Pesquisa uma consulta pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna a consulta</returns>
        [HttpGet("Listar/{id:int}", Name = "ObterConsulta")]
        [Authorize(Roles = "Veterinario")]
        public async Task<ActionResult<ConsultaDTO>> GetById(int id)
        {
            try
            {
                var consulta = await _context.Consultas.Include(dog => dog.Cachorro).Include(vet => vet.Veterinario).FirstOrDefaultAsync(cons => cons.Id == id);
                return consulta is null ? BadRequest($"Consulta com o Id: {id} não encontrado!") : Ok(new { msg = "Consulta solicitada:", consulta });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Pesquisa consultas pelo Id do cachorro
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna todas as consultas do cachorro pesquisado</returns>
        [HttpGet("Listar/Cachorro/{id:int}")]
        [Authorize(Roles = "Veterinario")]
        public async Task<ActionResult<IEnumerable<ConsultaDTO>>> GetByIdCachorro(int id)
        {
            try
            {
                var consulta = await _context.Consultas.Include(dog => dog.Cachorro).Where(cons => cons.Cachorro.Id == id).Include(vet => vet.Veterinario).Include(cli => cli.Cliente).OrderByDescending(id => id.Data).ToListAsync();
                return consulta.Any() ? Ok(new { msg = "Consultas solicitadas:", consulta }) : BadRequest($"Consultas com o Id do cachorro: {id} não encontradas!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Pesquisa consultas pelo Crmv do veterinário
        /// </summary>
        /// <param name="CodCrmv"></param>
        /// <returns>Retorna todas as consultas do veterinário pesquisado em ordem decrescente</returns>
        [HttpGet("Listar/Veterinario/{CodCrmv}")]
        [Authorize(Roles = "Veterinario")]
        public async Task<ActionResult<IEnumerable<ConsultaDTO>>> GetByCodCrmv(int CodCrmv)
        {
            try
            {
                var consulta = await _context.Consultas.Include(dog => dog.Cachorro).Include(vet => vet.Veterinario).Where(cons => cons.Veterinario.CodCrmv == CodCrmv).Include(cli => cli.Cliente).OrderByDescending(id => id.Data).ToListAsync();
                return consulta.Any() ? Ok(new { msg = "Consultas solicitadas:", consulta }) : BadRequest($"Consultas com o Código CRMV: {CodCrmv} não encontradas!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Pesquisa consultas pelo Cpf do cliente
        /// </summary>
        /// <param name="Cpf"></param>
        /// <returns>Retorna todas as consultas do Cpf do cliente pesquisado em ordem decrescente</returns>
        [HttpGet("Listar/Cliente/{Cpf}")]
        [Authorize(Roles = "Veterinario, Cliente")]
        public async Task<ActionResult<IEnumerable<ConsultaDTO>>> GetByCpfCliente(string Cpf)
        {
            try
            {
                var consulta = await _context.Consultas.Include(dog => dog.Cachorro).Where(cons => cons.Cachorro.Cliente.CPF == Cpf).Include(vet => vet.Veterinario).Include(cli => cli.Cliente).OrderByDescending(id => id.Data).ToListAsync();
                return consulta.Any() ? Ok(new { msg = "Consultas solicitadas:", consulta }) : BadRequest($"Consultas com Cpf: {Cpf} não encontradas!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Atualiza informações da consulta pelo seu Id
        /// </summary>
        /// <remarks>
        /// O que não será alterado, basta apagar!
        /// 
        /// Exemplo de request:
        /// 
        ///     PATCH /api/v1/Consultas/Atualizar
        ///     {
        ///     "pesoAtual": 20,
        ///     "sintomas": "Informar sintomas do animal",
        ///     "diagnostico": "Informar diagnóstico",
        ///     "comentarios": "Informar Tratamento",
        ///     "veterinarioId": 1,
        ///     "cachorroId": 2,
        ///     "clienteID": 1,
        ///     "isAtivo": true      
        ///      }
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="consultaDto"></param>
        /// <returns>Retorna a consulta atualizada</returns>
        [HttpPatch("Atualizar/{id:int}")]
        [Authorize(Roles = "Veterinario")]
        public IActionResult Patch(int id, ConsultaUpdateDTO consultaDto)
        {
            if (id > 0)
            {
                try
                {
                    var consulta = _context.Consultas.FirstOrDefault(dog => dog.Id == id);
                    if (consulta != null)
                    {

                        consulta.PesoAtual = consultaDto.PesoAtual != 0 ? consultaDto.PesoAtual : consulta.PesoAtual;
                        consulta.Sintomas = consultaDto.Sintomas ?? consulta.Sintomas;
                        consulta.Diagnostico = consultaDto.Diagnostico ?? consulta.Diagnostico;
                        consulta.Comentarios = consultaDto.Comentarios ?? consulta.Comentarios;
                        consulta.VeterinarioId = consultaDto.VeterinarioId != 0 ? consultaDto.VeterinarioId : consulta.VeterinarioId;
                        consulta.CachorroId = consultaDto.CachorroId != 0 ? consultaDto.CachorroId : consulta.CachorroId;
                        consulta.ClienteID = consultaDto.ClienteID != 0 ? consultaDto.ClienteID : consulta.ClienteID;
                        consulta.IsAtivo = consultaDto.IsAtivo == true ? consultaDto.IsAtivo : consulta.IsAtivo;

                        _context.SaveChanges();
                        return Ok(new { msg = "Consulta alterada:", consulta });
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new { msg = "Consulta não encontrado" });
                    }
                }
                catch
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Consulta não encontrado" });
                }
            }
            else
            {

                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O Id informado é inválido" });
            }
        }

        /// <summary>
        /// Desativa uma consulta pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna a consulta inativa</returns>
        [HttpDelete("Deletar/{id}")]
        [Authorize(Roles = "Veterinario")]
        public IActionResult Delete(int id)
        {
            try
            {
                Consulta consulta = _context.Consultas.FirstOrDefault(consulta => consulta.Id == id);
                if (consulta is null) return BadRequest($"Consulta com o Id: {id} não encontrado!");
                if (consulta.IsAtivo == false) return BadRequest($"Consulta com o Id: {id} já se encontra excluído no sistema!");
                consulta.IsAtivo = false;
                _context.Consultas.Update(consulta);
                _context.SaveChanges();
                return Ok($"Consulta com o Id: {id} Excluído com sucesso!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");

            }
        }

    }
}