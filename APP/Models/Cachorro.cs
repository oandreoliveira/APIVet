using System;
using System.Text.Json.Serialization;

namespace ApiVet.Models
{
    public class Cachorro
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Raca { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Genero { get; set; }
        public double Peso { get; set; }
        public int ClienteId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Cliente Cliente { get; set; }
        public bool IsAtivo { get; set; }
    }
}