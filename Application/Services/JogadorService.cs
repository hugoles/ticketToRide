using TicketToRide.Domain.Entities;
using TicketToRide.Domain.Interfaces;
using TicketToRide.Application.DTOs;
using TicketToRide.Domain.Enums;

namespace TicketToRide.Application.Services
{
    public class JogadorService
    {
        private readonly IPartidaRepository _partidaRepository;

        public JogadorService(IPartidaRepository partidaRepository)
        {
            _partidaRepository = partidaRepository;
        }

        public JogadorDTO AdicionarJogador(string partidaId, string nome)
        {
            var partida = _partidaRepository.ObterPartida(partidaId);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            if (partida.PartidaIniciada)
            {
                throw new InvalidOperationException("Não é possível adicionar jogadores após o início da partida");
            }

            if (partida.Jogadores.Count >= 5)
            {
                throw new InvalidOperationException("Número máximo de jogadores atingido (5)");
            }

            var jogadorId = $"JOGADOR_{partida.Jogadores.Count + 1}";
            var jogador = new Jogador(jogadorId, nome);

            partida.AdicionarJogador(jogador);
            _partidaRepository.SalvarPartida(partida);

            return MapearJogadorParaDTO(jogador);
        }

        public JogadorDTO? ObterJogador(string partidaId, string jogadorId)
        {
            var partida = _partidaRepository.ObterPartida(partidaId);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            var jogador = partida.ObterJogador(jogadorId);
            return jogador != null ? MapearJogadorParaDTO(jogador) : null;
        }

        public List<JogadorDTO> ObterJogadores(string partidaId)
        {
            var partida = _partidaRepository.ObterPartida(partidaId);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            return partida.Jogadores.Select(MapearJogadorParaDTO).ToList();
        }

        public bool RemoverJogador(string partidaId, string jogadorId)
        {
            var partida = _partidaRepository.ObterPartida(partidaId);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            if (partida.PartidaIniciada)
            {
                throw new InvalidOperationException("Não é possível remover jogadores após o início da partida");
            }

            var jogador = partida.ObterJogador(jogadorId);
            if (jogador == null)
            {
                return false;
            }

            partida.Jogadores.Remove(jogador);
            _partidaRepository.SalvarPartida(partida);
            return true;
        }

        public List<JogadorDTO> ObterRanking(string partidaId)
        {
            var partida = _partidaRepository.ObterPartida(partidaId);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            var ranking = partida.ObterRanking();
            return ranking.Select(MapearJogadorParaDTO).ToList();
        }

        private JogadorDTO MapearJogadorParaDTO(Jogador jogador)
        {
            return new JogadorDTO
            {
                Id = jogador.Id,
                Nome = jogador.Nome,
                Pontuacao = jogador.Pontuacao,
                PecasTremRestante = jogador.PecasTremRestante,
                MaoCartas = jogador.MaoCartas.Select(c => new CartaDTO
                {
                    Nome = c.Nome,
                    Cor = c.Cor,
                    EhLocomotiva = c.Cor == Cor.LOCOMOTIVA,
                    Descricao = c.ToString()
                }).ToList(),
                BilhetesDestino = jogador.BilhetesDestino.Select(b => new BilheteDestinoDTO
                {
                    Origem = b.Origem.Nome,
                    Destino = b.Destino.Nome,
                    Pontos = b.Pontos,
                    IsCompleto = false,
                    Descricao = b.ToString()
                }).ToList(),
                RotasConquistadas = jogador.RotasConquistadas.Select(r => new RotaDTO
                {
                    Id = r.Id,
                    Origem = r.Origem.Nome,
                    Destino = r.Destino.Nome,
                    Cor = r.Cor,
                    Tamanho = r.Tamanho,
                    EhDupla = r.EhDupla,
                    EstaDisponivel = r.Disponivel,
                    Pontos = r.CalcularPontos()
                }).ToList(),
                NumeroCartas = jogador.MaoCartas.Count,
                NumeroBilhetes = jogador.BilhetesDestino.Count,
                NumeroRotas = jogador.RotasConquistadas.Count
            };
        }
    }
}