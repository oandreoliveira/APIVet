using System;

namespace ApiVet.Models
{
    public class Consulta
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public double PesoAtual { get; set; }
        public string Sintomas { get; set; }
        public string Diagnostico { get; set; }
        public string Comentarios { get; set; }
        public int VeterinarioId { get; set; }
        public Veterinario Veterinario { get; set; }
        public int CachorroId { get; set; }
        public Cachorro Cachorro { get; set; }
        public int ClienteID { get; set; }
        public Cliente Cliente { get; set; }
        public bool IsAtivo { get; set; }



    }
}