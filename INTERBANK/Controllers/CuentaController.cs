using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using INTERBANK.Model;

namespace INTERBANK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuentaController : ControllerBase
    {
        private readonly InterbankContext _context;

        public CuentaController(InterbankContext context)
        {
            _context = context;
        }

        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> GetCuentasPorUsuario(int idUsuario)
        {
            var cuentas = await _context.Cuenta
                .Where(c => c.IdUsuario == idUsuario)
                .Select(c => new
                {
                    c.IdCuenta,
                    c.NumeroCuenta,
                    c.TipoCuenta,
                    c.SaldoDisponible,
                    c.Moneda,
                    c.Estado
                })
                .ToListAsync();

            if (cuentas == null || cuentas.Count == 0)
                return NotFound("El usuario no tiene cuentas registradas.");

            return Ok(cuentas);
        }

        [HttpGet("saldo/{idCuenta}")]
        public async Task<IActionResult> GetSaldoCuenta(int idCuenta)
        {
            var cuenta = await _context.Cuenta
                .Where(c => c.IdCuenta == idCuenta)
                .Select(c => new
                {
                    c.IdCuenta,
                    c.NumeroCuenta,
                    c.SaldoDisponible,
                    c.Moneda,
                    c.Estado
                })
                .FirstOrDefaultAsync();

            if (cuenta == null)
                return NotFound("Cuenta no encontrada.");

            return Ok(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> CrearCuenta([FromBody] Cuentum cuenta)
        {
            _context.Cuenta.Add(cuenta);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cuenta creada correctamente.",
                cuenta.IdCuenta
            });
        }
    }
}
