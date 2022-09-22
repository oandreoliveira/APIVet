using System;
using System.ComponentModel.DataAnnotations;

namespace ApiVet.DTO
{
    public class VeterinarioUpdateDTO
    {

        public string Nome { get; set; }
        [Range(100000, 999999, ErrorMessage = "Código do Conselho Regional de Med. Veterinária Contém 6 dígitos")]
        public int CodCrmv { get; set; }
        public bool IsAtivo { get; set; }
    }
}