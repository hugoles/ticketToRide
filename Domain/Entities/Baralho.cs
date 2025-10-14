namespace TicketToRide.Domain.Entities
{
    public abstract class Baralho<T> where T : Carta
    {
        protected List<T> MonteCompra { get; set; } = [];
        protected List<T> MonteDescarte { get; set; } = [];

        public abstract void InicializarBaralho(List<T> cartas);

        public bool TemCarta()
        {
            return MonteCompra.Count > 0;
        }

        public List<T> ComprarVarias(int quantidade)
        {
            List<T> cartas = new();
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

        public T? Comprar()
        {
            if (MonteCompra.Count == 0)
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

        public void Descartar(T carta)
        {
            MonteDescarte.Add(carta);
        }

        public void Embaralhar()
        {
            MonteCompra.AddRange(MonteDescarte);
            MonteDescarte.Clear();

            Random random = new();
            for (int i = MonteCompra.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (MonteCompra[i], MonteCompra[j]) = (MonteCompra[j], MonteCompra[i]);
            }
        }
    }
}