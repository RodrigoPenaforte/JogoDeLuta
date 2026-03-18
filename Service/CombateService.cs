using JogoDeLuta.Models;
using JogoDeLuta.Enum;
using JogoDeLuta.Utils;

namespace JogoDeLuta.Service
{
    public class CombateService
    {
        private readonly ClasseService classeService;

        public CombateService(ClasseService classeService)
        {
            this.classeService = classeService;
        }

        public async Task IniciarCombate(Personagem jogador)
        {
            if (jogador.Nivel > Personagem.NivelMaximo)
            {
                Console.WriteLine($"Nível máximo permitido é {Personagem.NivelMaximo}. Ajuste o nível do jogador e tente de novo.");
                return;
            }

            var inimigo = new Personagem
            {
                Id = 3,
                Nivel = 5,
                Nome = "Gandalf",
                Classe = classeService.ObterPorId(2)

            };

            if (inimigo.Nivel > Personagem.NivelMaximo)
            {
                Console.WriteLine($"Nível máximo permitido é {Personagem.NivelMaximo}. Ajuste o nível do inimigo e tente de novo.");
                return;
            }
            inimigo.AplicarNivel();

            var combate = new Combate
            {
                Jogador = jogador,
                Inimigo = inimigo
            };

            Console.WriteLine($"\n {jogador.Nome} ({jogador.Classe!.Nome}) VS {inimigo.Nome} ({inimigo.Classe!.Nome})\n");

            while (jogador.HP > 0 && inimigo.HP > 0)
            {
                jogador.ProcessarInicioDoTurno();
                if (jogador.HP <= 0) break;

                TurnoJogador(combate);
                jogador.ProcessarFimDoTurno();
                if (jogador.HP <= 0 && inimigo.HP <= 0) break;
                if (jogador.HP <= 0) break;

                if (inimigo.HP <= 0)
                    break;

                inimigo.ProcessarInicioDoTurno();
                if (jogador.HP <= 0 && inimigo.HP <= 0) break;
                if (inimigo.HP <= 0) break;

                await TurnoInimigo(combate);
                inimigo.ProcessarFimDoTurno();
                if (jogador.HP <= 0 && inimigo.HP <= 0) break;
                if (inimigo.HP <= 0) break;

                MostrarStatus(jogador, inimigo);
            }

            if (jogador.HP <= 0 && inimigo.HP <= 0)
            {
                Console.WriteLine("\n Empate! Os dois caíram ao mesmo tempo.");
            }
            else
            {
                Console.WriteLine(jogador.HP > 0 ? "\n Você venceu!" : "\n Você perdeu...");
            }
        }

        private void TurnoJogador(Combate combate)
        {
            var jogador = combate.Jogador!;
            var inimigo = combate.Inimigo!;

            while (true)
            {
                Console.WriteLine("\nSua vez:");
                Console.WriteLine("1 - Atacar (escolher habilidade)");
                Console.WriteLine("2 - Descansar");
                Console.WriteLine("3 - Ver meu status");

                string? input = Console.ReadLine();

                if (!int.TryParse(input, out int op))
                {
                    Console.WriteLine("Opção inválida...");
                    continue;
                }

                switch (op)
                {
                    case 1:
                        if (EscolherEUsarHabilidade(combate, jogador, inimigo, () => Descansar(jogador)))
                            return;
                        break;

                    case 2:
                        Descansar(jogador);
                        return;

                    case 3:
                        jogador.MostrarStatusDetalhado();
                        break;

                    default:
                        Console.WriteLine("Opção inválida...");
                        break;
                }
            }
        }

        private async Task TurnoInimigo(Combate combate)
        {
            var inimigo = combate.Inimigo!;
            var jogador = combate.Jogador!;

            Console.WriteLine("\nTurno do inimigo...");

            var habilidades = inimigo.Classe!.Habilidades;

            var habilidadesDisponiveis = habilidades
                .Where(h => h.NivelHabilidade <= inimigo.Nivel)
                .Where(h => inimigo.Energia >= h.CustoEnergia && inimigo.Stamina >= h.CustoStamina)
                .ToList();

            if (habilidadesDisponiveis.Count == 0)
            {
                Descansar(inimigo);
                return;
            }

            var habilidadesSeguras = habilidadesDisponiveis
                .Where(h => (inimigo.Energia - h.CustoEnergia) > 0 && (inimigo.Stamina - h.CustoStamina) > 0)
                .ToList();

            var finalizadoras = habilidadesDisponiveis
                .Where(h =>
                {
                    int n = inimigo.EscalaNivel();

                    int ataqueBase = h.Tipo == TipoDano.Fisico ? inimigo.Classe!.AtaqueFisico : inimigo.Classe!.AtaqueMagico;
                    int ataquePorNivel = h.Tipo == TipoDano.Fisico ? inimigo.Classe!.AtaqueFisicoPorNivel : inimigo.Classe!.AtaqueMagicoPorNivel;
                    int bonusAtaque = ataqueBase + ataquePorNivel * n;

                    int danoBrutoProvavel = h.Dano + bonusAtaque;
                    return danoBrutoProvavel >= jogador.HP;
                })
                .ToList();

            var pool = finalizadoras.Count > 0 ? finalizadoras : (habilidadesSeguras.Count > 0 ? habilidadesSeguras : habilidadesDisponiveis);

            var habilidadeEscolhida = pool[rand.Next(pool.Count)];

            Console.WriteLine($"{inimigo.Nome} está se preparando para usar {habilidadeEscolhida.Nome}...");
            await Carragamento.CarregandoTurnoInimigo();

            Combate.UsarHabilidade(inimigo, jogador, habilidadeEscolhida);
        }

        private static Random rand = new Random();

        private static bool EscolherEUsarHabilidade(Combate combate, Personagem atacante, Personagem alvo, Action descansar)
        {
            var habilidades = atacante.Classe!.Habilidades;

            if (habilidades.Count == 0)
            {
                Console.WriteLine("Sua classe não possui habilidades cadastradas.");
                return false;
            }

            while (true)
            {
                Console.WriteLine("\nEscolha uma habilidade (0 - Voltar):");

                for (int i = 0; i < habilidades.Count; i++)
                {
                    var h = habilidades[i];
                    var nivelTxt = h.NivelHabilidade > 0 ? $" | Nível: {h.NivelHabilidade}" : "";
                    Console.WriteLine($"{i + 1} - {h.Nome} | Dano: {h.Dano} | Energia: {h.CustoEnergia} | Stamina: {h.CustoStamina}{nivelTxt}");
                }

                string? input = Console.ReadLine();

                if (!int.TryParse(input, out int indice))
                {
                    Console.WriteLine("Entrada inválida...");
                    continue;
                }

                if (indice == 0)
                    return false;

                if (indice < 1 || indice > habilidades.Count)
                {
                    Console.WriteLine("Habilidade inválida...");
                    continue;
                }

                var habilidadeEscolhida = habilidades[indice - 1];

                if (habilidadeEscolhida.NivelHabilidade > atacante.Nivel)
                {
                    Console.WriteLine($"Você precisa estar no nível {habilidadeEscolhida.NivelHabilidade} para usar {habilidadeEscolhida.Nome}.");
                    continue;
                }

                if (atacante.Energia < habilidadeEscolhida.CustoEnergia || atacante.Stamina < habilidadeEscolhida.CustoStamina)
                {
                    Console.WriteLine("Você não tem energia ou stamina suficiente para usar essa habilidade.");
                    continue;
                }

                int energiaDepois = atacante.Energia - habilidadeEscolhida.CustoEnergia;
                int staminaDepois = atacante.Stamina - habilidadeEscolhida.CustoStamina;

                if (energiaDepois == 0 || staminaDepois == 0)
                {
                    Console.WriteLine("Atenção: se você usar essa habilidade, você vai ficar no limite (0) de energia e/ou stamina.");
                    Console.WriteLine($"Ficaria assim -> Energia: {energiaDepois} | Stamina: {staminaDepois}");
                    Console.WriteLine("Regra do jogo: se você ficar com 0 e o inimigo não cair, você perde.");
                    Console.WriteLine("1 - Confirmar mesmo assim");
                    Console.WriteLine("2 - Escolher outra habilidade");
                    Console.WriteLine("3 - Voltar e descansar");

                    string? confirmacao = Console.ReadLine();
                    if (!int.TryParse(confirmacao, out int opConf))
                        continue;

                    if (opConf == 3)
                    {
                        descansar();
                        return true;
                    }

                    if (opConf != 1)
                        continue;
                }

                Combate.UsarHabilidade(atacante, alvo, habilidadeEscolhida);
                Console.WriteLine($"Recursos após a ação -> Energia: {atacante.Energia} | Stamina: {atacante.Stamina}");

                if ((atacante.Energia == 0 || atacante.Stamina == 0) && alvo.HP > 0)
                {
                    Console.WriteLine($"{atacante.Nome} ficou sem recursos e foi derrotado!");
                    atacante.HP = 0;
                }
                return true;
            }
        }

        private void Descansar(Personagem personagem)
        {
            Console.WriteLine($"{personagem.Nome} decidiu descansar...");

            int recuperarEnergia = personagem.Classe!.RecuperacaoEnergia * 2;
            int recuperarStamina = personagem.Classe!.RecuperacaoStamina * 2;

            personagem.Energia = Math.Min(personagem.EnergiaMax, personagem.Energia + recuperarEnergia);
            personagem.Stamina = Math.Min(personagem.StaminaMax, personagem.Stamina + recuperarStamina);
        }

        private void MostrarStatus(Personagem jogador, Personagem inimigo)
        {
            Console.WriteLine("\n===== STATUS =====");
            Console.WriteLine($"{jogador.Nome} HP: {jogador.HP} | Energia: {jogador.Energia} | Stamina: {jogador.Stamina}");
            Console.WriteLine($"{inimigo.Nome} HP: {inimigo.HP} | Energia: {inimigo.Energia} | Stamina: {inimigo.Stamina}");

            if (inimigo.EfeitosAtivos.Count > 0)
            {
                var efeitosTxt = string.Join(", ",
                    inimigo.EfeitosAtivos.Select(e => $"{e.Efeito.Nome}({e.TurnosRestantes})"));
                Console.WriteLine($"{inimigo.Nome} efeitos: {efeitosTxt}");
            }
            Console.WriteLine("==================\n");
        }
    }
}