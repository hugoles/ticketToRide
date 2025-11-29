using TicketToRide.Application.DTOs;
using TicketToRide.Application.Mappers.Interfaces;
using TicketToRide.Application.Services.Interfaces;
using TicketToRide.Domain.Entities;
using TicketToRide.Domain.Enums;
using TicketToRide.Domain.Interfaces;

namespace TicketToRide.Application.Services
{
    public class TurnoService : ITurnoService
    {
        private readonly IPartidaRepository _partidaRepository;
        private readonly IMapper _mapper;

        public TurnoService(
            IPartidaRepository partidaRepository,
            IMapper mapper)
        {
            _partidaRepository = partidaRepository;
            _mapper = mapper;
        }

        public TurnoDTO? ObterTurnoAtual(string partidaId)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId) ?? throw new ArgumentException("Partida não encontrada");

            return MapearTurnoAtual(partida);
        }

        public TurnoDTO ComprarCartasVeiculo(string partidaId, string jogadorId, List<int> indicesCartasReveladas = null)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId) ?? throw new ArgumentException("Partida não encontrada");

            ValidarTurnoJogador(jogadorId, partida);

            Jogador? jogador = partida.ObterJogador(jogadorId) ?? throw new ArgumentException("Jogador não encontrado");

            IEnumerable<CartaVeiculo?> cartasCompradas = ComprarCartas(indicesCartasReveladas, partida);

            jogador.AdicionarCartasVeiculo(cartasCompradas);

            partida.ExecutarAcaoTurno(Acao.COMPRAR_CARTAS_VEICULO);

            _partidaRepository.SalvarPartida(partida);

            return MapearTurnoAtual(partida);
        }

        private static IEnumerable<CartaVeiculo?> ComprarCartas(List<int> indicesCartasReveladas, Partida partida)
        {
            if (indicesCartasReveladas?.Any() != true)
            {
                return partida.ComprarCartaVeiculo(2);
            }

            if (indicesCartasReveladas.Count > 2)
            {
                throw new InvalidOperationException("Jogador pode comprar no máximo 2 cartas reveladas");
            }

            return [.. indicesCartasReveladas
                .Select(i => partida.ComprarCartaRevelada(i))
                .Where(c => c != null)];
        }

        public TurnoDTO ReivindicarRota(string partidaId, string jogadorId, string rotaId, IEnumerable<int> cartasSelecionadas = null)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId) ?? throw new ArgumentException("Partida não encontrada");

            ValidarTurnoJogador(jogadorId, partida);

            Jogador? jogador = partida.ObterJogador(jogadorId) ?? throw new ArgumentException("Jogador não encontrado");

            Rota? rota = partida.Tabuleiro.ObterRotaPorId(rotaId) ?? throw new ArgumentException("Rota não encontrada");

            if (!rota.Disponivel)
            {
                throw new InvalidOperationException("Rota já foi conquistada");
            }

            if (!jogador.TemCartasSuficientesParaConquistarRota(rota, cartasSelecionadas))
            {
                throw new InvalidOperationException("As cartas selecionadas não servem para essa rota");
            }

            if (!jogador.TemPecasSuficientesParaConquistarRota(rota))
            {
                throw new InvalidOperationException("Jogador não tem peças de trem suficientes");
            }

            jogador.ConquistarRota(rota, cartasSelecionadas);

            partida.ExecutarAcaoTurno(Acao.REIVINDICAR_ROTA);

            _partidaRepository.SalvarPartida(partida);

            return MapearTurnoAtual(partida);
        }

        public TurnoDTO ComprarBilhetesDestino(string partidaId, string jogadorId, List<int> bilhetesSelecionados, bool primeiroTurno)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId) ?? throw new ArgumentException("Partida não encontrada");

            ValidarTurnoJogador(jogadorId, partida);

            Jogador? jogador = partida.ObterJogador(jogadorId) ?? throw new ArgumentException("Jogador não encontrado");

            List<BilheteDestino> bilhetesComprados = [];

            if (bilhetesSelecionados.Count == 0)
            {
                throw new InvalidOperationException("Jogador deve manter pelo menos 1 bilhete");
            }

            foreach (int indice in bilhetesSelecionados)
            {
                BilheteDestino? carta = partida.ComprarBilheteDestino(indice);
                if (carta != null)
                {
                    bilhetesComprados.Add(carta);
                }
            }

            jogador.AdicionarBilhetesDestino(bilhetesComprados);

            partida.DescartarBilhetesNaoEscolhidos(bilhetesSelecionados);

            if (!primeiroTurno)
            {
                partida.ExecutarAcaoTurno(Acao.COMPRAR_BILHETES_DESTINO);
            }

            _partidaRepository.SalvarPartida(partida);
            return MapearTurnoAtual(partida);
        }

        private static void ValidarTurnoJogador(string jogadorId, Partida partida)
        {
            if (!partida.Iniciada)
            {
                throw new InvalidOperationException("Partida não foi iniciada");
            }

            if (!partida.EstaNaVezDoJogador(jogadorId))
            {
                throw new InvalidOperationException("Não é a vez deste jogador");
            }
        }

        private TurnoDTO? MapearTurnoAtual(Partida partida)
        {
            return partida.ObterTurnoAtual() is null ? null : _mapper.Map<Turno, TurnoDTO>(partida.ObterTurnoAtual());
        }
    }
}