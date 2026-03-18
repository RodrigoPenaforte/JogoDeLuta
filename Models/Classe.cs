using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JogoDeLuta.Models
{
    public class Classe
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public int HPBase { get; set; }
        public int EnergiaBase { get; set; }
        public int StaminaBase { get; set; }
        public int HPPorNivel { get; set; }
        public int EnergiaPorNivel { get; set; }
        public int StaminaPorNivel { get; set; }
        public int AtaqueFisicoPorNivel { get; set; }
        public int AtaqueMagicoPorNivel { get; set; }
        public int DefesaFisicaPorNivel { get; set; }
        public int DefesaMagicaPorNivel { get; set; }
        public int DefesaFisica { get; set; }
        public int DefesaMagica { get; set; }
        public double ReducaoDanoBase { get; set; }
        public int AtaqueFisico { get; set; }
        public int AtaqueMagico { get; set; }
        public int RecuperacaoEnergia { get; set; }
        public int RecuperacaoStamina { get; set; }
        public List<Habilidade> Habilidades { get; set; } = new List<Habilidade>();


    }
}