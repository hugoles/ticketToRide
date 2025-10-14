namespace TicketToRide.Domain.Entities
{
    public abstract class Carta
    {
        public string Nome { get; set; }

        protected Carta(string nome)
        {
            Nome = nome;
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}