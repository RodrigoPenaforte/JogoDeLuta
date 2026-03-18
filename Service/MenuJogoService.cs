using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JogoDeLuta.Utils;

namespace JogoDeLuta.Service
{
    public class MenuJogoService
    {
        readonly ClasseService classeService = new();
        readonly EscolherPersonagemService escolherPersonagemService;
        readonly CombateService combateService;

        public MenuJogoService()
        {
            escolherPersonagemService = new EscolherPersonagemService(classeService);
            combateService = new CombateService(classeService);
        }

    
        public async Task MenuJogo()
        {
            while (true)
            {
                await Carragamento.Carregando();
                Console.WriteLine("\n Bem-vindo ao Senhor dos Anéis : O Jogo");
                await Carragamento.CarregandoApresentacao();
                Console.WriteLine();
                Console.WriteLine("\n1 - Escolher personagem\n");
                Console.WriteLine("2 - Sair do jogo");

                string? input = Console.ReadLine();

                if (!int.TryParse(input, out int op))
                {
                    Console.WriteLine("Entrada inválida...");
                    continue;
                }

                switch (op)
                {
                    case 1:
                        var personagem = escolherPersonagemService.Escolher();
                        Console.WriteLine($"Você escolheu: {personagem.Nome}");
                        await combateService.IniciarCombate(personagem);

                        break;

                    case 2:
                        Console.WriteLine("Saindo...");
                        return;

                    default:
                        Console.WriteLine("Opção inválida...");
                        break;
                }
            }
        }
    }
}