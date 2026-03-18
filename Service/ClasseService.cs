using JogoDeLuta.Models;
using JogoDeLuta.Enum;

namespace JogoDeLuta.Service
{
    public class ClasseService
    {
        private List<Classe> classes = new();

        private static readonly Random rand = new();

        public ClasseService()
        {
            CriarClasses();
        }

        private void CriarClasses()
        {
            classes.Add(new Classe
            {
                Id = 1,
                Nome = "Guerreiro",
                HPBase = 40,
                EnergiaBase = 12,
                StaminaBase = 24,
                HPPorNivel = 6,
                EnergiaPorNivel = 2,
                StaminaPorNivel = 3,
                AtaqueFisicoPorNivel = 1,
                AtaqueMagicoPorNivel = 0,
                DefesaFisicaPorNivel = 1,
                DefesaMagicaPorNivel = 1,
                DefesaFisica = 3,
                DefesaMagica = 1,
                ReducaoDanoBase = 0.05,
                AtaqueFisico = 3,
                AtaqueMagico = 0,
                RecuperacaoEnergia = 1,
                RecuperacaoStamina = 4,
                Habilidades =
                [
                    new()
                    {
                        Nome = "Corte horizontal",
                        Dano = 8,
                        CustoEnergia = 2,
                        CustoStamina = 6,
                        NivelHabilidade = 1,
                        Tipo = TipoDano.Fisico
                    },
                    new()
                    {
                        Nome = "Golpe perfurativo",
                        Dano = 10,
                        CustoEnergia = 3,
                        CustoStamina = 9,
                        NivelHabilidade = 3,
                        Tipo = TipoDano.Fisico,
                        Efeitos = new()
                        {
                            new()
                            {
                                Nome = "Sangramento",
                                Descricao = "Perde HP no fim do turno.",
                                Tipo = TipoEfeito.Debuff,
                                Momento = MomentoEfeito.FimDoTurno,
                                DuracaoTurnos = 1,
                                PodeAcumular = false,
                                MaxStacks = 1,
                                DanoPorTurno = 2
                            }
                        }
                    },
                    new()
                    {
                        Nome = "Dois Cortes",
                        Dano = 14,
                        CustoEnergia = 6,
                        CustoStamina = 15,
                        NivelHabilidade = 5,
                        Tipo = TipoDano.Fisico
                    },
                    new()
                    {
                        Nome = "Chama do dragão",
                        Dano = 18,
                        CustoEnergia = 15,
                        CustoStamina = 24,
                        NivelHabilidade = 8,
                        Tipo = TipoDano.Fisico,
                        Efeitos = new()
                        {
                            new()
                            {
                                Nome = "Enfraquecido",
                                Descricao = "Reduz o ataque físico por alguns turnos.",
                                Tipo = TipoEfeito.Debuff,
                                Momento = MomentoEfeito.AoAcertar,
                                DuracaoTurnos = 2,
                                PodeAcumular = false,
                                ModAtaqueFisicoPct = -0.20
                            }
                        }
                    },
                ]
            });

            classes.Add(new Classe
            {
                Id = 2,
                Nome = "Mago",
                HPBase = 30,
                EnergiaBase = 10,
                StaminaBase = 10,
                HPPorNivel = 4,
                EnergiaPorNivel = 5,
                StaminaPorNivel = 2,
                AtaqueFisicoPorNivel = 0,
                AtaqueMagicoPorNivel = 2,
                DefesaFisicaPorNivel = 0,
                DefesaMagicaPorNivel = 1,
                DefesaFisica = 1,
                DefesaMagica = 3,
                ReducaoDanoBase = 0.03,
                AtaqueFisico = 1,
                AtaqueMagico = 3,
                RecuperacaoEnergia = 6,
                RecuperacaoStamina = 2,
                Habilidades =
                [
                    new() { Nome = "Cajado de Madeira", Dano = 6, CustoEnergia = 2, CustoStamina = 4, NivelHabilidade = 1, Tipo = TipoDano.Fisico},
                    new() { Nome = "Bola de Fogo pequena", Dano = 10, CustoEnergia = 7, CustoStamina = 2, NivelHabilidade = 3, Tipo = TipoDano.Magico},
                    new() { Nome = "Bola de Fogo média", Dano = 13, CustoEnergia = 10, CustoStamina = 3, NivelHabilidade = 5, Tipo = TipoDano.Magico},
                    new()
                    {
                        Nome = "Bola de Fogo grande",
                        Dano = 16,
                        CustoEnergia = 14,
                        CustoStamina = 4,
                        NivelHabilidade = 8,
                        Tipo = TipoDano.Magico,
                        Efeitos = new()
                        {
                            new()
                            {
                                Nome = "Queimadura",
                                Descricao = "Sofre dano no fim do turno.",
                                Tipo = TipoEfeito.Debuff,
                                Momento = MomentoEfeito.FimDoTurno,
                                DuracaoTurnos = 2,
                                PodeAcumular = false,
                                MaxStacks = 1,
                                DanoPorTurno = 3
                            }
                        }
                    },
                    new()
                    {
                        Nome = "Raio",
                        Dano = 25,
                        CustoEnergia = 30,
                        CustoStamina = 7,
                        NivelHabilidade = 12,
                        Tipo = TipoDano.Magico,
                        Efeitos = new()
                        {
                            new()
                            {
                                Nome = "Eletrocutado",
                                Descricao = "Reduz ataque físico e mágico por alguns turnos.",
                                Tipo = TipoEfeito.Debuff,
                                Momento = MomentoEfeito.AoAcertar,
                                DuracaoTurnos = 2,
                                PodeAcumular = false,
                                MaxStacks = 1,
                                ModAtaqueFisicoPct = -0.15,
                                ModAtaqueMagicoPct = -0.15
                            }
                        }
                    }
                ]
            });
        }

        public Classe ObterPorId(int id)
        {
            return classes.First(c => c.Id == id);
        }

    }
}