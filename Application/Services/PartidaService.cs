using TicketToRide.Application.DTOs;
using TicketToRide.Domain.Entities;
using TicketToRide.Domain.Enums;
using TicketToRide.Domain.Interfaces;

namespace TicketToRide.Application.Services
{
    public class PartidaService
    {
        private readonly IPartidaRepository _partidaRepository;

        public PartidaService(IPartidaRepository partidaRepository)
        {
            _partidaRepository = partidaRepository;
        }

        public PartidaDTO CriarPartida()
        {
            Partida partida = _partidaRepository.CriarPartida();
            InicializarDadosJogo(partida);
            _partidaRepository.SalvarPartida(partida);
            return MapearParaDTO(partida);
        }

        public PartidaDTO? ObterPartida(string id)
        {
            Partida? partida = _partidaRepository.ObterPartida(id);
            return partida != null ? MapearParaDTO(partida) : null;
        }

        public PartidaDTO IniciarPartida(string id, int numJogadores)
        {
            Partida? partida = _partidaRepository.ObterPartida(id);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            partida.IniciarPartida(numJogadores);
            _partidaRepository.SalvarPartida(partida);
            return MapearParaDTO(partida);
        }

        public PartidaDTO FinalizarPartida(string id)
        {
            Partida? partida = _partidaRepository.ObterPartida(id);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            partida.FinalizarPartida();
            _partidaRepository.SalvarPartida(partida);
            return MapearParaDTO(partida);
        }

        public List<PartidaDTO> ObterTodasPartidas()
        {
            List<Partida> partidas = _partidaRepository.ObterTodasPartidas();
            return partidas.Select(MapearParaDTO).ToList();
        }

        public bool ExistePartida(string id)
        {
            return _partidaRepository.ExistePartida(id);
        }

        private void InicializarDadosJogo(Partida partida)
        {
            // Inicializar tabuleiro com rotas
            List<Rota> rotas = DadosJogo.ObterRotas();
            partida.Tabuleiro.AdicionarRotas(rotas);

            // Inicializar baralho de bilhetes de destino
            List<BilheteDestino> bilhetes = DadosJogo.ObterBilhetesDestino();
            partida.BaralhoCartasDestino.InicializarBaralho(bilhetes);
        }

        private PartidaDTO MapearParaDTO(Partida partida)
        {
            return new PartidaDTO
            {
                Id = partida.Id,
                Jogadores = partida.Jogadores.ConvertAll(MapearJogadorParaDTO),
                Rotas = partida.Tabuleiro.Rotas.ConvertAll(MapearRotaParaDTO),
                TurnoAtual = partida.TurnoAtual != null ? MapearTurnoParaDTO(partida.TurnoAtual) : null,
                PartidaIniciada = partida.PartidaIniciada,
                PartidaFinalizada = partida.PartidaFinalizada,
                NumeroJogadores = partida.Jogadores.Count,
                PodeIniciar = partida.PodeIniciar()
            };
        }

        private JogadorDTO MapearJogadorParaDTO(Jogador jogador)
        {
            return new JogadorDTO
            {
                Id = jogador.Id,
                Nome = jogador.Nome,
                Pontuacao = jogador.Pontuacao,
                PecasTremRestante = jogador.PecasTremRestante,
                MaoCartas = jogador.MaoCartas.ConvertAll(MapearCartaParaDTO),
                BilhetesDestino = jogador.BilhetesDestino.ConvertAll(MapearBilheteParaDTO),
                RotasConquistadas = jogador.RotasConquistadas.ConvertAll(MapearRotaParaDTO),
                NumeroCartas = jogador.MaoCartas.Count,
                NumeroBilhetes = jogador.BilhetesDestino.Count,
                NumeroRotas = jogador.RotasConquistadas.Count
            };
        }

        private RotaDTO MapearRotaParaDTO(Rota rota)
        {
            return new RotaDTO
            {
                Id = rota.Id,
                Origem = rota.Origem.Nome,
                Destino = rota.Destino.Nome,
                Cor = rota.Cor,
                Tamanho = rota.Tamanho,
                EhDupla = rota.EhDupla,
                EstaDisponivel = rota.Disponivel,
                Pontos = rota.CalcularPontos()
            };
        }

        private CartaDTO MapearCartaParaDTO(CartaVeiculo carta)
        {
            return new CartaDTO
            {
                Nome = carta.Nome,
                Cor = carta.Cor,
                EhLocomotiva = carta.Cor == Cor.LOCOMOTIVA,
                Descricao = carta.ToString()
            };
        }

        private BilheteDestinoDTO MapearBilheteParaDTO(BilheteDestino bilhete)
        {
            return new BilheteDestinoDTO
            {
                Origem = bilhete.Origem.Nome,
                Destino = bilhete.Destino.Nome,
                Pontos = bilhete.Pontos,
                IsCompleto = false, // Será calculado no contexto da partida
                Descricao = bilhete.ToString()
            };
        }

        private TurnoDTO MapearTurnoParaDTO(Turno turno)
        {
            return new TurnoDTO
            {
                Numero = turno.Numero,
                JogadorId = turno.JogadorAtual.Id,
                JogadorNome = turno.JogadorAtual.Nome,
                AcaoRealizada = turno.AcaoRealizada,
                AcaoCompletada = turno.AcaoCompletada,
                PodeExecutarAcao = turno.PodeExecutarAcao()
            };
        }
    }
}