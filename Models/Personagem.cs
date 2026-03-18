using JogoDeLuta.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JogoDeLuta.Models
{
    public class Personagem
    {
        public const int NivelMaximo = 20;
        public int Id { get; set; }
        public int Nivel { get; set; }
        public string? Nome { get; set; }
        public int HP { get; set; }
        public int HPMax { get; set; }
        public int Energia { get; set; }
        public int EnergiaMax { get; set; }
        public int Stamina { get; set; }
        public int StaminaMax { get; set; }
        public Classe? Classe { get; set; }
        public List<EfeitoAtivo> EfeitosAtivos { get; set; } = new();

        public int EscalaNivel()
        {
            int n = Math.Max(0, Nivel - 1);
            return (int)Math.Floor(Math.Sqrt(n));
        }

        private static string TextoMomento(MomentoEfeito momento) =>
            momento == MomentoEfeito.InicioDoTurno ? "no início do turno" :
            (momento == MomentoEfeito.FimDoTurno ? "no fim do turno" : "ao acertar");

        private static string Pct(double valor)
        {
            int pct = (int)Math.Round(valor * 100);
            return pct > 0 ? $"+{pct}%" : $"{pct}%";
        }

        private static string DescreverEfeito(Efeito efeito, int stacks)
        {
            var partes = new List<string>();

            if (efeito.DanoPorTurno > 0 && (efeito.Momento == MomentoEfeito.InicioDoTurno || efeito.Momento == MomentoEfeito.FimDoTurno))
                partes.Add($"{efeito.DanoPorTurno * Math.Max(1, stacks)} de dano {TextoMomento(efeito.Momento)}");

            if (efeito.ModAtaqueFisicoPct != 0)
                partes.Add($"{Pct(efeito.ModAtaqueFisicoPct)} ataque físico");

            if (efeito.ModAtaqueMagicoPct != 0)
                partes.Add($"{Pct(efeito.ModAtaqueMagicoPct)} ataque mágico");

            if (efeito.ModDanoFisicoPct != 0)
                partes.Add($"{Pct(efeito.ModDanoFisicoPct)} dano físico");

            if (efeito.ModDanoMagicoPct != 0)
                partes.Add($"{Pct(efeito.ModDanoMagicoPct)} dano mágico");

            if (efeito.ModDefesaFisica != 0)
                partes.Add($"{(efeito.ModDefesaFisica > 0 ? "+" : "")}{efeito.ModDefesaFisica * Math.Max(1, stacks)} defesa física");

            if (efeito.ModDefesaMagica != 0)
                partes.Add($"{(efeito.ModDefesaMagica > 0 ? "+" : "")}{efeito.ModDefesaMagica * Math.Max(1, stacks)} defesa mágica");

            if (partes.Count == 0)
                return "sem efeito numérico configurado";

            return string.Join(" | ", partes);
        }

        public void AplicarNivel()
        {
            if (Classe is null || Nivel <= 0)
                return;

            if (Nivel > NivelMaximo)
                return;

            int n = EscalaNivel();

            HPMax = Classe.HPBase + Classe.HPPorNivel * n;
            EnergiaMax = Classe.EnergiaBase + Classe.EnergiaPorNivel * n;
            StaminaMax = Classe.StaminaBase + Classe.StaminaPorNivel * n;

            if (HP <= 0) HP = HPMax;
            if (Energia <= 0) Energia = EnergiaMax;
            if (Stamina <= 0) Stamina = StaminaMax;
        }

        public void MostrarStatusDetalhado()
        {
            Console.WriteLine("\n===== MEU STATUS =====");
            Console.WriteLine($"{Nome} (Nível {Nivel})");
            Console.WriteLine($"HP: {HP}/{HPMax} | Energia: {Energia}/{EnergiaMax} | Stamina: {Stamina}/{StaminaMax}");

            if (EfeitosAtivos.Count == 0)
            {
                Console.WriteLine("Efeitos: nenhum");
                Console.WriteLine("======================\n");
                return;
            }

            Console.WriteLine("Efeitos ativos:");
            foreach (var ea in EfeitosAtivos)
            {
                var stacksTxt = ea.Stacks > 1 ? $" (stacks: {ea.Stacks})" : "";
                var descTxt = string.IsNullOrWhiteSpace(ea.Efeito.Descricao) ? "" : $" - {ea.Efeito.Descricao}";
                Console.WriteLine($"- {ea.Efeito.Nome} ({ea.Efeito.Tipo}) - {ea.TurnosRestantes} turno(s) restante(s){stacksTxt}{descTxt}");
                Console.WriteLine($"  -> {DescreverEfeito(ea.Efeito, ea.Stacks)}");
            }

            Console.WriteLine("======================\n");
        }

        public int ReceberDano(int danoBruto, TipoDano tipoDano)
        {
            int n = EscalaNivel();
            int defesaBase = tipoDano == TipoDano.Fisico
                ? Classe!.DefesaFisica
                : Classe!.DefesaMagica;

            int defesaPorNivel = tipoDano == TipoDano.Fisico
                ? Classe!.DefesaFisicaPorNivel
                : Classe!.DefesaMagicaPorNivel;

            int defesaTipo = defesaBase + defesaPorNivel * n;

            defesaTipo += SomarModDefesa(tipoDano);

            int danoAposDefesa = Math.Max(1, danoBruto - defesaTipo);

            int danoFinal = (int)(danoAposDefesa * (1 - Classe.ReducaoDanoBase));

            HP -= danoFinal;

            if (HP < 0)
                HP = 0;

            return danoFinal;
        }

        public bool AplicarEfeito(Efeito efeito, Personagem? origem = null)
        {
            var existente = EfeitosAtivos.FirstOrDefault(e => e.Efeito.Nome == efeito.Nome);

            if (existente is not null)
            {
                if (!efeito.PodeAcumular)
                    return false;

                if (existente.Stacks < Math.Max(1, efeito.MaxStacks))
                    existente.Stacks++;

                existente.TurnosRestantes = Math.Max(existente.TurnosRestantes, efeito.DuracaoTurnos);
                return true;
            }

            EfeitosAtivos.Add(new EfeitoAtivo
            {
                Efeito = efeito,
                TurnosRestantes = efeito.DuracaoTurnos,
                Stacks = 1,
                OrigemNome = origem?.Nome ?? ""
            });

            return true;
        }

        public void ProcessarInicioDoTurno()
        {
            if (EfeitosAtivos.Count == 0)
                return;

            int danoTotal = 0;

            foreach (var ea in EfeitosAtivos)
            {
                if (ea.Efeito.Momento == MomentoEfeito.InicioDoTurno && ea.Efeito.DanoPorTurno > 0)
                {
                    danoTotal += ea.Efeito.DanoPorTurno * Math.Max(1, ea.Stacks);
                }
            }

            if (danoTotal > 0)
            {
                HP -= danoTotal;
                if (HP < 0) HP = 0;
                Console.WriteLine($"[{Nome}] sofreu {danoTotal} de dano por efeitos.");
            }
        }

        public void ProcessarFimDoTurno()
        {
            if (EfeitosAtivos.Count == 0)
                return;

            int danoTotal = 0;

            foreach (var ativoEfeito in EfeitosAtivos)
            {
                if (ativoEfeito.Efeito.Momento == MomentoEfeito.FimDoTurno && ativoEfeito.Efeito.DanoPorTurno > 0)
                {
                    danoTotal += ativoEfeito.Efeito.DanoPorTurno * Math.Max(1, ativoEfeito.Stacks);
                }
            }

            if (danoTotal > 0)
            {
                HP -= danoTotal;
                if (HP < 0) HP = 0;
                Console.WriteLine($"[{Nome}] sofreu {danoTotal} de dano por efeitos.");
            }

            for (int i = EfeitosAtivos.Count - 1; i >= 0; i--)
            {
                EfeitosAtivos[i].TurnosRestantes--;
                if (EfeitosAtivos[i].TurnosRestantes <= 0)
                {
                    Console.WriteLine($"[{Nome}] efeito terminou: {EfeitosAtivos[i].Efeito.Nome}.");
                    EfeitosAtivos.RemoveAt(i);
                }
            }
        }

        public int SomarModDefesa(TipoDano tipoDano)
        {
            int soma = 0;
            foreach (var ea in EfeitosAtivos)
            {
                soma += (tipoDano == TipoDano.Fisico ? ea.Efeito.ModDefesaFisica : ea.Efeito.ModDefesaMagica) * Math.Max(1, ea.Stacks);
            }
            return soma;
        }

        public double SomarModDanoPct(TipoDano tipoDano)
        {
            double soma = 0;
            foreach (var ea in EfeitosAtivos)
            {
                soma += (tipoDano == TipoDano.Fisico ? ea.Efeito.ModDanoFisicoPct : ea.Efeito.ModDanoMagicoPct) * Math.Max(1, ea.Stacks);
            }
            return soma;
        }

        public double SomarModAtaquePct(TipoDano tipoDano)
        {
            double soma = 0;
            foreach (var ea in EfeitosAtivos)
            {
                soma += (tipoDano == TipoDano.Fisico ? ea.Efeito.ModAtaqueFisicoPct : ea.Efeito.ModAtaqueMagicoPct) * Math.Max(1, ea.Stacks);
            }
            return soma;
        }

        public void RecuperacaoPassivamente()
        {
            int faltaEnergia = EnergiaMax - Energia;
            int recuperacaoEnergia = Math.Min(faltaEnergia, Classe!.RecuperacaoEnergia);

            int faltaStamina = StaminaMax - Stamina;
            int recuperacaoStamina = Math.Min(faltaStamina, Classe!.RecuperacaoStamina);

            Energia += recuperacaoEnergia;
            Stamina += recuperacaoStamina;
        }

    }
}