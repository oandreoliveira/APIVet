namespace ApiVet.Models
{
    public class Veterinario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int CodCrmv { get; set; }
        public bool IsAtivo { get; set; }
    }
}