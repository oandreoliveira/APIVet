using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVet.Data;
using ApiVet.DTO;
using ApiVet.Helpers;
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
    public class CachorrosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public CachorrosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        /// <summary>
        /// Inclui um novo cachorro
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///
        ///     POST /api/v1/Cachorros/Criar
        ///     {
        ///         "nome": "Lessie",
        ///         "raca": "Bulldog",
        ///         "dataNascimento": "2022-07-23T00:40:31.780Z",
        ///         "genero": "Fêmea",
        ///         "peso": 10,
        ///         "isAtivo": true,
        ///         "clienteID": 1
        ///     }
        /// </remarks>
        /// <param name="cachorroTemp"></param>
        /// <returns>Retorna a inclusão de um novo cachorro</returns>
        [HttpPost("Criar")]
        public ActionResult Post(CachorroDTO cachorroTemp)
        {
            try
            {
                if (cachorroTemp is null) return BadRequest();

                Cachorro cachorro = _mapper.Map<Cachorro>(cachorroTemp);

                _context.Cachorros.Add(cachorro);
                _context.SaveChanges();
                //return StatusCode(201, cachorro);
                return new CreatedAtRouteResult("ObterCachorro", new { id = cachorro.Id }, cachorro);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Exibe todos os cachorros cadastrados
        /// </summary>
        /// <returns>Retorna todos os cachorros Ativos</returns>
        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<CachorroDTO>>> Get()
        {
            try
            {
                var cachorros = await _context.Cachorros.Where(dog => dog.IsAtivo == true).Include(cli => cli.Cliente).AsNoTracking().ToListAsync();
                return cachorros.Any() ? Ok(new { msg = "Cachorros:", cachorros }) : BadRequest("Cachorros não encontrados!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Pesquisa um cachorro pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna o cachorro</returns>
        [HttpGet("Cachorro/{id:int}", Name = "ObterCachorro")]
        public async Task<ActionResult<CachorroDTO>> GetById(int id)
        {
            try
            {
                var cachorro = await _context.Cachorros.FirstOrDefaultAsync(dog => dog.Id == id);
                return cachorro is null ? BadRequest($"Cachorro com o Id: {id} não encontrado!") : Ok(new { msg = "Cachorro solicitado:", cachorro });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Exibe todos os cachorros do mesmo Cliente pelo CPF 
        /// </summary>
        /// <param name="Cpf"></param>
        /// <returns>Retorna todos os cachorros vinculados ao Cliente</returns>
        [HttpGet("Listar/{Cpf}")]
        public async Task<ActionResult<ClienteDTO>> GetCachorrosByCpf(string Cpf)
        {
            try
            {
                var cliente = await _context.Cachorros.Include(cli => cli.Cliente).Where(cons => cons.Cliente.CPF == Cpf).AsNoTracking().ToListAsync();
                return cliente.Any() ? Ok(new { msg = "Cliente solicitado:", cliente }) : BadRequest($"Cpf do Cliente: {Cpf} não encontrado!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }
        }
        /// <summary>
        /// Atualiza informações do cachorro pelo seu Id
        /// </summary>
        /// <remarks>
        /// O que não será alterado, basta apagar!
        /// 
        /// Exemplo de request:
        ///
        ///     PATCH /api/v1/Cachorros/Atualizar
        ///     {
        ///      "nome": "Laika",
        ///      "raca": "Bulldog",
        ///      "dataNascimento": "2022-07-23T00:40:31.780Z",
        ///      "genero": "Fêmea",
        ///      "peso": 10,
        ///      "isAtivo": true,
        ///      "clienteID": 1
        ///     }
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="cachorroDto"></param>
        /// <returns>Retorna o cachorro pesquisado atualizado</returns>
        [HttpPatch("Atualizar/{id}")]
        public IActionResult Patch(int id, CachorroUpdateDTO cachorroDto)
        {
            if (id > 0)
            {
                try
                {
                    var cachorro = _context.Cachorros.FirstOrDefault(dog => dog.Id == id);
                    if (cachorro != null)
                    {

                        cachorro.Nome = cachorroDto.Nome ?? cachorro.Nome;
                        cachorro.Raca = cachorroDto.Raca ?? cachorro.Raca;
                        cachorro.Genero = cachorroDto.Genero ?? cachorro.Genero;
                        cachorro.DataNascimento = cachorroDto.DataNascimento ?? cachorro.DataNascimento;
                        cachorro.Peso = cachorroDto.Peso != 0 ? cachorroDto.Peso : cachorro.Peso;
                        cachorro.IsAtivo = cachorroDto.IsAtivo == true ? cachorroDto.IsAtivo : cachorro.IsAtivo;
                        cachorro.ClienteId = cachorroDto.ClienteID != 0 ? cachorroDto.ClienteID : cachorro.ClienteId;


                        _context.SaveChanges();
                        return Ok(new { msg = "Cachorro alterado:", cachorro });
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new { msg = "Cachorro não encontrado" });
                    }
                }
                catch
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Cachorro não encontrado" });
                }
            }
            else
            {

                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O Id informado é inválido" });
            }
        }

        /// <summary>
        /// Desativa um cachorro pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna o cachorro inativo</returns>
        [HttpDelete("Deletar/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Cachorro cachorro = _context.Cachorros.FirstOrDefault(dog => dog.Id == id);
                if (cachorro is null) return BadRequest($"Cachorro com o Id: {id} não encontrado!");
                if (cachorro.IsAtivo == false) return BadRequest($"Cachorro com o Id: {id} já se encontra excluído no sistema!");
                cachorro.IsAtivo = false;
                _context.Cachorros.Update(cachorro);
                _context.SaveChanges();
                return Ok($"Cachorro com o Id: {id} Excluído com sucesso!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");

            }

        }
        /// <summary>
        /// Exibe as 20 primeiras Raças da TheDogApi
        /// </summary>
        /// <returns>Retorna as  20 primeiras Raças da TheDogApio</returns>
        [HttpGet("TheDogAPI/ListarRacas")]
        public async Task<ActionResult<CachorroProfile>> GetRacas()
        {

            try
            {
                GetApi racas = new();
                List<CachorroProfile> listaRaca = new();
                listaRaca = await racas.GetBreeds();
                return Ok(listaRaca);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }


        }
        /// <summary>
        /// Exibe todas as Raças que contém o nome pesquisado da TheDogApi
        /// </summary>
        /// <param name="Nome"></param>
        /// <returns>Retorna a lista das Raças que contém o nome pesquisado</returns>

        [HttpGet("TheDogAPI/ListarRacas/{Nome}")]
        public async Task<ActionResult<CachorroProfile2>> GetRacasByName(string Nome)
        {
            try
            {
                GetApi racas = new();
                List<CachorroProfile2> listaRaca = new();
                listaRaca = await racas.GetBreedByName(Nome);
                return Ok(listaRaca);

            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }

        }

        /// <summary>
        /// Exibe as 20 primeiras Imagens Públicas das Raças da TheDogApi
        /// </summary>
        /// <returns>Retorna as Imagens Públicas 20 primeiras Raças da TheDogApio</returns>
        [HttpGet("TheDogAPI/ListarImagensPublicas")]
        public async Task<ActionResult<CachorroProfile>> GetPublicImages()
        {
            try
            {
                GetApi racas = new();
                List<CachorroProfile> listaRaca = new();
                listaRaca = await racas.GetBreeds();
                return Ok(listaRaca);
            }
            catch (System.Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao processar sua solicitação");
            }


        }

    }
}

