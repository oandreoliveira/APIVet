using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiVet.DTO
{
    public class CachorroDTO
    {

        [Required(ErrorMessage = "Nome do animal é obrigatório!")]
        [StringLength(50, ErrorMessage = "Max 50 Caracteres")]
        [MinLength(2, ErrorMessage = "Nome muito curto!")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Raça do animal é obrigatória!")]
        public string Raca { get; set; }
        [Required(ErrorMessage = "Data de nascimento é obrigatória!")]
        [DisplayFormat(DataFormatString = "mm/dd/yyyy")]
        public DateTime? DataNascimento { get; set; }
        [Required(ErrorMessage = "Gênero do animal é obrigatório!")]
        public string Genero { get; set; }
        [Required(ErrorMessage = "Peso do animal é obrigatório!")]
        public double Peso { get; set; }
        public bool IsAtivo { get; set; } = true;
        [Required]
        public int ClienteID { get; set; }


    }
}