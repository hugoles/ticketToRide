# Ticket to Ride - Jogo Digital Completo

Este projeto implementa uma versão digital e simplificada do jogo de tabuleiro "Ticket to Ride" usando ASP.NET Core Web API como backend e JavaScript com Bootstrap como frontend.

## 🚀 Tecnologias Utilizadas

- **Backend**: ASP.NET Core 9.0 Web API com Clean Architecture
- **Frontend**: HTML5, CSS3, JavaScript (Vanilla)
- **UI Framework**: Bootstrap 5.3.0
- **Ícones**: Font Awesome 6.0.0
- **Arquitetura**: Clean Architecture (Domain, Application, API)
- **Persistência**: Em memória (Dictionary estático)

## 📁 Estrutura do Projeto

```
TicketToRide Project/
├── Domain/                    # Camada de Domínio
│   ├── Entities/             # Entidades do jogo
│   │   ├── Partida.cs
│   │   ├── Jogador.cs
│   │   ├── Tabuleiro.cs
│   │   ├── Rota.cs
│   │   ├── Cidade.cs
│   │   ├── Turno.cs
│   │   ├── Carta.cs
│   │   ├── CartaVeiculo.cs
│   │   ├── BilheteDestino.cs
│   │   ├── Baralho.cs
│   │   ├── BaralhoCartasVeiculo.cs
│   │   └── BaralhoCartasDestino.cs
│   ├── Enums/
│   │   ├── Acao.cs
│   │   └── Cor.cs
│   └── Interfaces/
│       └── IPartidaRepository.cs
├── Application/              # Camada de Aplicação
│   ├── Services/
│   │   ├── PartidaService.cs
│   │   ├── JogadorService.cs
│   │   └── TurnoService.cs
│   ├── DTOs/
│   │   ├── PartidaDTO.cs
│   │   ├── JogadorDTO.cs
│   │   ├── RotaDTO.cs
│   │   ├── CartaDTO.cs
│   │   ├── BilheteDestinoDTO.cs
│   │   └── TurnoDTO.cs
│   ├── Repositories/
│   │   └── PartidaRepositoryMemory.cs
│   └── DadosJogo.cs         # Dados hardcoded do jogo
├── Controllers/             # Camada de API
│   ├── PartidaController.cs
│   ├── JogadorController.cs
│   └── TurnoController.cs
├── wwwroot/                 # Frontend
│   ├── index.html
│   ├── default.html
│   ├── css/style.css
│   └── js/
│       ├── app.js
│       ├── partida.js
│       └── jogo.js
├── Program.cs
└── TicketToRideAPI.csproj
```

## 🎮 Funcionalidades Implementadas

### ✅ Requisitos Funcionais Atendidos

- **RF01**: Gerenciamento de Partida Multijogador (2-5 jogadores)
- **RF02**: Mecânica de Turnos com 3 ações:
  - RF02.1: Comprar Cartas de Vagão
  - RF02.2: Reivindicar uma Rota
  - RF02.3: Comprar Bilhetes de Destino
- **RF03**: Gestão de Recursos (decks, embaralhamentos, mãos)
- **RF04**: Controle de Estado do Jogo (criar, iniciar, finalizar)
- **RF05**: Cálculo de Pontuação (imediato e final)

### ✅ Requisitos Não-Funcionais Atendidos

- **RNF01**: Desempenho < 3 segundos (em memória é instantâneo)
- **RNF02**: Interface intuitiva com representação visual
- **RNF03**: Regras oficiais aplicadas consistentemente
- **RNF04**: Suporta até 5 jogadores sem perda de performance

## 🛠️ Como Executar

### Pré-requisitos
- .NET 9.0 SDK (já instalado: versão 9.0.305)
- Navegador web moderno

### Passos para Executar

1. **Navegue até o diretório do projeto:**
   ```bash
   cd "...\TicketToRide Project"
   ```

2. **Execute o projeto:**
  Primeiro tem que buildar
   ```bash
   dotnet build
   ```
   ```bash
   dotnet run
   ```

3. **Acesse a aplicação:**
   - **Frontend**: https://localhost:5257
   - **API**: https://localhost:7000/api/partida ou http://localhost:5000/api/partida

## 🎯 Como Jogar

### 1. Configuração da Partida
- Adicione entre 2 e 5 jogadores
- Cada jogador deve ter um nome único
- Clique em "Iniciar Partida" quando estiver pronto

### 2. Durante o Jogo
- **Sua vez**: Execute uma das 3 ações disponíveis
- **Comprar Cartas**: Adquira cartas de vagão do monte
- **Reivindicar Rota**: Use cartas para conquistar rotas entre cidades
- **Comprar Bilhetes**: Adquira bilhetes de destino para pontos extras

### 3. Objetivo
- Conquistar rotas para conectar cidades
- Completar bilhetes de destino
- Acumular a maior pontuação possível

### 4. Pontuação
- **Rotas**: 1-15 pontos baseado no tamanho
- **Bilhetes**: Pontos extras se completos, penalidade se incompletos
- **Bônus**: 10 pontos para a rota contínua mais longa

## 🔧 API Endpoints

### Partida
- `POST /api/partida/criar` - Criar nova partida
- `GET /api/partida/{id}` - Obter estado da partida
- `POST /api/partida/{id}/iniciar` - Iniciar partida
- `POST /api/partida/{id}/finalizar` - Finalizar partida
- `GET /api/partida/{id}/pontuacao` - Obter pontuação

### Jogador
- `POST /api/jogador/partida/{id}/jogador` - Adicionar jogador
- `GET /api/jogador/partida/{id}/jogador/{jogadorId}` - Obter jogador
- `DELETE /api/jogador/partida/{id}/jogador/{jogadorId}` - Remover jogador

### Turno
- `GET /api/turno/partida/{id}/turno/atual` - Obter turno atual
- `POST /api/turno/partida/{id}/turno/comprar-cartas` - Comprar cartas
- `POST /api/turno/partida/{id}/turno/reivindicar-rota` - Reivindicar rota
- `POST /api/turno/partida/{id}/turno/comprar-bilhetes` - Comprar bilhetes

## 🗺️ Dados do Jogo

O jogo inclui:
- **36 cidades** da América do Norte
- **100+ rotas** entre cidades com cores e tamanhos variados
- **30 bilhetes de destino** com diferentes valores de pontos
- **110 cartas de vagão** (12 de cada cor + 14 locomotivas)

## 🐛 Solução de Problemas

### Erro de Certificado SSL
Se houver problemas com HTTPS:
1. Aceitar o certificado no navegador
2. Ou usar HTTP: http://localhost:5000

### Porta em Uso
Se as portas estiverem ocupadas, edite `Properties/launchSettings.json`

### CORS Issues
O CORS está configurado para localhost:3000 e 127.0.0.1:3000

## 🎉 Status do Projeto

✅ **COMPLETO** - O jogo está totalmente funcional com:
- Backend ASP.NET Core com Clean Architecture
- Frontend JavaScript responsivo
- Todas as regras do Ticket to Ride implementadas
- Interface intuitiva e moderna
- Sistema de pontuação completo
- Gerenciamento de partidas multijogador

O projeto está pronto para jogar! 🚂🎮
