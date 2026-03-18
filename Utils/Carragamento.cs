using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JogoDeLuta.Utils
{
    public static class Carragamento
    {
        public static async Task Carregando()
        {
            Console.WriteLine("Carregando...");

            for (int i = 0; i < 20; i++)
            {
                Console.Write("█");
                await Task.Delay(200);
            }

            Console.WriteLine("\nConcluído...");
        }

        public static async Task CarregandoApresentacao()
        {
            for (int i = 0; i < 30; i++)
            {
                Console.Write("█");
                await Task.Delay(100);
            }
        }

        public static async Task CarregandoTurnoInimigo()
        {
            Console.WriteLine("Carregando a ação do inimigo...");
            for (int i = 0; i < 30; i++)
            {
                Console.Write("█");
                await Task.Delay(400);
            }

            Console.WriteLine();
        }

    }
}