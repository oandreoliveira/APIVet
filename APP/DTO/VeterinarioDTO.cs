using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiVet.DTO
{
    public class VeterinarioDTO
    {
        [Required(ErrorMessage = "Nome do Veterinário é obrigatório!")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Código do Conselho Regional de Med. Veterinária é obrigatório!")]
        [Range(100000, 999999, ErrorMessage = "Código do Conselho Regional de Med. Veterinária Contém 6 dígitos")]
        public int CodCrmv { get; set; }
        [JsonIgnore]
        public bool IsAtivo { get; set; } = true;
    }
}