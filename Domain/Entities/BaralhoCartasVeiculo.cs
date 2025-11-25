namespace TicketToRide.Domain.Entities
{
    public class BaralhoCartasVeiculo : Baralho<CartaVeiculo>
    {
        public BaralhoCartasVeiculo(IEnumerable<CartaVeiculo> cartas)
        {
            InicializarMonteCompra(cartas);
        }

        public CartaVeiculo? ComprarCartaRevelada(int indice)
        {
            if (indice < 0 || indice >= ObterTamanhoMonte())
            {
                return null;
            }

            return ComprarCartaPorIndice(indice);
        }

        public IEnumerable<CartaVeiculo> ListarCartasReveladas()
        {
            List<CartaVeiculo> cartasReveladas = [];
            for (int i = 0; i < 5; i++)
            {
                cartasReveladas.Add(ObterCartaPorIndice(i));
            }

            return cartasReveladas;
        }
    }
}