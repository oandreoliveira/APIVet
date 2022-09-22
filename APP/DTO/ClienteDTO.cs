using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiVet.DTO
{
    public class ClienteDTO
    {

        [Required(ErrorMessage = "Nome do cliente é obrigatório!")]
        [StringLength(80)]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Cpf do Cliente é obrigatório!")]
        [Range(9999999999, 100000000000, ErrorMessage = "Cpf inválido, informe os 11 dígitos")]
        public string CPF { get; set; }
        [Required(ErrorMessage = "Endereço do cliente é obrigatório!")]
        [StringLength(200)]
        public string Endereco { get; set; }
        [Required(ErrorMessage = "Telefone de contato é obrigatório!")]
        [MinLength(9, ErrorMessage = "Informe apenas os 9 dígitos")]
        [StringLength(9, ErrorMessage = "Informe apenas os 9 dígitos")]
        public string Telefone { get; set; }
        [JsonIgnore]
        public bool IsAtivo { get; set; } = true;

    }
}