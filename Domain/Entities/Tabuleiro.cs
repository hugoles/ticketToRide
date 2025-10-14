namespace TicketToRide.Domain.Entities
{
    public class Tabuleiro
    {
        public List<Rota> Rotas { get; set; } = [];

        public Tabuleiro()
        {
        }

        public void AdicionarRotas(List<Rota> rotas)
        {
            Rotas.AddRange(rotas);
        }

        public Rota? GetRota(string idRota)
        {
            return Rotas.FirstOrDefault(r => r.Id == idRota);
        }

        public List<Rota> GetRotasDisponiveis()
        {
            return Rotas.Where(r => r.Disponivel).ToList();
        }

        public List<Rota> GetRotasConquistadas()
        {
            return Rotas.Where(r => !r.Disponivel).ToList();
        }
    }
}