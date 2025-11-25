using Microsoft.AspNetCore.Mvc;
using TicketToRide.Application.DTOs;
using TicketToRide.Application.Services;

namespace TicketToRide.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurnoController : ControllerBase
    {
        private readonly TurnoService _turnoService;

        public TurnoController(TurnoService turnoService)
        {
            _turnoService = turnoService;
        }

        [HttpGet("partida/{partidaId}/turno/atual")]
        public ActionResult<TurnoDTO> ObterTurnoAtual(string partidaId)
        {
            try
            {
                TurnoDTO turno = _turnoService.ObterTurnoAtual(partidaId);
                return Ok(turno);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("partida/{partidaId}/turno/comprar-cartas")]
        public ActionResult<TurnoDTO> ComprarCartasVeiculo(string partidaId, [FromBody] ComprarCartasRequest request)
        {
            try
            {
                TurnoDTO turno = _turnoService.ComprarCartasVeiculo(partidaId, request.JogadorId, request.Indices);
                return Ok(turno);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("partida/{partidaId}/turno/reivindicar-rota")]
        public ActionResult<TurnoDTO> ReivindicarRota(string partidaId, [FromBody] ReivindicarRotaRequest request)
        {
            try
            {
                TurnoDTO turno = _turnoService.ReivindicarRota(partidaId, request.JogadorId, request.RotaId);
                return Ok(turno);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("partida/{partidaId}/turno/comprar-bilhetes")]
        public ActionResult<TurnoDTO> ComprarBilhetesDestino(string partidaId, [FromBody] ComprarBilhetesRequest request)
        {
            try
            {
                TurnoDTO turno = _turnoService.ComprarBilhetesDestino(partidaId, request.JogadorId, request.BilhetesSelecionados);
                return Ok(turno);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class ComprarCartasRequest
    {
        public string JogadorId { get; set; } = string.Empty;
        public List<int>? Indices { get; set; }
    }

    public class ReivindicarRotaRequest
    {
        public string JogadorId { get; set; } = string.Empty;
        public string RotaId { get; set; } = string.Empty;
    }

    public class ComprarBilhetesRequest
    {
        public string JogadorId { get; set; } = string.Empty;
        public List<string> BilhetesSelecionados { get; set; } = [];
    }

    public class PassarTurnoRequest
    {
        public string JogadorId { get; set; } = string.Empty;
    }
}