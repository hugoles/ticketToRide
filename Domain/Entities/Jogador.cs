using TicketToRide.Domain.Enums;

namespace TicketToRide.Domain.Entities
{
    public class Jogador
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Pontuacao { get; set; } = 0;
        public int PecasTremRestante { get; set; } = 45;
        public List<CartaVeiculo> MaoCartas { get; private set; } = [];
        public List<BilheteDestino> BilhetesDestino { get; set; } = [];
        public List<Rota> RotasConquistadas { get; set; } = [];

        public Jogador(string id, string nome)
        {
            Id = id;
            Nome = nome;
        }

        public int ConquistarRota(Rota rota, List<CartaVeiculo> cartasUsadas)
        {
            if (!rota.PodeSerConquistadaPor(this))
            {
                return 0;
            }

            if (!TemCartasParaRota(rota, cartasUsadas))
            {
                return 0;
            }

            foreach (CartaVeiculo carta in cartasUsadas)
            {
                MaoCartas.Remove(carta);
            }

            RotasConquistadas.Add(rota);

            RemoverPecasTrem(rota.Tamanho);

            int pontos = rota.CalcularPontos();

            rota.Disponivel = false;

            Pontuacao += pontos;

            return pontos;
        }

        private bool TemCartasParaRota(Rota rota, List<CartaVeiculo> cartasUsadas)
        {
            if (cartasUsadas.Count != rota.Tamanho)
            {
                return false;
            }

            if (cartasUsadas.Any(x => !MaoCartas.Contains(x)))
            {
                return false;
            }

            int cartasValidas = cartasUsadas.Count(c => c.PodeSerUsadaPara(rota.Cor));
            return cartasValidas >= rota.Tamanho;
        }

        public int CalcularComprimentoRotaContinua()
        {
            // Implementar algoritmo para encontrar a rota contínua mais longa
            // Por simplicidade, retornar a soma de todas as rotas por enquanto
            return RotasConquistadas.Sum(r => r.Tamanho);
        }

        public void RemoverPecasTrem(int quantidade)
        {
            PecasTremRestante = Math.Max(0, PecasTremRestante - quantidade);
        }

        public List<CartaVeiculo> ComprarCartas(List<CartaVeiculo> cartas)
        {
            MaoCartas.AddRange(cartas);
            return cartas;
        }

        public List<BilheteDestino> ComprarCartas(List<BilheteDestino> cartas)
        {
            BilhetesDestino.AddRange(cartas);
            return cartas;
        }

        public int CalcularPontosBilhetes()
        {
            int pontos = 0;
            foreach (BilheteDestino bilhete in BilhetesDestino)
            {
                if (bilhete.IsCompleto(RotasConquistadas))
                {
                    pontos += bilhete.Pontos;
                }
                else
                {
                    pontos -= bilhete.Pontos;
                }
            }
            return pontos;
        }

        public void AtualizarPontuacao()
        {
            int pontosBilhetes = CalcularPontosBilhetes();

            Pontuacao += pontosBilhetes;
        }

        public List<CartaVeiculo> ObterCartasParaCor(Cor cor)
        {
            return [.. MaoCartas.Where(c => c.PodeSerUsadaPara(cor))];
        }

        public bool TemCartasSuficientesParaConquistarRota(Rota rota)
        {
            List<CartaVeiculo> cartasDisponiveis = ObterCartasParaCor(rota.Cor);
            return cartasDisponiveis.Count >= rota.Tamanho;
        }

        public List<CartaVeiculo> SelecionarCartasParaRota(Rota rota)
        {
            List<CartaVeiculo> cartasSelecionadas = new();
            List<CartaVeiculo> cartasDisponiveis = ObterCartasParaCor(rota.Cor);

            List<CartaVeiculo> locomotivas = [.. cartasDisponiveis.Where(c => c.Cor == Cor.LOCOMOTIVA)];
            List<CartaVeiculo> cartasNormais = [.. cartasDisponiveis.Except(locomotivas)];

            for (int i = 0; i < Math.Min(rota.Tamanho, cartasNormais.Count); i++)
            {
                cartasSelecionadas.Add(cartasNormais[i]);
            }

            int cartasFaltando = rota.Tamanho - cartasSelecionadas.Count;
            for (int i = 0; i < Math.Min(cartasFaltando, locomotivas.Count); i++)
            {
                cartasSelecionadas.Add(locomotivas[i]);
            }

            return cartasSelecionadas;
        }

        public void AdicionarBilhetesDestino(List<BilheteDestino> bilhetes)
        {
            BilhetesDestino.AddRange(bilhetes);
        }

        public bool PodeComprarCartas()
        {
            return MaoCartas.Count < 10;
        }

        public bool PodeReivindicarRota()
        {
            return PecasTremRestante > 0;
        }
    }
}