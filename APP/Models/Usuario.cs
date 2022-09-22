namespace ApiVet.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool IsCliente { get; set; }
        public bool IsAtivo { get; set; } = true;
    }
}