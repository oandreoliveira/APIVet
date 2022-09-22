using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiVet.DTO
{
    public class UsuarioDTO
    {

        [Required(ErrorMessage = "Email do Cliente é obrigatório!")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Senha  é obrigatória!")]
        [DataType(DataType.Password)]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "A senha precisa ter entre 6 e 10 caracteres")]
        public string Senha { get; set; }
        public bool IsCliente { get; set; }



    }
}