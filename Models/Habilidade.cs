using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JogoDeLuta.Enum;

namespace JogoDeLuta.Models
{
    public class Habilidade
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public int Dano { get; set; }
        public int CustoEnergia { get; set; }
        public int CustoStamina { get; set; }
        public int NivelHabilidade { get; set; }
        public TipoDano Tipo { get; set; }
        public List<Efeito> Efeitos { get; set; } = new();
    }
}