using TicketToRide.Domain.Enums;

namespace TicketToRide.Domain.Entities
{
    public class BaralhoCartasVeiculo : Baralho<CartaVeiculo>
    {
        public BaralhoCartasVeiculo()
        {
            InicializarBaralho([]);
        }

        public override void InicializarBaralho(List<CartaVeiculo> cartas)
        {
            Cor[] cores = [Cor.VERMELHO, Cor.AZUL, Cor.VERDE, Cor.AMARELO, Cor.PRETO, Cor.BRANCO, Cor.LARANJA, Cor.ROSA];

            foreach (Cor cor in cores)
            {
                for (int i = 0; i < 12; i++)
                {
                    MonteCompra.Add(new CartaVeiculo(cor));
                }
            }

            for (int i = 0; i < 14; i++)
            {
                MonteCompra.Add(new CartaVeiculo(Cor.LOCOMOTIVA));
            }

            Embaralhar();
        }

        public CartaVeiculo? ComprarCartaVisivel(int indice)
        {
            if (indice < 0 || indice >= MonteCompra.Count)
            {
                return null;
            }

            CartaVeiculo carta = MonteCompra[indice];
            MonteCompra.RemoveAt(indice);
            return carta;
        }
    }
}