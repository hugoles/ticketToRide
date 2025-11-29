using TicketToRide.Application.DTOs;

namespace TicketToRide.Application.Services.Interfaces
{
    public interface ITurnoService
    {
        public TurnoDTO? ObterTurnoAtual(string partidaId);

        public TurnoDTO ComprarCartasVeiculo(string partidaId, string jogadorId, List<int> indicesCartasReveladas = null);

        public TurnoDTO ReivindicarRota(string partidaId, string jogadorId, string rotaId, IEnumerable<int> cartasSelecionadas = null);

        public TurnoDTO ComprarBilhetesDestino(string partidaId, string jogadorId, List<int> bilhetesSelecionados, bool primeiroTurno);
    }
}