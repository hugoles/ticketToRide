using TicketToRide.Application.DTOs;
using TicketToRide.Domain.Entities;
using TicketToRide.Domain.Enums;
using TicketToRide.Domain.Interfaces;
using TicketToRideApi.Domain.Events;
using TicketToRideApi.Domain.Interfaces;

namespace TicketToRide.Application.Services
{
    public class TurnoService
    {
        private readonly IPartidaRepository _partidaRepository;
        private readonly IDomainEventDispatcher _eventDispatcher;

        public TurnoService(
            IPartidaRepository partidaRepository,
            IDomainEventDispatcher eventDispatcher)
        {
            _partidaRepository = partidaRepository;
            _eventDispatcher = eventDispatcher;
        }

        public TurnoDTO ObterTurnoAtual(string partidaId)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId) ?? throw new ArgumentException("Partida não encontrada");
            if (partida.TurnoAtual == null)
            {
                throw new InvalidOperationException("Não há turno ativo");
            }

            return partida.TurnoAtual.MapearParaDTO();
        }

        public TurnoDTO ComprarCartasVeiculo(string partidaId, string jogadorId, List<int> indices = null)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId) ?? throw new ArgumentException("Partida não encontrada");
            if (!partida.Iniciada)
            {
                throw new InvalidOperationException("Partida não foi iniciada");
            }

            if (!partida.EstaNaVezDoJogador(jogadorId))
            {
                throw new InvalidOperationException("Não é a vez deste jogador");
            }

            Jogador? jogador = partida.ObterJogador(jogadorId) ?? throw new ArgumentException("Jogador não encontrado");
            List<CartaVeiculo> cartasCompradas = [];

            if (indices?.Any() == true)
            {
                foreach (int indice in indices)
                {
                    CartaVeiculo? carta = partida.BaralhoCartasVeiculo.ComprarCartaRevelada(indice);
                    if (carta != null)
                    {
                        cartasCompradas.Add(carta);
                    }
                }
            }
            else
            {
                cartasCompradas = partida.BaralhoCartasVeiculo.Comprar(2);
            }

            jogador.AdiconarCartasVeiculo(cartasCompradas);
            partida.ExecutarAcaoTurno(Acao.COMPRAR_CARTAS_VEICULO);

            _partidaRepository.SalvarPartida(partida);
            return partida.TurnoAtual.MapearParaDTO();
        }

        public TurnoDTO ReivindicarRota(string partidaId, string jogadorId, string rotaId)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId) ?? throw new ArgumentException("Partida não encontrada");
            if (!partida.Iniciada)
            {
                throw new InvalidOperationException("Partida não foi iniciada");
            }

            if (!partida.EstaNaVezDoJogador(jogadorId))
            {
                throw new InvalidOperationException("Não é a vez deste jogador");
            }

            Jogador? jogador = partida.ObterJogador(jogadorId) ?? throw new ArgumentException("Jogador não encontrado");

            Rota? rota = partida.Tabuleiro.ObterRotaPorId(rotaId) ?? throw new ArgumentException("Rota não encontrada");

            if (!rota.Disponivel)
            {
                throw new InvalidOperationException("Rota já foi conquistada");
            }

            if (!jogador.TemCartasSuficientesParaConquistarRota(rota))
            {
                throw new InvalidOperationException("Jogador não tem cartas suficientes para esta rota");
            }

            if (!jogador.TemPecasSuficientesParaConquistarRota(rota))
            {
                throw new InvalidOperationException("Jogador não tem peças de trem suficientes");
            }

            jogador.ConquistarRota(rota);

            partida.ExecutarAcaoTurno(Acao.REIVINDICAR_ROTA);

            _eventDispatcher.Publish(new RotaReivindicadaEvent(
                IdPartida: partida.Id,
                IdJogador: jogador.Id,
                RotaReivindicada: rota,
                OcorridoEm: DateTime.UtcNow
            ));

            _partidaRepository.SalvarPartida(partida);
            return partida.TurnoAtual.MapearParaDTO();
        }

        public TurnoDTO ComprarBilhetesDestino(string partidaId, string jogadorId, List<string> bilhetesSelecionados)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId) ?? throw new ArgumentException("Partida não encontrada");
            if (!partida.EstaNaVezDoJogador(jogadorId))
            {
                throw new InvalidOperationException("Não é a vez deste jogador");
            }

            Jogador? jogador = partida.ObterJogador(jogadorId) ?? throw new ArgumentException("Jogador não encontrado");
            List<BilheteDestino> bilhetesComprados = partida.BaralhoCartasDestino.Comprar(3);

            if (bilhetesComprados.Count == 0)
            {
                throw new InvalidOperationException("Não há bilhetes disponíveis no baralho");
            }

            if (bilhetesSelecionados.Count == 0)
            {
                throw new InvalidOperationException("Jogador deve manter pelo menos 1 bilhete");
            }

            // Adicionar bilhetes selecionados à mão do jogador
            List<BilheteDestino> bilhetesParaManter = bilhetesComprados.Where(b =>
                bilhetesSelecionados.Contains($"{b.Origem.Nome}-{b.Destino.Nome}")).ToList();

            jogador.AdicionarBilhetesDestino(bilhetesParaManter);

            // Devolver bilhetes não selecionados ao baralho
            List<BilheteDestino> bilhetesParaDevolver = bilhetesComprados.Except(bilhetesParaManter).ToList();
            partida.BaralhoCartasDestino.Descartar(bilhetesParaDevolver);

            partida.ExecutarAcaoTurno(Acao.COMPRAR_BILHETES_DESTINO);

            _partidaRepository.SalvarPartida(partida);
            return partida.TurnoAtual.MapearParaDTO();
        }
    }
}