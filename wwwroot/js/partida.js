// Ticket to Ride - Partida Manager

class PartidaManager {
    constructor() {
        this.partidaId = null;
        this.jogadores = [];
        this.app = window.ticketToRideApp;
    }

    async adicionarJogador() {
        const nomeInput = document.getElementById('player-name');
        const nome = nomeInput.value.trim();

        if (!nome) {
            this.app.showNotification('Por favor, digite um nome para o jogador', 'warning');
            return;
        }

        if (this.jogadores.some(j => j.nome.toLowerCase() === nome.toLowerCase())) {
            this.app.showNotification('Já existe um jogador com este nome', 'warning');
            return;
        }

        if (this.jogadores.length >= 5) {
            this.app.showNotification('Número máximo de jogadores atingido (5)', 'warning');
            return;
        }

        try {
            if (!this.partidaId) {
                await this.criarPartida();
            }

            const jogador = await this.app.makeApiCall(`/api/jogador/partida/${this.partidaId}/jogador`, 'POST', { nome: nome });
            this.jogadores.push(jogador);

            nomeInput.value = '';
            this.atualizarListaJogadores();
            this.verificarPodeIniciar();

            this.app.showNotification(`Jogador ${nome} adicionado com sucesso!`, 'success');
        } catch (error) {
            console.error('Erro ao adicionar jogador:', error);
            this.app.showNotification(`Erro ao adicionar jogador: ${error.message}`, 'danger');
        }
    }

    async criarPartida() {
        try {
            const partida = await this.app.makeApiCall('/api/partida/criar', 'POST');
            this.partidaId = partida.id;
            this.app.currentPartidaId = this.partidaId;

            console.log('Partida criada:', this.partidaId);
        } catch (error) {
            console.error('Erro ao criar partida:', error);
            this.app.showNotification(`Erro ao criar partida: ${error.message}`, 'danger');
            throw error;
        }
    }

    async iniciarPartida() {
        if (this.jogadores.length < 2) {
            this.app.showNotification('É necessário pelo menos 2 jogadores para iniciar', 'warning');
            return;
        }

        if (this.jogadores.length > 5) {
            this.app.showNotification('Número máximo de jogadores é 5', 'warning');
            return;
        }

        try {
            const partida = await this.app.makeApiCall(`/api/partida/${this.partidaId}/iniciar`, 'POST', { numJogadores: this.jogadores.length });

            this.app.currentJogadorId = partida.turnoAtual.jogadorId;

            window.jogoManager = new JogoManager(this.partidaId, this.app.currentJogadorId);

            this.app.showScreen('game-screen');

            await window.jogoManager.atualizarEstado();

            this.app.showNotification('Partida iniciada com sucesso!', 'success');
        } catch (error) {
            console.error('Erro ao iniciar partida:', error);
            this.app.showNotification(`Erro ao iniciar partida: ${error.message}`, 'danger');
        }
    }

    atualizarListaJogadores() {
        const listaElement = document.getElementById('players-list');
        listaElement.innerHTML = '';

        this.jogadores.forEach((jogador, index) => {
            const jogadorElement = document.createElement('div');
            jogadorElement.className = 'alert alert-info d-flex justify-content-between align-items-center';
            jogadorElement.innerHTML = `
                <span><i class="fas fa-user"></i> ${jogador.nome}</span>
                <button class="btn btn-sm btn-outline-danger" onclick="removerJogador(${index})">
                    <i class="fas fa-times"></i>
                </button>
            `;
            listaElement.appendChild(jogadorElement);
        });
    }

    verificarPodeIniciar() {
        const startBtn = document.getElementById('start-game-btn');
        const podeIniciar = this.jogadores.length >= 2 && this.jogadores.length <= 5;
        startBtn.disabled = !podeIniciar;
    }

    async removerJogador(index) {
        const jogador = this.jogadores[index];

        try {
            await this.app.makeApiCall(`/api/jogador/partida/${this.partidaId}/jogador/${jogador.id}`, 'DELETE');

            this.jogadores.splice(index, 1);
            this.atualizarListaJogadores();
            this.verificarPodeIniciar();

            this.app.showNotification(`Jogador ${jogador.nome} removido`, 'info');
        } catch (error) {
            console.error('Erro ao remover jogador:', error);
            this.app.showNotification(`Erro ao remover jogador: ${error.message}`, 'danger');
        }
    }

    novaPartida() {
        this.partidaId = null;
        this.jogadores = [];
        this.app.currentPartidaId = null;
        this.app.currentJogadorId = null;

        document.getElementById('player-name').value = '';
        this.atualizarListaJogadores();
        this.verificarPodeIniciar();

        this.app.showScreen('setup-screen');
        this.app.updateGameStatus('Aguardando configuração');

        this.app.showNotification('Nova partida iniciada', 'info');
    }
}

function removerJogador(index) {
    if (window.partidaManager) {
        window.partidaManager.removerJogador(index);
    }
}

document.addEventListener('DOMContentLoaded', function () {
    window.partidaManager = new PartidaManager();
    console.log('Partida manager initialized');
});