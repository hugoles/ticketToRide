namespace TicketToRide.Domain.Entities
{
    public class Cidade
    {
        public string Nome { get; set; }

        public Cidade(string nome)
        {
            Nome = nome;
        }

        public override string ToString()
        {
            return Nome;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Cidade outraCidade)
            {
                return Nome.Equals(outraCidade.Nome, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}