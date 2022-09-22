using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiVet.DTO
{
    public class ConsultaDTO
    {


        [JsonIgnore]
        public DateTime? Data { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Peso atual do animal é obrigatório!")]
        public double PesoAtual { get; set; }
        [Required(ErrorMessage = "Informar os sintomas é obrigatório!")]
        [StringLength(400)]
        public string Sintomas { get; set; }
        [Required(ErrorMessage = "Diagnóstico é obrigatório!")]
        [StringLength(400)]
        public string Diagnostico { get; set; }
        [Required(ErrorMessage = "Comentérios são obrigatórios!")]
        [StringLength(400)]
        public string Comentarios { get; set; }
        [Required]
        public int VeterinarioId { get; set; }
        [Required]
        public int CachorroId { get; set; }
        public int ClienteID { get; set; }
        [JsonIgnore]
        public bool IsAtivo { get; set; } = true;
    }
}