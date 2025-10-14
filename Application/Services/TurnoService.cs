using TicketToRide.Application.DTOs;
using TicketToRide.Domain.Entities;
using TicketToRide.Domain.Enums;
using TicketToRide.Domain.Interfaces;

namespace TicketToRide.Application.Services
{
    public class TurnoService
    {
        private readonly IPartidaRepository _partidaRepository;

        public TurnoService(IPartidaRepository partidaRepository)
        {
            _partidaRepository = partidaRepository;
        }

        public TurnoDTO ObterTurnoAtual(string partidaId)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            if (partida.TurnoAtual == null)
            {
                throw new InvalidOperationException("Não há turno ativo");
            }

            return MapearTurnoParaDTO(partida.TurnoAtual);
        }

        public TurnoDTO ComprarCartasVeiculo(string partidaId, string jogadorId, List<int> indicesCartasVisiveis = null)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            if (!partida.PartidaIniciada)
            {
                throw new InvalidOperationException("Partida não foi iniciada");
            }

            Jogador jogadorAtual = partida.TurnoAtual.JogadorAtual;
            if (!jogadorAtual.Id.Equals(jogadorId))
            {
                throw new InvalidOperationException("Não é a vez deste jogador");
            }

            Jogador? jogador = partida.ObterJogador(jogadorId);
            if (jogador == null)
            {
                throw new ArgumentException("Jogador não encontrado");
            }

            if (!jogador.PodeComprarCartas())
            {
                throw new InvalidOperationException("Jogador já tem o máximo de cartas (10)");
            }

            List<CartaVeiculo> cartasCompradas = new();

            if (indicesCartasVisiveis?.Any() == true)
            {
                // Comprar cartas visíveis específicas
                foreach (int indice in indicesCartasVisiveis)
                {
                    CartaVeiculo? carta = partida.BaralhoCartasVeiculo.ComprarCartaVisivel(indice);
                    if (carta != null)
                    {
                        cartasCompradas.Add(carta);
                    }
                }
            }
            else
            {
                // Comprar do monte
                cartasCompradas = partida.BaralhoCartasVeiculo.ComprarVarias(2);
            }

            jogador.ComprarCartas(cartasCompradas);
            partida.TurnoAtual.ExecutarAcao(Acao.COMPRAR_CARTAS_VEICULO);

            _partidaRepository.SalvarPartida(partida);
            return MapearTurnoParaDTO(partida.TurnoAtual);
        }

        public TurnoDTO ReivindicarRota(string partidaId, string jogadorId, string rotaId)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            if (!partida.PartidaIniciada)
            {
                throw new InvalidOperationException("Partida não foi iniciada");
            }

            Jogador jogadorAtual = partida.TurnoAtual.JogadorAtual;
            if (!jogadorAtual.Id.Equals(jogadorId))
            {
                throw new InvalidOperationException("Não é a vez deste jogador");
            }

            Jogador? jogador = partida.ObterJogador(jogadorId);
            if (jogador == null)
            {
                throw new ArgumentException("Jogador não encontrado");
            }

            Rota? rota = partida.Tabuleiro.GetRota(rotaId);
            if (rota == null)
            {
                throw new ArgumentException("Rota não encontrada");
            }

            if (!rota.Disponivel)
            {
                throw new InvalidOperationException("Rota já foi conquistada");
            }

            if (!jogador.TemCartasSuficientesParaConquistarRota(rota))
            {
                throw new InvalidOperationException("Jogador não tem cartas suficientes para esta rota");
            }

            if (!jogador.PodeReivindicarRota())
            {
                throw new InvalidOperationException("Jogador não tem peças de trem suficientes");
            }

            List<CartaVeiculo> cartasParaUsar = jogador.SelecionarCartasParaRota(rota);
            int pontos = jogador.ConquistarRota(rota, cartasParaUsar);

            if (pontos == 0)
            {
                throw new InvalidOperationException("Falha ao conquistar rota");
            }

            partida.TurnoAtual.ExecutarAcao(Acao.REIVINDICAR_ROTA);

            _partidaRepository.SalvarPartida(partida);
            return MapearTurnoParaDTO(partida.TurnoAtual);
        }

        public TurnoDTO ComprarBilhetesDestino(string partidaId, string jogadorId, List<string> bilhetesSelecionados)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            Jogador jogadorAtual = partida.TurnoAtual.JogadorAtual;
            if (!jogadorAtual.Id.Equals(jogadorId))
            {
                throw new InvalidOperationException("Não é a vez deste jogador");
            }

            Jogador? jogador = partida.ObterJogador(jogadorId);
            if (jogador == null)
            {
                throw new ArgumentException("Jogador não encontrado");
            }

            // Comprar 3 bilhetes do baralho
            List<BilheteDestino> bilhetesComprados = partida.BaralhoCartasDestino.ComprarBilhetes(3);

            if (bilhetesComprados.Count == 0)
            {
                throw new InvalidOperationException("Não há bilhetes disponíveis no baralho");
            }

            // O jogador deve manter pelo menos 1 bilhete
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
            partida.BaralhoCartasDestino.DevolverBilhetes(bilhetesParaDevolver);

            partida.TurnoAtual.ExecutarAcao(Acao.COMPRAR_BILHETES_DESTINO);

            _partidaRepository.SalvarPartida(partida);
            return MapearTurnoParaDTO(partida.TurnoAtual);
        }

        public TurnoDTO ProximoTurno(string partidaId)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            if (!partida.PartidaIniciada)
            {
                throw new InvalidOperationException("Partida não foi iniciada");
            }

            if (partida.TurnoAtual == null || !partida.TurnoAtual.AcaoCompletada)
            {
                throw new InvalidOperationException("Turno atual não foi completado");
            }

            Turno proximoTurno = partida.CriarProximoTurno();
            _partidaRepository.SalvarPartida(partida);

            return MapearTurnoParaDTO(proximoTurno);
        }

        public TurnoDTO PassarTurno(string partidaId, string jogadorId)
        {
            Partida? partida = _partidaRepository.ObterPartida(partidaId);
            if (partida == null)
            {
                throw new ArgumentException("Partida não encontrada");
            }

            if (!partida.PartidaIniciada)
            {
                throw new InvalidOperationException("Partida não foi iniciada");
            }

            if (partida.TurnoAtual == null || partida.TurnoAtual.JogadorAtual.Id != jogadorId)
            {
                throw new InvalidOperationException("Não é o turno do jogador");
            }

            // Marcar ação como completada e passar para o próximo turno
            partida.TurnoAtual.ExecutarAcao(Acao.COMPRAR_CARTAS_VEICULO); // Ação padrão para passar turno
            Turno proximoTurno = partida.CriarProximoTurno();
            _partidaRepository.SalvarPartida(partida);

            return MapearTurnoParaDTO(proximoTurno);
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