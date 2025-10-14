using TicketToRide.Domain.Enums;
using TicketToRideAPI.Domain;

namespace TicketToRide.Domain.Entities
{
    public class CartaVeiculo : Carta
    {
        public Cor Cor { get; set; }

        public CartaVeiculo(Cor cor) : base(cor.GetEnumDescription())
        {
            Cor = cor;
        }

        public bool PodeSerUsadaPara(Cor corRota)
        {
            return corRota == Cor.LOCOMOTIVA || Cor == corRota;
        }

        public override string ToString()
        {
            return Cor.GetEnumDescription();
        }
    }
}