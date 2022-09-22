using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ApiVet.Models;

namespace ApiVet.DTO
{
    public class ClienteUpdateDTO
    {
        [StringLength(80)]
        public string Nome { get; set; }
        [Range(9999999999, 100000000000, ErrorMessage = "Cpf inválido, informe os 11 dígitos")]
        public string CPF { get; set; }
        [StringLength(200)]
        public string Endereco { get; set; }
        [MinLength(9, ErrorMessage = "Informe apenas os 9 dígitos")]
        [StringLength(9, ErrorMessage = "Informe apenas os 9 dígitos")]
        public string Telefone { get; set; }
        public bool IsAtivo { get; set; }
       
    }
}