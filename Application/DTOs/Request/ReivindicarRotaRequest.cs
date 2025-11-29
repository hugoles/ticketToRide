namespace TicketToRide.Controllers
{
    public class ReivindicarRotaRequest
    {
        public string JogadorId { get; set; } = string.Empty;
        public string RotaId { get; set; } = string.Empty;

        public IEnumerable<int> CartasSelecionadas { get; set; } = [];
    }
}