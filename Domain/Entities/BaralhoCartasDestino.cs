namespace TicketToRide.Domain.Entities
{
    public class BaralhoCartasDestino : Baralho<BilheteDestino>
    {
        public BaralhoCartasDestino()
        {
            InicializarBaralho([]);
        }

        public override void InicializarBaralho(List<BilheteDestino> bilhetes)
        {
            MonteCompra.AddRange(bilhetes);
            Embaralhar();
        }

        public List<BilheteDestino> ComprarBilhetes(int quantidade = 3)
        {
            return ComprarVarias(quantidade);
        }

        public void DevolverBilhetes(List<BilheteDestino> bilhetes)
        {
            foreach (BilheteDestino bilhete in bilhetes)
            {
                Descartar(bilhete);
            }
        }
    }
}