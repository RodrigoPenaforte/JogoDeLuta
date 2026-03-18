# Sistema de Combate (JogoDeLuta)

Se você baixou esse código e quer usar/entender rápido... Vou te explicar como eu montei o combate e onde você mexe pra criar classe, skill e efeito :)

O objetivo do projeto é ser um “RPG de turno simples” no console, mas com as partes legais: dano físico/mágico, nível, custo de energia/stamina e buffs/debuffs.

---

## Como o jogo roda (bem por cima)

- `Program.cs` chama o menu.
- `MenuJogoService` mostra as opções e quando você escolhe jogar ele chama:
  - `EscolherPersonagemService` (pra escolher o personagem)
  - `CombateService` (pra iniciar a luta)

---

## Onde eu configuro classes e habilidades?

Quase tudo que é balanceamento e criação de conteúdo tá aqui:

- `Service/ClasseService.cs`

É ali que eu defino:

- **Classes** (`Guerreiro`, `Mago`, etc.)
- **Atributos base** (HP/Energia/Stamina, ataque e defesa)
- **Crescimento por nível**
- **Habilidades**
- **Efeitos** dentro das habilidades (buff/debuff)

Resumo da ópera: se você quer “inventar gameplay”, 90% do tempo é nesse arquivo.

---

## O que é “Classe” nesse jogo?

Arquivo:

- `Models/Classe.cs`

Eu separo os atributos em três blocos:

### 1) Vida e recursos (base)

- `HPBase`: vida no nível 1
- `EnergiaBase`: energia no nível 1
- `StaminaBase`: stamina no nível 1

### 2) Crescimento por nível

- `HPPorNivel`
- `EnergiaPorNivel`
- `StaminaPorNivel`
- `AtaqueFisicoPorNivel`, `AtaqueMagicoPorNivel`
- `DefesaFisicaPorNivel`, `DefesaMagicaPorNivel`

### 3) Ataque/Defesa por tipo de dano

Aqui é o pulo do gato, físico e mágico são mundos diferentes.

- `AtaqueFisico` / `AtaqueMagico`
- `DefesaFisica` / `DefesaMagica`

E ainda tem:

- `ReducaoDanoBase`: redução percentual geral (tipo mitigação final)
- `RecuperacaoEnergia` e `RecuperacaoStamina`: usados quando você escolhe **Descansar** (recuperação de recursos)

---

## Nível: como o personagem fica mais forte (sem ficar op e quebrar o jogo)

Arquivo:

- `Models/Personagem.cs`

Eu tenho um detalhe importante aqui: **não uso nível linear puro**, porque isso explode o jogo em level alto.

### Escala do nível (retorno decrescente)

Eu uso:

- `EscalaNivel() = floor(sqrt(Nivel - 1))`

Então o crescimento vai ficando mais lento conforme sobe nível. Isso segura muito o “snowball”.


### Aplicar nível

Sempre que eu crio um `Personagem` (jogador ou inimigo), eu chamo `AplicarNivel()` pra calcular:

- `HPMax = HPBase + HPPorNivel * escala`
- `EnergiaMax = EnergiaBase + EnergiaPorNivel * escala`
- `StaminaMax = StaminaBase + StaminaPorNivel * escala`

E aí ele começa cheio (HP/Energia/Stamina no máximo) quando é uma instância nova.

---

## Dano físico vs dano mágico

Enum:

- `Enum/TipoDano.cs` (`Fisico`, `Magico`)

Na habilidade (`Models/Habilidade.cs`) tem:

- `Tipo` → diz se é físico ou mágico

E isso muda qual defesa o alvo vai usar:

- se for físico → `DefesaFisica`
- se for mágico → `DefesaMagica`

---

## Como o dano é calculado (sem enrolação)

Arquivo:

- `Models/Combate.cs` (método `UsarHabilidade`)

O cálculo é esse “combo”:

1) Confere se tem energia/stamina suficiente  
2) Desconta o custo  
3) Calcula `bonusAtaque` (classe + nível + buffs/debuffs)  
4) Soma com o dano base da habilidade  
5) Aplica modificador de dano (se tiver) + variação aleatória pequena  
6) Manda pro `ReceberDano` (que aplica defesa + redução percentual)  
7) Mostra log no console (dano bruto, bloqueado, dano final, etc.)  

Se o `danoFinal` virar 0, eu já aviso que o alvo bloqueou completamente.

---

## Regrinhas do turno (qualidade de vida)

No seu turno:

- Você pode **atacar**, **descansar** (gasta turno) ou **ver seu status** (não gasta turno).
- Se você tentar usar uma habilidade que você **não tem nível** pra usar, o jogo te avisa e te deixa escolher outra (não passa o turno).
- Se você não tiver **energia/stamina** pra skill, mesma coisa: ele te avisa e você escolhe outra.

E o inimigo também respeita nível mínimo das habilidades.

---

## Buff/Debuff (efeitos): onde a brincadeira começa

### Enums

- `Enum/TipoEfeito.cs`: `Buff` ou `Debuff`
- `Enum/MomentoEfeito.cs`:
  - `AoAcertar`
  - `InicioDoTurno`
  - `FimDoTurno`

### Modelos

- `Models/Efeito.cs`: a “descrição” do efeito (nome, duração, DOT - (dano ao longo do tempo), modificadores…)
- `Models/EfeitoAtivo.cs`: o efeito rodando no personagem (turnos restantes, stacks…)

### Onde eu guardo os efeitos ativos

No `Personagem`:

- `List<EfeitoAtivo> EfeitosAtivos`

### Como o efeito entra no jogo

Quando uma habilidade acerta, eu percorro `habilidade.Efeitos` e aplico no alvo.  
O efeito **entra na lista na hora do hit**, mas ele **executa** só no momento certo:

- `InicioDoTurno`: roda no começo do turno do alvo
- `FimDoTurno`: roda no fim do turno do alvo

### Duração (como eu conto “1 turno”)

Eu decidi assim:

- A duração decrementa no `ProcessarFimDoTurno()`.

Então:

- `DuracaoTurnos = 1` significa “o alvo ainda joga o turno dele, e no fim do turno sofre o efeito e ele acaba”.

### Não acumulativo x acumulativo

No `Personagem.AplicarEfeito`:

- Se `PodeAcumular = false` e o efeito já existe → **não reaplica e não reseta** (o console avisa).
- Se `PodeAcumular = true` → pode stackar até `MaxStacks` e renovar duração.

---

## O que cada modificador do efeito faz (em português normal)

No `Efeito.cs` eu deixei duas famílias principais:

### Mod de “Ataque” (pega mais no scaling)

- `ModAtaqueFisicoPct` / `ModAtaqueMagicoPct`

Isso mexe no **bônus de ataque** (classe + nível).  
Ex.: debuff de -20% 

### Mod de “Dano” (pega no dano total)

- `ModDanoFisicoPct` / `ModDanoMagicoPct`

Isso mexe no dano final do golpe (depois de somar `habilidade.Dano + bônus`).  
Se você quer reduzir mesmo o estrago da habilidade, é aqui.

### Defesa plana

- `ModDefesaFisica` / `ModDefesaMagica`

Soma direto na defesa do alvo enquanto o efeito estiver ativo.

### DOT (dano por turno)

- `DanoPorTurno`

Se o `Momento` for `InicioDoTurno` ou `FimDoTurno`, eu aplico esse dano automaticamente.

---

## “Tá desbalanceado”, onde eu mexo?

Checklist rápido:

- Tá morrendo rápido demais → aumenta `HPBase`, sobe defesa, diminui dano base das skills
- Tá demorando demais → sobe dano base e/ou reduz defesa/redução
- Level alto quebrando tudo → mexe no `EscalaNivel()` e/ou reduz os `*PorNivel`

Lembrando que isso aqui é um sistema de **console livre**:

- você inventa o **nome do personagem**
- inventa o **nome da classe**
- decide quais **atributos** essa classe tem (HP/Energia/Stamina/ataque/defesa/crescimento)
- cria quantas **habilidades** quiser e enfia **efeitos** nelas

Eu só deixei uma regra fixa pra não virar bagunça: **nível máximo é 20**. Se tentar rodar com nível > 20, o sistema avisa e não inicia o combate. Se você quiser mudar isso, é só ajustar o `NivelMaximo` em `Models/Personagem.cs`.

Arquivos que você vai abrir toda hora:

- `Service/ClasseService.cs` (conteúdo + números)
- `Models/Personagem.cs` (nível + defesa)
- `Models/Combate.cs` (dano)

---

## Ideias de efeitos (caso você queira expandir)

- “Barreira Arcana” (Buff): +Defesa mágica por 2 turnos
- “Silêncio” (Debuff): impede usar habilidade mágica
- “Exausto” (Debuff): aumenta custo de stamina
- “Foco” (Buff): reduz custo de energia

---

## (bem curto)

Eu configuro classes/habilidades no `ClasseService`. O dano é físico/mágico, o nível cresce com √(nível-1) pra segurar o power creep, e buffs/debuffs são efeitos que entram no hit e rodam no início/fim do turno com duração e regra de acumular ou não.

