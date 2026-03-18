using JogoDeLuta.Models;

namespace JogoDeLuta.Service
{
    public class EscolherPersonagemService
    {
        private List<Personagem> personagens = [];
        private readonly ClasseService classeService;

        public EscolherPersonagemService(ClasseService classeService)
        {
            this.classeService = classeService;
            CriarPersonagem();
        }
        private void CriarPersonagem()
        {
            var lutador = classeService.ObterPorId(1);
            var mago = classeService.ObterPorId(2);

            personagens.Add(new Personagem
            {
                Id = 1,
                Nivel = 5,
                Nome = "Aragorn",
                Classe = lutador
            });

            personagens.Add(new Personagem
            {
                Id = 2,
                Nivel = 5,
                Nome = "Boromir",
                Classe = lutador
            });
        }
        public Personagem Escolher()
        {
            Console.WriteLine("Personagens:");

            foreach (var personagem in personagens)
            {
                Console.WriteLine($"{personagem.Id} | Nome: {personagem.Nome} | Classe: {personagem.Classe!.Nome}");
            }

            while (true)
            {
                string? input = Console.ReadLine();

                if (!int.TryParse(input, out int escolha))
                {
                    Console.WriteLine("Entrada inválida...");
                    continue;
                }

                var personagem = personagens.FirstOrDefault(p => p.Id == escolha);

                if (personagem == null)
                {
                    Console.WriteLine("Personagem não encontrado...");
                    continue;
                }

                var copia = new Personagem
                {
                    Id = personagem.Id,
                    Nivel = personagem.Nivel,
                    Nome = personagem.Nome,
                    Classe = personagem.Classe
                };

                copia.AplicarNivel();
                return copia;
            }
        }

    }
}