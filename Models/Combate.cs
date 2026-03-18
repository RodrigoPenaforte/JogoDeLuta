using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JogoDeLuta.Enum;

namespace JogoDeLuta.Models
{
    public class Combate
    {
        public int Id { get; set; }
        public Personagem? Jogador { get; set; }
        public Personagem? Inimigo { get; set; }

        private static readonly Random rand = new();

        private static string TextoMomento(MomentoEfeito momento) =>
            momento == MomentoEfeito.InicioDoTurno ? "no início do turno" :
            (momento == MomentoEfeito.FimDoTurno ? "no fim do turno" : "ao acertar");

        private static string DescreverEfeito(Efeito efeito)
        {
            var partes = new List<string>();

            if (efeito.DanoPorTurno > 0 && (efeito.Momento == MomentoEfeito.InicioDoTurno || efeito.Momento == MomentoEfeito.FimDoTurno))
                partes.Add($"{efeito.DanoPorTurno} de dano {TextoMomento(efeito.Momento)}");

            if (efeito.ModAtaqueFisicoPct != 0)
                partes.Add($"{Pct(efeito.ModAtaqueFisicoPct)} ataque físico");

            if (efeito.ModAtaqueMagicoPct != 0)
                partes.Add($"{Pct(efeito.ModAtaqueMagicoPct)} ataque mágico");

            if (efeito.ModDanoFisicoPct != 0)
                partes.Add($"{Pct(efeito.ModDanoFisicoPct)} dano físico");

            if (efeito.ModDanoMagicoPct != 0)
                partes.Add($"{Pct(efeito.ModDanoMagicoPct)} dano mágico");

            if (efeito.ModDefesaFisica != 0)
                partes.Add($"{(efeito.ModDefesaFisica > 0 ? "+" : "")}{efeito.ModDefesaFisica} defesa física");

            if (efeito.ModDefesaMagica != 0)
                partes.Add($"{(efeito.ModDefesaMagica > 0 ? "+" : "")}{efeito.ModDefesaMagica} defesa mágica");

            if (partes.Count == 0)
                return "sem efeito numérico configurado";

            return string.Join(" | ", partes);
        }

        private static string Pct(double valor)
        {
            int pct = (int)Math.Round(valor * 100);
            return pct > 0 ? $"+{pct}%" : $"{pct}%";
        }

        public static void UsarHabilidade(Personagem atacante, Personagem alvo, Habilidade habilidade)
        {
            if (atacante.Energia < habilidade.CustoEnergia ||
                atacante.Stamina < habilidade.CustoStamina)
            {
                Console.WriteLine("Sem energia ou stamina suficiente!");
                return;
            }

            atacante.Energia -= habilidade.CustoEnergia;
            atacante.Stamina -= habilidade.CustoStamina;

            int n = atacante.EscalaNivel();

            int ataqueBase = habilidade.Tipo == TipoDano.Fisico
                ? atacante.Classe!.AtaqueFisico
                : atacante.Classe!.AtaqueMagico;

            int ataquePorNivel = habilidade.Tipo == TipoDano.Fisico
                ? atacante.Classe!.AtaqueFisicoPorNivel
                : atacante.Classe!.AtaqueMagicoPorNivel;

            int bonusAtaque = ataqueBase + ataquePorNivel * n;
            bonusAtaque = (int)Math.Round(bonusAtaque * (1 + atacante.SomarModAtaquePct(habilidade.Tipo)));

            int variacao = rand.Next(-2, 3);
            int danoBrutoBase = habilidade.Dano + bonusAtaque;
            danoBrutoBase = (int)Math.Round(danoBrutoBase * (1 + atacante.SomarModDanoPct(habilidade.Tipo)));
            int danoBruto = Math.Max(1, danoBrutoBase + variacao);

            int danoAplicado = alvo.ReceberDano(danoBruto, habilidade.Tipo);
            int danoAbsorvido = danoBruto - danoAplicado;

            string tipoTexto = habilidade.Tipo == TipoDano.Fisico ? "Físico" : "Mágico";

            Console.WriteLine($"\n[{atacante.Nome}] {habilidade.Nome} ({tipoTexto}) -> [{alvo.Nome}]");
            Console.WriteLine($"Dano: {danoAplicado}  (bruto {danoBruto}, bloqueado {danoAbsorvido}) | HP alvo: {alvo.HP}");

            if (danoAplicado <= 0)
                Console.WriteLine("Bloqueio total!");

            foreach (var efeito in habilidade.Efeitos)
            {
                bool aplicado = alvo.AplicarEfeito(efeito, atacante);
                var acumulativoTxt = efeito.PodeAcumular ? "acumulativo" : "não acumulativo";
                string momentoTxt = TextoMomento(efeito.Momento);

                if (aplicado)
                {
                    Console.WriteLine($"Efeito aplicado: {efeito.Nome} ({efeito.Tipo}) | {efeito.DuracaoTurnos} turno(s) | {momentoTxt} | {acumulativoTxt}");
                    if (!string.IsNullOrWhiteSpace(efeito.Descricao))
                        Console.WriteLine($"  - {efeito.Descricao}");
                    Console.WriteLine($"  - {DescreverEfeito(efeito)}");
                }
                else
                {
                    Console.WriteLine($"Efeito ignorado: {efeito.Nome} (já está ativo, {acumulativoTxt}).");
                }
            }
        }
    }
}