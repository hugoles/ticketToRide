using Microsoft.AspNetCore.Mvc;
using TicketToRide.Application.DTOs;
using TicketToRide.Application.Services.Interfaces;
using TicketToRideAPI.Application.DTOs.Request;

namespace TicketToRide.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurnoController : ControllerBase
    {
        private readonly ITurnoService _turnoService;

        public TurnoController(ITurnoService turnoService)
        {
            _turnoService = turnoService;
        }

        [HttpGet("partida/{partidaId}/turno/atual")]
        public ActionResult<TurnoDTO> ObterTurnoAtual(string partidaId)
        {
            TurnoDTO turno = _turnoService.ObterTurnoAtual(partidaId);
            return Ok(turno);
        }

        [HttpPost("partida/{partidaId}/turno/comprar-cartas")]
        public ActionResult<TurnoDTO> ComprarCartasVeiculo(string partidaId, [FromBody] ComprarCartasRequest request)
        {
            TurnoDTO turno = _turnoService.ComprarCartasVeiculo(partidaId, request.JogadorId, request.Indices);
            return Ok(turno);
        }

        [HttpPost("partida/{partidaId}/turno/reivindicar-rota")]
        public ActionResult<TurnoDTO> ReivindicarRota(string partidaId, [FromBody] ReivindicarRotaRequest request)
        {
            TurnoDTO turno = _turnoService.ReivindicarRota(partidaId, request.JogadorId, request.RotaId, request.CartasSelecionadas);
            return Ok(turno);
        }

        [HttpPost("partida/{partidaId}/turno/comprar-bilhetes")]
        public ActionResult<TurnoDTO> ComprarBilhetesDestino(string partidaId, [FromBody] ComprarBilhetesRequest request)
        {
            TurnoDTO turno = _turnoService.ComprarBilhetesDestino(partidaId, request.JogadorId, request.BilhetesSelecionados, request.PrimeiroTurno);
            return Ok(turno);
        }
    }
}