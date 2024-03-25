using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace APIPRUEBAS.Controllers
{
    [Authorize]
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        public readonly DBAPIContext _context;

        public ProductoController(DBAPIContext context)
        {
            _context = context;
        }

        [HttpGet, Route("Listar")]
        public IActionResult Lista()
        {
            try
            {
                var lista = _context.Productos.Include(c => c.Categoria).ToList();

                return StatusCode(StatusCodes.Status200OK, new { message = "OK", lresponse = lista });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet, Route("Obtener/{id:int}")]
        public IActionResult Obtener(int id)
        {
            try
            {
                var objeto = _context.Productos.Find(id);

                if (objeto == null)
                {
                    return BadRequest("Producto no encontrado");
                }

                objeto = _context.Productos.Include(c => c.Categoria).Where(p => p.IdProducto == id).FirstOrDefault();

                return StatusCode(StatusCodes.Status200OK, new { message = "OK", oresponse = objeto });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost, Route("Guardar")]
        public IActionResult Guardar([FromBody] Producto obj)
        {
            try
            {
                _context.Productos.Add(obj); ;
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { message = "OK" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut, Route("Editar")]
        public IActionResult Editar([FromBody] Producto obj)
        {
            var objeto = _context.Productos.Find(obj.IdProducto);

            if (objeto == null)
            {
                return BadRequest("Producto no encontrado");
            }

            try
            {
                objeto.CodigoBarra = obj.CodigoBarra is null ? objeto.CodigoBarra : obj.CodigoBarra;
                objeto.Descripcion = obj.Descripcion is null ? objeto.Descripcion : obj.Descripcion;
                objeto.Marca = obj.Marca is null ? objeto.Marca : obj.Marca;
                objeto.IdCategoria = obj.IdCategoria is null ? objeto.IdCategoria : obj.IdCategoria;
                objeto.Precio = obj.Precio is null ? objeto.Precio : obj.Precio;

                _context.Productos.Update(objeto);
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { message = "OK" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpDelete, Route("Eliminar/{id:int}")]
        public IActionResult Eliminar(int id)
        {
            var objeto = _context.Productos.Find(id);

            if (objeto == null)
            {
                return BadRequest("Producto no encontrado");
            }

            try
            {
                _context.Productos.Remove(objeto);
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { message = "OK" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
