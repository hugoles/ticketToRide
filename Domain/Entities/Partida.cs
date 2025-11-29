using TicketToRide.Domain.Enums;
using TicketToRide.Domain.Interfaces;
using TicketToRideAPI.Domain.Interfaces;

namespace TicketToRide.Domain.Entities
{
    public class Partida : IPartidaSubject
    {
        private const int TurnoInicial = 1;
        private Turno? turnoAtual;
        private readonly List<Jogador> jogadores = [];

        public string Id { get; }
        public IReadOnlyList<Jogador> Jogadores => jogadores.AsReadOnly();
        public Tabuleiro Tabuleiro { get; }
        public BaralhoCartasDestino BaralhoCartasDestino { get; }
        public BaralhoCartasVeiculo BaralhoCartasVeiculo { get; }
        public bool Iniciada { get; private set; }
        public bool Finalizada { get; private set; }

        private readonly List<IObserver> _observadores = [];

        private Partida(
            string id,
            Tabuleiro tabuleiro,
            BaralhoCartasDestino baralhoDestino,
            BaralhoCartasVeiculo baralhoVeiculo)
        {
            Id = id;
            Tabuleiro = tabuleiro;
            BaralhoCartasDestino = baralhoDestino;
            BaralhoCartasVeiculo = baralhoVeiculo;
        }

        public static Partida CriarPartida(
            string id,
            Tabuleiro tabuleiro,
            BaralhoCartasDestino baralhoDestino,
            BaralhoCartasVeiculo baralhoVeiculo)
        {
            return new(id, tabuleiro, baralhoDestino, baralhoVeiculo);
        }

        public void IniciarPartida()
        {
            turnoAtual = new Turno(TurnoInicial, Jogadores[0]);
            Iniciada = true;

            Notify();
        }

        public void FinalizarPartida()
        {
            Finalizada = true;

            Notify();
        }

        public Turno? ObterTurnoAtual() => turnoAtual;

        public void AdicionarJogador(Jogador jogador) => jogadores.Add(jogador);

        public void RemoverJogador(Jogador jogador) => jogadores.Remove(jogador);

        public Jogador? CalcularRotaMaisLonga()
        {
            return Jogadores
                .Where(j => j.RotasConquistadas.Count > 0)
                .Select(j => new { Jogador = j, Comprimento = j.CalcularComprimentoRotaContinua() })
                .OrderByDescending(x => x.Comprimento)
                .FirstOrDefault()
                ?.Jogador;
        }

        public Turno AvancarTurno()
        {
            Jogador proximoJogador = ObterProximoJogador(turnoAtual?.JogadorAtual);
            Turno proximoTurno = new(turnoAtual.Numero + 1, proximoJogador);
            turnoAtual = proximoTurno;
            return proximoTurno;
        }

        private Jogador ObterProximoJogador(Jogador jogadorAtual)
        {
            int indiceAtual = jogadores.IndexOf(jogadorAtual);
            int proximoIndice = (indiceAtual + 1) % Jogadores.Count;
            return Jogadores[proximoIndice];
        }

        public Jogador? ObterJogador(string jogadorId)
        {
            return Jogadores.FirstOrDefault(j => j.Id == jogadorId);
        }

        public List<Jogador> ObterRanking()
        {
            return [.. Jogadores.OrderByDescending(j => j.Pontuacao)];
        }

        public override string ToString()
        {
            return $"Partida {Id} - {Jogadores.Count} jogadores - {ObterStatusPartida()}";
        }

        private string ObterStatusPartida()
        {
            if (Finalizada)
            {
                return "Finalizada";
            }

            if (Iniciada)
            {
                return "Em Andamento";
            }

            return "Aguardando";
        }

        public bool EstaNaVezDoJogador(string jogadorId)
        {
            return turnoAtual?.JogadorAtual.Id == jogadorId;
        }

        public void ExecutarAcaoTurno(Acao acaoRealizada)
        {
            turnoAtual?.SalvarAcaoRealizada(acaoRealizada);
            AvancarTurno();
        }

        public CartaVeiculo? ComprarCartaRevelada(int indice)
        {
            return BaralhoCartasVeiculo.ComprarCartaRevelada(indice);
        }

        public IEnumerable<CartaVeiculo?> ComprarCartaVeiculo(int num)
        {
            return BaralhoCartasVeiculo.Comprar(num);
        }

        public void DescartarBilhetesNaoEscolhidos(IEnumerable<int> indicesEscolhidos)
        {
            BaralhoCartasDestino.DescartarNaoEscolhidas(indicesEscolhidos);
        }

        public BilheteDestino? ComprarBilheteDestino(int indice)
        {
            return BaralhoCartasDestino.ComprarCartaDestino(indice);
        }

        public void Attach(IObserver observer)
        {
            if (!_observadores.Contains(observer))
            {
                _observadores.Add(observer);
            }
        }

        public void Detach(IObserver observer)
        {
            _observadores.Remove(observer);
        }

        public void Notify()
        {
            foreach (IObserver observer in _observadores)
            {
                observer.Update(this);
            }
        }
    }
}