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
    public class ClientesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// 
        /// <remarks>
        /// Exemplo de request:
        ///
        ///     POST /api/v1/Clientes/Criar
        ///     {
        ///          "nome": "Carlos",
        ///          "cpf": "00000000000",
        ///          "endereco": "Rua da Aurora, 555",
        ///         "telefone": "999999999"
        ///     }
        /// </remarks>
        /// <param name="clienteTemp"></param>
        /// <returns>Retorna a inclusão de um novo cliente</returns>
        [HttpPost("Criar")]
        public ActionResult Post(ClienteDTO clienteTemp)
        {
            try
            {
                if (clienteTemp is null) return BadRequest();
                var pesquisaCli = _context.Clientes.FirstOrDefault(cli => cli.CPF == clienteTemp.CPF);
                if (pesquisaCli != null) return BadRequest(new { msg = "Este CPF já está cadastrado em nosso Sistema" });

                Cliente cliente = _mapper.Map<Cliente>(clienteTemp);

                _context.Clientes.Add(cliente);
                _context.SaveChanges();
                return new CreatedAtRouteResult("ObterCliente", new { id = cliente.Id }, cliente);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Exibe todos os clientes cadastrados
        /// </summary>
        /// <returns>Retorna todos os clientes Ativos</returns>
        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> Get()
        {
            try
            {
                var clientes = await _context.Clientes.Where(cli => cli.IsAtivo == true).AsNoTracking().ToListAsync();
                return clientes.Any() ? Ok(new { msg = "Clientes:", clientes }) : BadRequest("Clientes não encontrados!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Pesquisa um cliente pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna o cliente</returns>
        [HttpGet("Cliente/{id:int}", Name = "ObterCliente")]
        public async Task<ActionResult<ClienteDTO>> GetById(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(cli => cli.Id == id);
                return cliente is null ? BadRequest($"Cliente com o Id: {id} não encontrado!") : Ok(new { msg = "Cliente solicitado:", cliente });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Pesquisa um cliente pelo seu CPF
        /// </summary>
        /// <param name="Cpf"></param>
        /// <returns>Retorna o cliente</returns>
        [HttpGet("Cliente/{Cpf}")]
        public async Task<ActionResult<ClienteDTO>> GetByCpf(string Cpf)
        {
            try
            {
                var cliente = await _context.Clientes.Where(cons => cons.CPF == Cpf).ToListAsync();
                return cliente.Any() ? Ok(new { msg = "Cliente solicitado:", cliente }) : BadRequest($"Cpf do Cliente: {Cpf} não encontrado!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }


        /// <summary>
        /// Atualiza informações do cliente pelo seu Id
        /// </summary>
        /// <remarks>
        /// O que não será alterado, basta apagar!
        /// 
        /// Exemplo de request:
        /// 
        ///         PATCH /api/v1/Clientes/atualizar
        ///         {
        ///            "nome": "Victor",
        ///            "cpf": "00000000000",
        ///            "endereco": "Rua da Aurora, 555",
        ///            "telefone": "999999999",
        ///            "isAtivo": true
        ///         }
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="clienteDto"></param>
        /// <returns>Retorna o cliente pesquisado atualizado </returns>
        [HttpPatch("Atualizar/{id:int}")]
        public IActionResult Patch(int id, ClienteUpdateDTO clienteDto)
        {
            if (id > 0)
            {
                try
                {
                    var cliente = _context.Clientes.FirstOrDefault(cli => cli.Id == id);
                    if (cliente != null)
                    {

                        cliente.Nome = clienteDto.Nome ?? cliente.Nome;
                        cliente.CPF = clienteDto.CPF ?? cliente.CPF;
                        cliente.Endereco = clienteDto.Endereco ?? cliente.Endereco;
                        cliente.Telefone = clienteDto.Telefone ?? cliente.Telefone;
                        cliente.IsAtivo = clienteDto.IsAtivo == true ? clienteDto.IsAtivo : cliente.IsAtivo;

                        var pesquisaCli = _context.Clientes.FirstOrDefault(cli => cli.CPF == clienteDto.CPF && cli.Id != cliente.Id);
                        if (pesquisaCli != null) return BadRequest(new { msg = "Este CPF já está cadastrado em nosso Sistema" });

                        _context.SaveChanges();
                        return Ok(new { msg = "Cliente alterado:", cliente });
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new { msg = "Cliente não encontrado" });
                    }
                }
                catch
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Cliente não encontrado" });
                }
            }
            else
            {

                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O Id informado é inválido" });
            }
        }

        /// <summary>
        /// Desativa um cliente pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna o cliente inativo</returns>
        [HttpDelete("Deletar/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Cliente cliente = _context.Clientes.FirstOrDefault(cli => cli.Id == id);
                if (cliente is null) return BadRequest($"Cliente com o Id: {id} não encontrado!");
                if (cliente.IsAtivo == false) return BadRequest($"Cliente com o Id: {id} já se encontra excluído no sistema!");
                cliente.IsAtivo = false;
                _context.Clientes.Update(cliente);
                _context.SaveChanges();
                return Ok($"Cliente com o Id: {id} Excluído com sucesso!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");

            }
        }
    }
}