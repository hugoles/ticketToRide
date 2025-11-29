using TicketToRide.Domain.Enums;

namespace TicketToRide.Domain.Entities
{
    public class CartaVeiculo : Carta
    {
        public Cor Cor { get; }

        public CartaVeiculo(Cor cor) : base(cor.GetEnumDescription())
        {
            Cor = cor;
        }

        public bool PodeSerUsadaPara(Cor corRota)
        {
            return Cor == Cor.LOCOMOTIVA || Cor == corRota || corRota == Cor.CINZA;
        }
    }
}