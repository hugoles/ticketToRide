using TicketToRideAPI.Application.DTOs;

namespace TicketToRide.Application.DTOs
{
    public class JogadorDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int Pontuacao { get; set; }
        public int PecasTremRestante { get; set; }
        public List<CartaVeiculoDTO> MaoCartas { get; set; } = [];
        public List<BilheteDestinoDTO> BilhetesDestino { get; set; } = [];
        public List<RotaDTO> RotasConquistadas { get; set; } = [];
        public int NumeroCartas { get; set; }
        public int NumeroBilhetes { get; set; }
        public int NumeroRotas { get; set; }
    }
}
