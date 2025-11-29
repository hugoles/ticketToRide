using TicketToRide.Domain.Enums;
using TicketToRide.Domain.Interfaces;
using TicketToRideAPI.Domain.Interfaces;

namespace TicketToRide.Domain.Entities
{
    public class Jogador : IJogadorSubject
    {
        private const int QuantidadeInicialPecasTrem = 45;
        private List<CartaVeiculo> maoCartas = [];
        private List<BilheteDestino> bilhetesDestino = [];

        public string Id { get; }
        public string Nome { get; }
        public int Pontuacao { get; private set; } = 0;
        public int PecasTremRestante { get; private set; } = QuantidadeInicialPecasTrem;

        public IReadOnlyList<CartaVeiculo> MaoCartas => maoCartas.AsReadOnly();
        public IReadOnlyList<BilheteDestino> BilhetesDestino => bilhetesDestino.AsReadOnly();
        public List<Rota> RotasConquistadas { get; } = [];

        private List<IObserver> _observers = [];

        public Jogador(string id, string nome)
        {
            Id = id;
            Nome = nome;
        }

        public void ConquistarRota(Rota rota, IEnumerable<int> cartasSelecionadas)
        {
            RemoverCartasDaMao(ObterCartasDaMao(cartasSelecionadas));

            RotasConquistadas.Add(rota);

            RemoverPecasTrem(rota.Tamanho);

            rota.ConquistarRota();

            Pontuacao += rota.CalcularPontos();

            Notify();
        }

        private void RemoverCartasDaMao(List<CartaVeiculo> cartasUsadas)
        {
            foreach (CartaVeiculo carta in cartasUsadas)
            {
                maoCartas.Remove(carta);
            }
        }

        private void RemoverPecasTrem(int quantidade)
        {
            PecasTremRestante = Math.Max(0, PecasTremRestante - quantidade);
        }

        public bool TemPecasSuficientesParaConquistarRota(Rota rota)
        {
            return PecasTremRestante >= rota.Tamanho;
        }

        public bool TemCartasSuficientesParaConquistarRota(Rota rota, IEnumerable<int> cartasSelecionadas)
        {
            if (cartasSelecionadas.Count() != rota.Tamanho)
            {
                return false;
            }

            List<CartaVeiculo> cartas = ObterCartasDaMao(cartasSelecionadas);

            if (rota.Cor == Cor.CINZA)
            {
                IEnumerable<Cor> coresUsadas = cartas
                    .Where(c => c.Cor != Cor.LOCOMOTIVA)
                    .Select(c => c.Cor)
                    .Distinct();

                return coresUsadas.Count() <= 1;
            }

            return cartas.All(c => rota.PodeSerConquistadaCom(c));
        }

        public int CalcularPontuacaoTotal()
        {
            int pontosRotas = RotasConquistadas.Sum(r => r.CalcularPontos());
            int pontosBilhetes = BilhetesDestino.Where(b => b.EstaCompleto()).Sum(b => b.Pontos);
            return pontosRotas + pontosBilhetes;
        }

        public int CalcularComprimentoRotaContinua()
        {
            return RotasConquistadas.Sum(r => r.Tamanho);
        }

        public void AdicionarCartasVeiculo(IEnumerable<CartaVeiculo> cartas)
        {
            maoCartas.AddRange(cartas);
        }

        public void AdicionarBilhetesDestino(IEnumerable<BilheteDestino> bilhetes)
        {
            bilhetesDestino.AddRange(bilhetes);
        }

        private List<CartaVeiculo> ObterCartasDaMao(IEnumerable<int> cartasSelecionadas)
        {
            return [.. MaoCartas.Where((c, i) => cartasSelecionadas.Contains(i))];
        }

        public void AdicionarPontuacao(int pontos)
        {
            Pontuacao += pontos;
        }

        public void Attach(IObserver observer)
        {
            Console.WriteLine($"Subject (Jogador {Nome}): Attached an observer.");
            this._observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            this._observers.Remove(observer);
            Console.WriteLine($"Subject (Jogador {Nome}): Detached an observer.");
        }

        public void Notify()
        {
            foreach (IObserver observer in _observers)
            {
                observer.Update(this);
            }
        }
    }
}