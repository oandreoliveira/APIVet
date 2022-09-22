using System.ComponentModel.DataAnnotations;

namespace ApiVet.DTO
{
    public class ConsultaUpdateDTO
    {

        public double PesoAtual { get; set; }
        [StringLength(400)]
        public string Sintomas { get; set; }
        [StringLength(400)]
        public string Diagnostico { get; set; }
        [StringLength(400)]
        public string Comentarios { get; set; }
        public int ClienteID { get; set; }
        public int VeterinarioId { get; set; }

        public int CachorroId { get; set; }

        public bool IsAtivo { get; set; } = true;
    }
}