namespace JogoDeLuta.Models
{
    public class EfeitoAtivo
    {
        public Efeito Efeito { get; set; } = new();
        public int TurnosRestantes { get; set; }
        public int Stacks { get; set; } = 1;
        public string OrigemNome { get; set; } = "";
    }
}

