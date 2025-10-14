using TicketToRide.Domain.Enums;

namespace TicketToRide.Domain.Entities
{
    public class Rota
    {
        public string Id { get; set; }
        public Cidade Origem { get; set; }
        public Cidade Destino { get; set; }
        public Cor Cor { get; set; }
        public int Tamanho { get; set; }
        public bool EhDupla { get; set; }
        public bool Disponivel { get; set; } = true;

        public Rota(string id, Cidade origem, Cidade destino, Cor cor, int tamanho, bool ehDupla = false)
        {
            Id = id;
            Origem = origem;
            Destino = destino;
            Cor = cor;
            Tamanho = tamanho;
            EhDupla = ehDupla;
        }

        public int CalcularPontos()
        {
            return Tamanho switch
            {
                1 => 1,
                2 => 2,
                3 => 4,
                4 => 7,
                5 => 10,
                6 => 15,
                _ => 0
            };
        }

        public bool PodeSerConquistadaPor(Jogador jogador)
        {
            if (!Disponivel)
            {
                return false;
            }

            List<CartaVeiculo> cartasNecessarias = jogador.ObterCartasParaCor(Cor);
            return cartasNecessarias.Count >= Tamanho;
        }
    }
}