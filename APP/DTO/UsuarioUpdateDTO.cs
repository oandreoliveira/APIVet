using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiVet.DTO
{
    public class UsuarioUpdateDTO
    {


        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "A senha precisa ter entre 6 e 10 caracteres")]
        public string Senha { get; set; }
        public bool IsCliente { get; set; }
        public bool IsAtivo { get; set; }
    }
}