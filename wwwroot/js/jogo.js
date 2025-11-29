// Ticket to Ride - Jogo Manager

class JogoManager {
    constructor(partidaId, jogadorId) {
        this.partidaId = partidaId;
        this.jogadorId = jogadorId;
        this.app = window.ticketToRideApp;
        this.estadoAtual = null;
        this.rotaSelecionada = null;
        this.cartasParaRotaSelecionada = [];
    }

    mostrarModalPrimeiroTurno(jogadorId) {
        const turno = this.estadoAtual.turnoAtual;

        const modal = document.getElementById("modal-primeiro-turno");
        const container = document.getElementById("modal-ticket-options");
        modal.style.display = "flex";

        container.innerHTML = "";
        this.bilhetesSelecionados = [];

        this.estadoAtual.opcoesBilheteDestino.forEach((b, index) => {
            const el = document.createElement("div");
            el.className = "card mb-2 ticket-option";
            el.dataset.index = index;
            el.innerHTML = `<div class="card-body p-2">
                            <div class="d-flex justify-content-between">
                                <strong>${b.origem} → ${b.destino}</strong>
                                <span class="badge bg-primary">${b.pontos} pts</span>
                            </div>
                        </div>`;
            el.addEventListener("click", () => this.toggleBilhete(index, el));
            container.appendChild(el);
        });

        const btn = document.getElementById("confirmar-modal-tickets");
        btn.onclick = async () => {
            if (this.bilhetesSelecionados.length < 2) {
                this.app.showNotification("Você deve selecionar pelo menos 2 bilhetes!", "warning");
                return;
            }

            try {
                await this.comprarBilhetes(true);
                modal.style.display = "none";
                this.app.primeiroTurnoFeito.add(turno.jogadorId);
            } catch (err) {
                console.error(err);
            }
        };
    }

    async atualizarEstado() {
        try {
            this.estadoAtual = await this.app.makeApiCall(`/api/partida/${this.partidaId}`);
            this.atualizarInterface();

            if (this.estadoAtual.turnoAtual) {
                const status = `Turno ${this.estadoAtual.turnoAtual.numero} - ${this.estadoAtual.turnoAtual.jogadorNome}`;
                console.log('Atualizando status da navbar:', status);
                this.app.updateGameStatus(status);
            } else {
                console.log('Atualizando status da navbar: Partida em andamento');
                this.app.updateGameStatus('Partida em andamento');
            }
        } catch (error) {
            console.error('Erro ao atualizar estado:', error);
            this.app.showNotification(`Erro ao atualizar estado: ${error.message}`, 'danger');
        }
    }

    async atualizarJogo() {
        await this.atualizarEstado();
    }

    atualizarInterface() {
        this.atualizarTurnoAtual();
        this.atualizarStatusJogadores();
        this.atualizarRotasDisponiveis();
        this.atualizarMinhasCartas();
        this.atualizarMeusBilhetes();
        this.atualizarMinhasRotas();
        this.atualizarCartasVisiveis();
        this.atualizarBilhetesVisiveis();

        const turno = this.estadoAtual.turnoAtual;
        if (turno && !this.app.primeiroTurnoFeito.has(turno.jogadorId)) {
            this.mostrarModalPrimeiroTurno(this.jogadorId);
        }
    }

    atualizarTurnoAtual() {
        const turnoElement = document.getElementById('current-turn');
        if (!this.estadoAtual.turnoAtual) return;

        const turno = this.estadoAtual.turnoAtual;

        turnoElement.innerHTML = `
            <div class="d-flex justify-content-between align-items-center">
                <div>
                    <strong>Turno ${turno.numero}</strong> - ${turno.jogadorNome}
                    ${turno.acaoRealizada ? `<br><small>Ação: ${this.getAcaoNome(turno.acaoRealizada)}</small>` : ''}
                </div>
                <div>
                    <span class="badge bg-success">Sua vez!</span>
                </div>
            </div>
        `;
    }

    atualizarStatusJogadores() {
        const statusElement = document.getElementById('players-status');
        statusElement.innerHTML = '';

        this.estadoAtual.jogadores.forEach(jogador => {
            const jogadorElement = document.createElement('div');
            jogadorElement.className = 'card mb-2';
            jogadorElement.innerHTML = `
                <div class="card-body p-2">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <strong>${jogador.nome}</strong>
                            ${jogador.id === this.jogadorId ? '<small class="text-muted">(Você)</small>' : ''}
                        </div>
                        <div class="text-end">
                            <div><strong>${jogador.pontuacao}</strong> pts</div>
                            <small class="text-muted">${jogador.pecasTremRestante} trens</small>
                        </div>
                    </div>
                    <div class="mt-1">
                        <small class="text-muted">
                            ${jogador.numeroRotas} rotas | ${jogador.numeroBilhetes} bilhetes | ${jogador.numeroCartas} cartas
                        </small>
                    </div>
                </div>
            `;
            statusElement.appendChild(jogadorElement);
        });
    }

    atualizarRotasDisponiveis() {
        const rotasElement = document.getElementById('available-routes');
        const rotasDisponiveis = this.estadoAtual.rotas.filter(r => r.estaDisponivel);

        if (rotasDisponiveis.length === 0) {
            rotasElement.innerHTML = '<div class="alert alert-info">Nenhuma rota disponível</div>';
            return;
        }

        let html = `
            <table class="table table-sm table-striped">
                <thead>
                    <tr>
                        <th>Rota</th>
                        <th>Cor</th>
                        <th>Tamanho</th>
                        <th>Pontos</th>
                        <th>Ação</th>
                    </tr>
                </thead>
                <tbody>
        `;

        rotasDisponiveis.forEach(rota => {
            const podeReivindicar = this.podeReivindicarRota(rota);
            html += `
                <tr>
                    <td>${rota.origem} → ${rota.destino}</td>
                    <td><span class="badge" style="background-color: ${this.getCorHex(rota.cor)}; color: ${this.getCorTextoConformeBackground(rota.cor)}">${this.getCorNome(rota.cor)}</span></td>
                    <td>${rota.tamanho}</td>
                    <td>${rota.pontos}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary"
                                onclick="selecionarRota('${rota.id}')"
                                ${!podeReivindicar ? 'disabled' : ''}>
                            ${podeReivindicar ? 'Reivindicar' : 'Sem cartas'}
                        </button>
                    </td>
                </tr>
            `;
        });

        html += '</tbody></table>';
        rotasElement.innerHTML = html;
    }

    atualizarMinhasCartas() {
        const cartasElement = document.getElementById('my-cards');
        if (!this.estadoAtual.turnoAtual) return;

        const jogadorAtual = this.estadoAtual.turnoAtual.jogadorId;
        const meuJogador = this.estadoAtual.jogadores.find(j => j.id === jogadorAtual);

        if (!meuJogador) return;

        cartasElement.innerHTML = '';

        this.cartasParaRotaSelecionadas = [];

        const cartasPorCor = {};
        meuJogador.maoCartas.forEach((carta, index) => {
            if (!cartasPorCor[carta.cor]) {
                cartasPorCor[carta.cor] = [];
            }
            cartasPorCor[carta.cor].push({ carta, originalIndex: index });
        });

        Object.keys(cartasPorCor).forEach(cor => {
            const cartasAgrupadas = cartasPorCor[cor];

            const grupoEl = document.createElement('div');
            grupoEl.className = 'card mb-2';

            const headerEl = document.createElement('div');
            headerEl.className = 'card-body p-2 pb-1';
            headerEl.innerHTML = `
            <div class="d-flex justify-content-between align-items-center">
                <span class="badge" style="background-color: ${this.getCorHex(cor)}; color: ${this.getCorTextoConformeBackground(cor)}">
                    ${this.getCorNome(cor)}
                </span>
                <span class="badge bg-secondary">${cartasAgrupadas.length}</span>
            </div>
        `;

            const bodyEl = document.createElement('div');
            bodyEl.className = 'card-body pt-1';

            const containerCartas = document.createElement('div');
            containerCartas.className = 'd-flex flex-wrap gap-1';

            cartasAgrupadas.forEach(({ carta, originalIndex }) => {
                const cartaEl = document.createElement('div');
                cartaEl.className = 'badge carta-mao-selecionavel';
                cartaEl.dataset.index = originalIndex;
                cartaEl.dataset.cor = carta.cor;

                cartaEl.style.backgroundColor = this.getCorHex(carta.cor);
                cartaEl.style.color = this.getCorTextoConformeBackground(carta.cor);
                cartaEl.style.cursor = 'pointer';

                cartaEl.textContent = this.getCorNome(carta.cor);

                if (this.cartasParaRotaSelecionadas.includes(originalIndex)) {
                    cartaEl.classList.add('carta-mao-selecionada');
                }

                cartaEl.addEventListener('click', () => this.toggleCartaDaMaoParaRota(originalIndex, cartaEl));

                containerCartas.appendChild(cartaEl);
            });

            bodyEl.appendChild(containerCartas);

            grupoEl.appendChild(headerEl);
            grupoEl.appendChild(bodyEl);
            cartasElement.appendChild(grupoEl);
        });
    }

    toggleCartaDaMaoParaRota(originalIndex, element) {
        const jaSelecionada = this.cartasParaRotaSelecionadas.includes(originalIndex);

        if (jaSelecionada) {
            this.cartasParaRotaSelecionadas =
                this.cartasParaRotaSelecionadas.filter(id => id !== originalIndex);
            element.classList.remove('carta-mao-selecionada');
        } else {
            this.cartasParaRotaSelecionadas.push(originalIndex);
            element.classList.add('carta-mao-selecionada');
        }
    }

    atualizarMeusBilhetes() {
        const bilhetesElement = document.getElementById('my-tickets');
        if (!this.estadoAtual.turnoAtual) return;

        const jogadorAtual = this.estadoAtual.turnoAtual.jogadorId;
        const meuJogador = this.estadoAtual.jogadores.find(j => j.id === jogadorAtual);

        if (!meuJogador) return;

        bilhetesElement.innerHTML = '';

        meuJogador.bilhetesDestino.forEach(bilhete => {
            const bilheteElement = document.createElement('div');
            bilheteElement.className = 'card mb-2';
            bilheteElement.innerHTML = `
                <div class="card-body p-2">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <strong>${bilhete.origem} → ${bilhete.destino}</strong>
                        </div>
                        <span class="badge ${bilhete.isCompleto ? 'bg-success' : 'bg-warning'}">
                            ${bilhete.pontos} pts
                        </span>
                    </div>
                </div>
            `;
            bilhetesElement.appendChild(bilheteElement);
        });
    }

    atualizarMinhasRotas() {
        const rotasElement = document.getElementById('my-routes');
        if (!this.estadoAtual.turnoAtual) return;

        const jogadorAtual = this.estadoAtual.turnoAtual.jogadorId;
        const meuJogador = this.estadoAtual.jogadores.find(j => j.id === jogadorAtual);

        if (!meuJogador) return;

        rotasElement.innerHTML = '';

        meuJogador.rotasConquistadas.forEach(rota => {
            const rotaElement = document.createElement('div');
            rotaElement.className = 'card mb-2';
            rotaElement.innerHTML = `
                 <div class="card-body p-2">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <strong>${rota.origem} → ${rota.destino}</strong>
                        </div>
                        <span">
                            ${rota.pontos} pts
                        </span>
                    </div>
                </div>
            `;
            rotasElement.appendChild(rotaElement);
        });
    }

    atualizarCartasVisiveis() {
        const container = document.getElementById("visible-cards");
        container.innerHTML = "";
        this.cartasVisiveisSelecionadas = [];

        if (!this.estadoAtual || !Array.isArray(this.estadoAtual.cartasVisiveis)) return;

        this.estadoAtual.cartasVisiveis.forEach((carta, index) => {
            const cardEl = document.createElement('div');
            cardEl.className = 'card mb-2 carta-visivel';
            cardEl.dataset.index = index;

            cardEl.innerHTML = `<div class="card-body p-2">
                                    <div class="d-flex justify-content-between align-items-center">
                                        <span class="badge" style="background-color: ${this.getCorHex(carta.cor)}; color: ${this.getCorTextoConformeBackground(carta.cor)}">${this.getCorNome(carta.cor)}</span>
                                     </div>
                                </div>`;

            cardEl.addEventListener("click", () => this.toggleCartaVisivel(index, cardEl));

            container.appendChild(cardEl);
        });
    }

    atualizarBilhetesVisiveis() {
        const container = document.getElementById("ticket-options");
        if (!container) return;

        container.innerHTML = "";

        this.bilhetesSelecionados = [];
    }

    toggleCartaVisivel(index, element) {
        const selecionada = this.cartasVisiveisSelecionadas.includes(index);

        if (selecionada) {
            this.cartasVisiveisSelecionadas =
                this.cartasVisiveisSelecionadas.filter(i => i !== index);
            element.classList.remove("carta-selecionada");
        } else {
            this.cartasVisiveisSelecionadas.push(index);
            element.classList.add("carta-selecionada");
        }
    }

    podeReivindicarRota(rota) {
        if (!this.estadoAtual.turnoAtual) return false;

        const jogadorAtual = this.estadoAtual.turnoAtual.jogadorId;
        const meuJogador = this.estadoAtual.jogadores.find(j => j.id === jogadorAtual);
        if (!meuJogador) return false;

        if (rota.cor === "CINZA") {
            const locomotivas = meuJogador.maoCartas.filter(c => c.cor === "LOCOMOTIVA").length;
            const cartasPorCor = {};

            meuJogador.maoCartas
                .filter(c => c.cor !== "LOCOMOTIVA")
                .forEach(c => cartasPorCor[c.cor] = (cartasPorCor[c.cor] || 0) + 1);

            const melhorCor = Math.max(0, ...Object.values(cartasPorCor));
            return (melhorCor + locomotivas) >= rota.tamanho;
        }

        const cartasCor = meuJogador.maoCartas.filter(c =>
            c.cor === rota.cor || c.cor === "LOCOMOTIVA"
        );

        return cartasCor.length >= rota.tamanho;
    }

    async comprarCartas() {
        if (!this.estadoAtual.turnoAtual) {
            this.app.showNotification('Nenhum turno ativo!', 'warning');
            return;
        }

        const jogadorAtual = this.estadoAtual.turnoAtual.jogadorId;
        try {
            await this.app.makeApiCall(`/api/turno/partida/${this.partidaId}/turno/comprar-cartas`, 'POST', { jogadorId: jogadorAtual, indices: this.cartasVisiveisSelecionadas });
            await this.atualizarEstado();
            this.app.showNotification('Cartas compradas com sucesso!', 'success');
        } catch (error) {
            console.error('Erro ao comprar cartas:', error);
            this.app.showNotification(`Erro ao comprar cartas: ${error.message}`, 'danger');
        }
    }

    selecionarRota(rotaId) {
        this.rotaSelecionada = rotaId;
        // Por simplicidade, vamos reivindicar a rota diretamente
        this.reivindicarRota();
    }

    async reivindicarRota() {
        if (!this.estadoAtual.turnoAtual) {
            this.app.showNotification('Nenhum turno ativo!', 'warning');
            return;
        }

        if (!this.rotaSelecionada) {
            const rotasDisponiveis = this.estadoAtual.rotas.filter(r => r.estaDisponivel);
            if (rotasDisponiveis.length === 0) {
                this.app.showNotification('Não há rotas disponíveis', 'warning');
                return;
            }
            this.rotaSelecionada = rotasDisponiveis[0].id;
        }

        if (!this.cartasParaRotaSelecionadas || this.cartasParaRotaSelecionadas.length === 0) {
            this.app.showNotification('Selecione as cartas que deseja usar para reivindicar a rota.', 'warning');
            return;
        }

        const jogadorAtual = this.estadoAtual.turnoAtual.jogadorId;

        try {
            await this.app.makeApiCall(
                `/api/turno/partida/${this.partidaId}/turno/reivindicar-rota`,
                'POST',
                {
                    jogadorId: jogadorAtual,
                    rotaId: this.rotaSelecionada,
                    cartasSelecionadas: this.cartasParaRotaSelecionadas
                }
            );

            this.rotaSelecionada = null;
            this.cartasParaRotaSelecionadas = [];
            await this.atualizarEstado();
            this.app.showNotification('Rota reivindicada com sucesso!', 'success');
        } catch (error) {
            console.error('Erro ao reivindicar rota:', error);
            this.app.showNotification(`Erro ao reivindicar rota: ${error.message}`, 'danger');
        }
    }

    async comprarBilhetes(primeiroTurno) {
        if (!this.estadoAtual.turnoAtual) {
            this.app.showNotification('Nenhum turno ativo!', 'warning');
            return;
        }

        const jogadorAtual = this.estadoAtual.turnoAtual.jogadorId;
        const selecionados = [...document.querySelectorAll(".ticket-option.selected-ticket")];
        if (selecionados.length === 0) {
            this.app.showNotification("Selecione pelo menos 1 bilhete!", "warning");
            return;
        }

        try {
            await this.app.makeApiCall(`/api/turno/partida/${this.partidaId}/turno/comprar-bilhetes`, 'POST', {
                jogadorId: jogadorAtual,
                bilhetesSelecionados: selecionados.map(el => parseInt(el.dataset.index)),
                primeiroTurno: primeiroTurno
            });

            await this.atualizarEstado();
            this.app.showNotification('Bilhetes comprados com sucesso!', 'success');
        } catch (error) {
            console.error('Erro ao comprar bilhetes:', error);
            this.app.showNotification(`Erro ao comprar bilhetes: ${error.message}`, 'danger');
        }
    }

    mostrarBilhetesParaEscolha() {
        const container = document.getElementById("ticket-options");
        container.innerHTML = "";
        this.bilhetesSelecionados = [];

        this.estadoAtual.opcoesBilheteDestino.forEach((b, index) => {
            const el = document.createElement("div");
            el.className = "card mb-2 ticket-option";
            el.dataset.index = index;

            el.innerHTML = `
            <div class="card-body p-2">
                <div class="d-flex justify-content-between">
                    <strong>${b.origem} → ${b.destino}</strong>
                    <span class="badge bg-primary">${b.pontos} pts</span>
                </div>
            </div>
        `;

            el.addEventListener("click", () => this.toggleBilhete(index, el));

            container.appendChild(el);
        });

        const btn = document.createElement("button");
        btn.className = "btn btn-success mt-2";
        btn.textContent = "Confirmar Seleção";
        btn.addEventListener("click", () => {
            if (typeof this.comprarBilhetes === "function") {
                this.comprarBilhetes();
            } else if (typeof window.comprarBilhetes === "function") {
                window.comprarBilhetes();
            } else {
                console.warn("comprarBilhetes não encontrada");
            }
        });

        container.appendChild(btn);
    }

    toggleBilhete(index, element) {
        const selecionado = this.bilhetesSelecionados.includes(index);

        if (selecionado) {
            this.bilhetesSelecionados =
                this.bilhetesSelecionados.filter(i => i !== index);
            element.classList.remove("selected-ticket");
        } else {
            this.bilhetesSelecionados.push(index);
            element.classList.add("selected-ticket");
        }
    }

    async finalizarPartida() {
        try {
            const partida = await this.app.makeApiCall(`/api/partida/${this.partidaId}/finalizar`, 'POST');
            this.mostrarResultado(partida);
        } catch (error) {
            console.error('Erro ao finalizar partida:', error);
            this.app.showNotification(`Erro ao finalizar partida: ${error.message}`, 'danger');
        }
    }

    mostrarResultado(partida) {
        const ranking = partida.jogadores.sort((a, b) => b.pontuacao - a.pontuacao);

        const resultadoElement = document.getElementById('final-ranking');
        let html = '<h4>Ranking Final</h4><div class="list-group">';

        ranking.forEach((jogador, index) => {
            const posicao = index + 1;
            const medalha = posicao === 1 ? '🥇' : posicao === 2 ? '🥈' : posicao === 3 ? '🥉' : '';

            html += `
                <div class="list-group-item d-flex justify-content-between align-items-center">
                    <div>
                        <strong>${posicao}º ${medalha} ${jogador.nome}</strong>
                        ${jogador.id === this.jogadorId ? '<small class="text-muted">(Você)</small>' : ''}
                    </div>
                    <span class="badge bg-primary rounded-pill">${jogador.pontuacao} pontos</span>
                </div>
            `;
        });

        html += '</div>';
        resultadoElement.innerHTML = html;

        this.app.showScreen('result-screen');
        this.app.updateGameStatus('Partida finalizada');
    }

    getAcaoNome(acao) {
        const acoes = {
            'REIVINDICAR_ROTA': 'Reivindicar Rota',
            'COMPRAR_BILHETES_DESTINO': 'Comprar Bilhetes',
            'COMPRAR_CARTAS_VEICULO': 'Comprar Cartas'
        };
        return acoes[acao] || acao;
    }

    getCorHex(cor) {
        const cores = {
            'VERMELHO': '#dc3545',
            'AZUL': '#0d6efd',
            'VERDE': '#198754',
            'AMARELO': '#ffc107',
            'PRETO': '#212529',
            'BRANCO': '#f8f9fa',
            'LARANJA': '#fd7e14',
            'ROSA': '#e83e8c',
            'CINZA': '#6c757d',
            'LOCOMOTIVA': '#6f42c1'
        };
        return cores[cor] || '#6c757d';
    }

    getCorNome(corNumero) {
        const cores = {
            'VERMELHO': 'Vermelho',
            'AZUL': 'Azul',
            'VERDE': 'Verde',
            'AMARELO': 'Amarelo',
            'PRETO': 'Preto',
            'BRANCO': 'Branco',
            'LARANJA': 'Laranja',
            'ROSA': 'Rosa',
            'CINZA': 'Cinza',
            'LOCOMOTIVA': 'Locomotiva'
        };
        return cores[corNumero] || 'Desconhecida';
    }

    getCorTextoConformeBackground(backgroundColor) {
        const cores = {
            'AMARELO': '#212529',
            'BRANCO': '#212529',
        };
        return cores[backgroundColor] || "#f8f9fa";
    }
}

function atualizarEstado() {
    if (window.jogoManager) {
        window.jogoManager.atualizarEstado();
    }
}

function atualizarJogo() {
    if (window.jogoManager) {
        window.jogoManager.atualizarJogo();
    }
}

function finalizarPartida() {
    if (window.jogoManager) {
        window.jogoManager.finalizarPartida();
    }
}

function comprarCartas() {
    if (window.jogoManager) {
        window.jogoManager.comprarCartas();
    }
}

function reivindicarRota() {
    if (window.jogoManager) {
        window.jogoManager.reivindicarRota();
    }
}

function comprarBilhetes() {
    if (window.jogoManager) {
        window.jogoManager.comprarBilhetes();
    }
}

function mostrarBilhetesParaEscolha() {
    if (window.jogoManager) {
        window.jogoManager.mostrarBilhetesParaEscolha();
    }
}

function selecionarRota(rotaId) {
    if (window.jogoManager) {
        window.jogoManager.selecionarRota(rotaId);
    }
}