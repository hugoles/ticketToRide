// Ticket to Ride - Frontend JavaScript Application

class TicketToRideApp {
    constructor() {
        this.apiBaseUrl = 'http://localhost:5257'; // ASP.NET Core port
        this.currentPartidaId = null;
        this.currentJogadorId = null;
        this.init();
    }

    init() {
        console.log('Ticket to Ride App initialized');
        this.initDarkMode();
        this.setupEventListeners();
        this.updateGameStatus('Aguardando configuração');
        this.testAPI();
    }

    setupEventListeners() {
        document.getElementById('adicionarJogadorBtn')?.addEventListener('click', () => {
            adicionarJogador();
        });

        document.getElementById('iniciarPartidaBtn')?.addEventListener('click', () => {
            iniciarPartida();
        });

        document.getElementById('novaPartidaBtn')?.addEventListener('click', () => {
            novaPartida();
        });

        document.getElementById('comprarCartasBtn')?.addEventListener('click', () => {
            comprarCartas();
        });

        document.getElementById('reivindicarRotaBtn')?.addEventListener('click', () => {
            reivindicarRota();
        });

        document.getElementById('comprarBilhetesBtn')?.addEventListener('click', () => {
            comprarBilhetes();
        });

        document.getElementById('atualizarJogoBtn')?.addEventListener('click', () => {
            atualizarJogo();
        });
    }

    async testAPI() {
        try {
            const response = await fetch(`${this.apiBaseUrl}/api/partida`);
            if (response.ok) {
                console.log('API connection successful');
                this.updateGameStatus('Conectado ao servidor');
            } else {
                console.log('API connection failed');
                this.updateGameStatus('Erro de conexão com servidor');
            }
        } catch (error) {
            console.error('API test failed:', error);
            this.updateGameStatus('Erro de conexão com servidor');
        }
    }

    async updateGameStatus(message) {
        const statusElement = document.getElementById('game-status');
        console.log('updateGameStatus chamado com:', message);
        if (statusElement) {
            statusElement.textContent = message;
            console.log('Status atualizado para:', statusElement.textContent);
        } else {
            console.error('Elemento game-status não encontrado!');
        }
    }

    showScreen(screenId) {
        // Hide all screens
        const screens = ['setup-screen', 'game-screen', 'result-screen'];
        screens.forEach(id => {
            const element = document.getElementById(id);
            if (element) {
                element.style.display = 'none';
            }
        });

        // Show target screen
        const targetScreen = document.getElementById(screenId);
        if (targetScreen) {
            targetScreen.style.display = 'block';
        }
    }

    showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show`;
        notification.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

        const container = document.getElementById('notifications');
        if (container) {
            container.appendChild(notification);

            // Auto remove after 5 seconds
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.remove();
                }
            }, 5000);
        }
    }

    async makeApiCall(endpoint, method = 'GET', data = null) {
        try {
            const options = {
                method: method,
                headers: {
                    'Content-Type': 'application/json',
                }
            };

            if (data) {
                options.body = JSON.stringify(data);
            }

            const response = await fetch(`${this.apiBaseUrl}${endpoint}`, options);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            console.error('API call failed:', error);
            this.showNotification(`Erro na comunicação com o servidor: ${error.message}`, 'danger');
            throw error;
        }
    }

    toggleDarkMode() {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);

        const darkModeBtn = document.getElementById('dark-mode-btn');
        if (darkModeBtn) {
            if (newTheme === 'dark') {
                darkModeBtn.innerHTML = '<i class="fas fa-sun"></i> Light Mode';
            } else {
                darkModeBtn.innerHTML = '<i class="fas fa-moon"></i> Dark Mode';
            }
        }
    }

    initDarkMode() {
        const savedTheme = localStorage.getItem('theme') || 'light';
        document.documentElement.setAttribute('data-theme', savedTheme);

        const darkModeBtn = document.getElementById('dark-mode-btn');
        if (darkModeBtn) {
            if (savedTheme === 'dark') {
                darkModeBtn.innerHTML = '<i class="fas fa-sun"></i> Light Mode';
            } else {
                darkModeBtn.innerHTML = '<i class="fas fa-moon"></i> Dark Mode';
            }
        }
    }
}

// Global functions for HTML onclick events
function adicionarJogador() {
    if (window.partidaManager) {
        window.partidaManager.adicionarJogador();
    }
}

function iniciarPartida() {
    if (window.partidaManager) {
        window.partidaManager.iniciarPartida();
    }
}

function novaPartida() {
    if (window.partidaManager) {
        window.partidaManager.novaPartida();
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

function atualizarJogo() {
    if (window.jogoManager) {
        window.jogoManager.atualizarJogo();
    }
}

function toggleDarkMode() {
    if (window.ticketToRideApp) {
        window.ticketToRideApp.toggleDarkMode();
    }
}

function passarTurno() {
    if (window.jogoManager) {
        window.jogoManager.passarTurno();
    }
}

// Initialize the app when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    window.ticketToRideApp = new TicketToRideApp();
});