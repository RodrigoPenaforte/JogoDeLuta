using JogoDeLuta.Enum;

namespace JogoDeLuta.Models
{
    public class Efeito
    {
        public string Nome { get; set; } = "";
        public string Descricao { get; set; } = "";
        public TipoEfeito Tipo { get; set; }
        public MomentoEfeito Momento { get; set; }
        public int DuracaoTurnos { get; set; }
        public bool PodeAcumular { get; set; }
        public int MaxStacks { get; set; } = 1;
        public int DanoPorTurno { get; set; }
        public double ModAtaqueFisicoPct { get; set; }
        public double ModAtaqueMagicoPct { get; set; }
        public double ModDanoFisicoPct { get; set; }
        public double ModDanoMagicoPct { get; set; }
        public int ModDefesaFisica { get; set; }
        public int ModDefesaMagica { get; set; }
    }
}

