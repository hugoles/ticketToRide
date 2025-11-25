namespace TicketToRide.Domain.Entities
{
    public abstract class Baralho<T> where T : Carta
    {
        private List<T> MonteCompra { get; } = [];
        private List<T> MonteDescarte { get; } = [];

        private bool TemCarta()
        {
            return MonteCompra.Count > 0;
        }

        protected void InicializarMonteCompra(IEnumerable<T> cartas)
        {
            AdicionarCartasAoMonteCompra(cartas);
            EmbaralharCartas();
        }

        protected void AdicionarCartasAoMonteCompra(IEnumerable<T> cartas)
        {
            MonteCompra.AddRange(cartas);
        }

        protected T? ComprarCartaPorIndice(int indice)
        {
            if (indice < 0 || indice >= MonteCompra.Count)
            {
                return null;
            }

            T carta = MonteCompra[indice];
            MonteCompra.RemoveAt(indice);
            return carta;
        }

        protected T? ObterCartaPorIndice(int indice)
        {
            if (indice < 0 || indice >= MonteCompra.Count)
            {
                return null;
            }

            return MonteCompra[indice];
        }

        public List<T> Comprar(int quantidade)
        {
            List<T> cartas = [];
            for (int i = 0; i < quantidade; i++)
            {
                T? carta = Comprar();
                if (carta != null)
                {
                    cartas.Add(carta);
                }
                else
                {
                    break;
                }
            }
            return cartas;
        }

        private T? Comprar()
        {
            if (!TemCarta())
            {
                if (MonteDescarte.Count > 0)
                {
                    Embaralhar();
                }
                else
                {
                    return null;
                }
            }

            T carta = MonteCompra[0];
            MonteCompra.RemoveAt(0);
            return carta;
        }

        public void Descartar(IEnumerable<T> carta)
        {
            MonteDescarte.AddRange(carta);
        }

        protected void Embaralhar()
        {
            RenovarMonteCompra();
            EmbaralharCartas();
        }

        private void RenovarMonteCompra()
        {
            MonteCompra.AddRange(MonteDescarte);
            MonteDescarte.Clear();
        }

        private void EmbaralharCartas()
        {
            Random random = new();
            for (int i = MonteCompra.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (MonteCompra[i], MonteCompra[j]) = (MonteCompra[j], MonteCompra[i]);
            }
        }

        protected int ObterTamanhoMonte()
        {
            return MonteCompra.Count;
        }
    }
}