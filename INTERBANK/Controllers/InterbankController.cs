using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using INTERBANK.Model;

namespace INTERBANK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoController : ControllerBase
    {
        private readonly InterbankContext _context;

        public MovimientoController(InterbankContext context)
        {
            _context = context;
        }

        // GET: api/Movimiento/ListarMovimientosCuenta
        [HttpGet("ListarMovimientosCuenta")]
        public async Task<ActionResult<IEnumerable<Movimiento>>> ListarMovimientosCuenta(int idCuenta)
        {
            var cuenta = await _context.Cuenta.FindAsync(idCuenta);
            if (cuenta == null)
                return NotFound("La cuenta no existe.");

            var movimientos = await _context.Movimientos
                .Where(m => m.IdCuenta == idCuenta)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            return movimientos;
        }


        // POST: api/Movimiento/ObtenerMovimientosPorRango
        [HttpPost("ObtenerMovimientosPorRango")]
        public async Task<ActionResult<IEnumerable<Movimiento>>> ObtenerMovimientosPorRango(
            int idCuenta, DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaFin < fechaInicio)
                return Content("La fecha final no puede ser menor a la inicial.");

            var cuenta = await _context.Cuenta.FindAsync(idCuenta);
            if (cuenta == null)
                return NotFound("La cuenta no existe.");

            var movimientos = await _context.Movimientos
                .Where(m => m.IdCuenta == idCuenta &&
                            m.Fecha >= fechaInicio &&
                            m.Fecha <= fechaFin)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            return movimientos;
        }



    }

    public class AhorroController : ControllerBase
    {
        private readonly InterbankContext _context;

        public AhorroController(InterbankContext context)
        {
            _context = context;
        }

        // GET: api/Ahorro/ListarAhorrosUsuario
        [HttpGet("ListarAhorrosUsuario")]
        public async Task<ActionResult<IEnumerable<ProductoFinanciero>>> ListarAhorrosUsuario(int idUsuario)
        {
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
                return NotFound("El usuario no existe.");

            var ahorros = await _context.ProductoFinancieros
                .Where(p => p.IdUsuario == idUsuario && p.Tipo== "AHORRO")
                .ToListAsync();

            return ahorros;
        }


        // POST: api/Ahorro/ObtenerDetalleAhorro
        [HttpPost("ObtenerDetalleAhorro")]
        public async Task<ActionResult<ProductoFinanciero>> ObtenerDetalleAhorro(int idProducto)
        {
            var ahorro = await _context.ProductoFinancieros.FirstOrDefaultAsync(p => p.IdProducto == idProducto);

            if (ahorro == null)
                return NotFound("El producto de ahorro no existe.");

            if (ahorro.Tipo != "AHORRO")
                return Content("El producto no es de tipo ahorro.");

            return ahorro;
        }
    }

    public class ProductosPersonalizadosController : ControllerBase
    {
        private readonly InterbankContext _context;

        public ProductosPersonalizadosController(InterbankContext context)
        {
            _context = context;
        }

        // GET: api/ProductosPersonalizados/ListarProductosPersonalizados
        [HttpGet("ListarProductosPersonalizados")]
        public async Task<ActionResult<IEnumerable<ProductoFinanciero>>> ListarProductosPersonalizados(int idUsuario)
        {
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
                return NotFound("El usuario no existe.");

            // ejemplo: productos de tipo 'PERSONALIZADO'
            var productos = await _context.ProductoFinancieros
                .Where(p => p.IdUsuario == idUsuario || p.Tipo== "PERSONALIZADO")
                .ToListAsync();

            return productos;
        }


        // GET: api/ProductosPersonalizados/ObtenerProductoRecomendado
        [HttpGet("ObtenerProductoRecomendado")]
        public async Task<ActionResult<ProductoFinanciero>> ObtenerProductoRecomendado(int idProducto)
        {
            var producto = await _context.ProductoFinancieros.FindAsync(idProducto);
            if (producto == null)
                return NotFound("El producto no existe.");

            return producto;
        }



        // POST: api/ProductosPersonalizados/CrearProductoFavorito
        [HttpPost("CrearProductoFavorito")]
        public async Task<IActionResult> CrearProductoFavorito(int idUsuario, int idProducto)
        {
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
                return NotFound("El usuario no existe.");

            var producto = await _context.ProductoFinancieros.FindAsync(idProducto);
            if (producto == null)
                return NotFound("El producto no existe.");

            // verificar si ya existe
            var existente = await _context.ProductoFavoritos
                .FirstOrDefaultAsync(f => f.IdUsuario == idUsuario && f.IdProducto == idProducto);

            if (existente != null)
                return Content("El producto ya está marcado como favorito.");

            ProductoFavorito nuevo = new ProductoFavorito()
            {
                IdUsuario = idUsuario,
                IdProducto = idProducto
            };

            _context.ProductoFavoritos.Add(nuevo);
            await _context.SaveChangesAsync();

            return Ok("Producto agregado como favorito.");
        }



        // PUT: api/ProductosPersonalizados/ActualizarPreferenciasProducto
        [HttpPut("ActualizarPreferenciasProducto")]
        public async Task<IActionResult> ActualizarPreferenciasProducto(int idUsuario, string categoria, string valor)
        {
            if (string.IsNullOrEmpty(categoria))
                return Content("La categoría no puede estar vacía.");

            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
                return NotFound("El usuario no existe.");

            var preferencia = await _context.PreferenciaProductos
                .FirstOrDefaultAsync(p => p.IdUsuario == idUsuario && p.Categoria == categoria);

            if (preferencia == null)
            {
                // crear si no existe
                preferencia = new PreferenciaProducto()
                {
                    IdUsuario = idUsuario,
                    Categoria = categoria,
                    Valor = valor
                };

                _context.PreferenciaProductos.Add(preferencia);
                await _context.SaveChangesAsync();

                return Ok("Preferencia creada.");
            }

            // actualizar si existe
            preferencia.Valor = valor;
            preferencia.FechaActualizacion = DateTime.Now;

            _context.Entry(preferencia).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Preferencia actualizada.");
        }



        // DELETE: api/ProductosPersonalizados/EliminarProductoFavorito
        [HttpDelete("EliminarProductoFavorito")]
        public async Task<IActionResult> EliminarProductoFavorito(int idUsuario, int idProducto)
        {
            var favorito = await _context.ProductoFavoritos
                .FirstOrDefaultAsync(f => f.IdUsuario == idUsuario && f.IdProducto == idProducto);

            if (favorito == null)
                return NotFound("El producto favorito no existe o ya fue eliminado.");

            _context.ProductoFavoritos.Remove(favorito);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CuentaAhorroController : ControllerBase
    {
        private readonly InterbankContext _context;

        public CuentaAhorroController(InterbankContext context)
        {
            _context = context;
        }


        // GET: api/CuentaAhorro/ListarTiposCuentaAhorro
        [HttpGet("ListarTiposCuentaAhorro")]
        public async Task<ActionResult<IEnumerable<TipoCuentaAhorro>>> ListarTiposCuentaAhorro()
        {
            var tipos = await _context.TipoCuentaAhorros
                .Where(t => t.Estado == "ACTIVO")
                .ToListAsync();

            return tipos;
        }


        // GET: api/CuentaAhorro/ObtenerRequisitosCuentaAhorro
        [HttpGet("ObtenerRequisitosCuentaAhorro")]
        public async Task<ActionResult<IEnumerable<RequisitoCuentaAhorro>>> ObtenerRequisitosCuentaAhorro(int idTipoCuenta)
        {
            var tipo = await _context.TipoCuentaAhorros.FindAsync(idTipoCuenta);

            if (tipo == null)
                return NotFound("El tipo de cuenta no existe.");

            var requisitos = await _context.RequisitoCuentaAhorros
                .Where(r => r.IdTipoCuenta == idTipoCuenta)
                .ToListAsync();

            return requisitos;
        }


        // POST: api/CuentaAhorro/CrearCuentaAhorro
        [HttpPost("CrearCuentaAhorro")]
        public async Task<IActionResult> CrearCuentaAhorro(
            int idUsuario,
            int idTipoCuenta,
            string numeroCuenta,
            string moneda)
        {
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
                return NotFound("El usuario no existe.");

            var tipo = await _context.TipoCuentaAhorros.FindAsync(idTipoCuenta);
            if (tipo == null)
                return NotFound("El tipo de cuenta no existe.");

            Cuentum nueva = new Cuentum()
            {
                IdUsuario = idUsuario,
                NumeroCuenta = numeroCuenta,
                TipoCuenta = "AHORRO",
                Moneda = moneda,
                SaldoDisponible = 0,
                Estado = "ACTIVO",
                FechaCreacion = DateTime.Now
            };

            _context.Cuenta.Add(nueva);
            await _context.SaveChangesAsync();

            return Ok("Cuenta de ahorro creada exitosamente.");
        }



        // PUT: api/CuentaAhorro/ActualizarCuentaAhorro
        [HttpPut("ActualizarCuentaAhorro")]
        public async Task<IActionResult> ActualizarCuentaAhorro(
            int idCuenta, string estado, string motivoCierre)
        {
            var cuenta = await _context.Cuenta.FindAsync(idCuenta);
            if (cuenta == null)
                return NotFound("La cuenta no existe.");

            cuenta.Estado = estado;

            if (estado == "CERRADA")
            {
                cuenta.FechaCierre = DateTime.Now;
                cuenta.MotivoCierre = motivoCierre;
            }

            _context.Entry(cuenta).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Cuenta actualizada correctamente.");
        }


        // DELETE: api/CuentaAhorro/EliminarCuentaAhorro
        [HttpDelete("EliminarCuentaAhorro")]
        public async Task<IActionResult> EliminarCuentaAhorro(int idCuenta)
        {
            var cuenta = await _context.Cuenta.FindAsync(idCuenta);
            if (cuenta == null)
                return NotFound("La cuenta no existe.");

            _context.Cuenta.Remove(cuenta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
