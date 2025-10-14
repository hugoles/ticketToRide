namespace TicketToRide.Domain.Entities
{
    public class Partida
    {
        public string Id { get; set; } = string.Empty;
        public List<Jogador> Jogadores { get; set; } = [];
        public Tabuleiro Tabuleiro { get; set; }
        public BaralhoCartasDestino BaralhoCartasDestino { get; set; }
        public BaralhoCartasVeiculo BaralhoCartasVeiculo { get; set; }
        public Turno? TurnoAtual { get; set; }
        public bool PartidaIniciada { get; set; } = false;
        public bool PartidaFinalizada { get; set; } = false;

        public Partida()
        {
            Tabuleiro = new Tabuleiro();
            BaralhoCartasDestino = new BaralhoCartasDestino();
            BaralhoCartasVeiculo = new BaralhoCartasVeiculo();
        }

        public Partida IniciarPartida(int numJogadores)
        {
            if (numJogadores < 2 || numJogadores > 5)
            {
                throw new ArgumentException("Número de jogadores deve ser entre 2 e 5");
            }

            if (Jogadores.Count != numJogadores)
            {
                throw new InvalidOperationException($"Número de jogadores não corresponde ao esperado. Esperado: {numJogadores}, Atual: {Jogadores.Count}");
            }

            PartidaIniciada = true;

            foreach (Jogador jogador in Jogadores)
            {
                List<CartaVeiculo> cartasIniciais = BaralhoCartasVeiculo.ComprarVarias(4);
                jogador.ComprarCartas(cartasIniciais);
            }

            TurnoAtual = new Turno(1, Jogadores[0]);

            return this;
        }

        public void FinalizarPartida()
        {
            if (!PartidaIniciada)
            {
                throw new InvalidOperationException("Partida não foi iniciada");
            }

            PartidaFinalizada = true;

            CalcularPontuacaoFinal();
        }

        public void CalcularPontuacaoFinal()
        {
            foreach (Jogador jogador in Jogadores)
            {
                jogador.AtualizarPontuacao();
            }

            Jogador? vencedorRotaLonga = CalcularRotaMaisLonga();
            if (vencedorRotaLonga != null)
            {
                vencedorRotaLonga.Pontuacao += 10;
            }
        }

        public Jogador? CalcularRotaMaisLonga()
        {
            Jogador? vencedor = null;
            int maiorComprimento = 0;

            foreach (Jogador jogador in Jogadores)
            {
                int comprimento = jogador.CalcularComprimentoRotaContinua();
                if (comprimento > maiorComprimento)
                {
                    maiorComprimento = comprimento;
                    vencedor = jogador;
                }
            }

            return vencedor;
        }

        public Turno CriarProximoTurno()
        {
            if (TurnoAtual == null)
            {
                throw new InvalidOperationException("Não há turno atual");
            }

            Jogador proximoJogador = ObterProximoJogador(TurnoAtual.JogadorAtual);
            Turno proximoTurno = new(TurnoAtual.Numero + 1, proximoJogador);
            TurnoAtual = proximoTurno;

            return proximoTurno;
        }

        private Jogador ObterProximoJogador(Jogador jogadorAtual)
        {
            int indiceAtual = Jogadores.IndexOf(jogadorAtual);
            int proximoIndice = (indiceAtual + 1) % Jogadores.Count;
            return Jogadores[proximoIndice];
        }

        public void AdicionarJogador(Jogador jogador)
        {
            if (PartidaIniciada)
            {
                throw new InvalidOperationException("Não é possível adicionar jogadores após o início da partida");
            }

            if (Jogadores.Count >= 5)
            {
                throw new InvalidOperationException("Número máximo de jogadores atingido (5)");
            }

            Jogadores.Add(jogador);
        }

        public Jogador? ObterJogador(string jogadorId)
        {
            return Jogadores.FirstOrDefault(j => j.Id == jogadorId);
        }

        public bool PodeIniciar()
        {
            return Jogadores.Count >= 2 && Jogadores.Count <= 5 && !PartidaIniciada;
        }

        public List<Jogador> ObterRanking()
        {
            return [.. Jogadores.OrderByDescending(j => j.Pontuacao)];
        }

        public override string ToString()
        {
            string status = PartidaFinalizada ? "Finalizada" : PartidaIniciada ? "Em Andamento" : "Aguardando";
            return $"Partida {Id} - {Jogadores.Count} jogadores - {status}";
        }
    }
}