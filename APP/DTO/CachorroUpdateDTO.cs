using System;
using System.ComponentModel.DataAnnotations;

namespace ApiVet.DTO
{
    public class CachorroUpdateDTO
    {

        [StringLength(50, ErrorMessage = "Max 50 Caracteres")]
        [MinLength(2, ErrorMessage = "Nome muito curto!")]
        public string Nome { get; set; }
        public string Raca { get; set; }
        [DisplayFormat(DataFormatString = "mm/dd/yyyy")]
        public DateTime? DataNascimento { get; set; }

        public string Genero { get; set; }

        public double Peso { get; set; }

        public int ClienteID { get; set; }

        public bool IsAtivo { get; set; }
    }
}