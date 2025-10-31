using Microsoft.AspNetCore.Mvc;
using TP7.PresupuestoModel;
using TP7.PresupuestosRepositorySpace;
using TP7.PresupuestosDetalleModel;

namespace TP7.Controllers;

/// <summary>
/// Controlador para gestionar operaciones con presupuestos
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PresupuestoController : ControllerBase
{
    private readonly PresupuestosRepository _presupuestoRepository;

    /// <summary>
    /// Constructor del controlador de presupuestos
    /// </summary>
    public PresupuestoController()
    {
        _presupuestoRepository = new PresupuestosRepository();
    }

    /// <summary>
    /// Crea un nuevo presupuesto
    /// </summary>
    /// <param name="presupuesto">Datos del presupuesto a crear</param>
    /// <returns>El presupuesto creado con su ID asignado</returns>
    /// <response code="200">Presupuesto creado exitosamente</response>
    /// <response code="400">Error al crear el presupuesto</response>
    [HttpPost]
    [ProducesResponseType(typeof(Presupuesto), 200)]
    [ProducesResponseType(400)]
    public ActionResult<Presupuesto> CrearPresupuesto(Presupuesto presupuesto)
    {
        try
        {
            var nuevoPresupuesto = _presupuestoRepository.Create(presupuesto);
            return Ok(nuevoPresupuesto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Agrega un producto al detalle de un presupuesto
    /// </summary>
    /// <param name="id">ID del presupuesto</param>
    /// <param name="detalle">Detalle del producto a agregar</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Producto agregado exitosamente</response>
    /// <response code="400">Error al agregar el producto</response>
    [HttpPost("{id}/ProductoDetalle")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult AgregarProductoAPresupuesto(int id, [FromBody] PresupuestosDetalle detalle)
    {
        try
        {
            _presupuestoRepository.AgregarProductoAPresupuesto(id, detalle.Producto.IdProducto, detalle.Cantidad);
            return Ok("Producto agregado al presupuesto exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene un presupuesto por su ID
    /// </summary>
    /// <param name="id">ID del presupuesto a buscar</param>
    /// <returns>Presupuesto encontrado</returns>
    /// <response code="200">Presupuesto encontrado</response>
    /// <response code="404">Presupuesto no encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Presupuesto), 200)]
    [ProducesResponseType(404)]
    public ActionResult<Presupuesto> GetById(int id)
    {
        var presupuesto = _presupuestoRepository.GetById(id);
        if (presupuesto == null)
        {
            return NotFound("Presupuesto no encontrado");
        }
        return Ok(presupuesto);
    }

    /// <summary>
    /// Obtiene todos los presupuestos
    /// </summary>
    /// <returns>Lista de todos los presupuestos</returns>
    /// <response code="200">Lista de presupuestos obtenida exitosamente</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Presupuesto>), 200)]
    public ActionResult<List<Presupuesto>> GetAll()
    {
        var presupuestos = _presupuestoRepository.GetAll();
        return Ok(presupuestos);
    }

    /// <summary>
    /// Elimina un presupuesto por su ID
    /// </summary>
    /// <param name="id">ID del presupuesto a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Presupuesto eliminado exitosamente</response>
    /// <response code="404">Presupuesto no encontrado</response>
    /// <response code="400">Error al eliminar el presupuesto</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult EliminarPresupuesto(int id)
    {
        try
        {
            var resultado = _presupuestoRepository.Delete(id);
            if (resultado)
            {
                return Ok("Presupuesto eliminado exitosamente");
            }
            return NotFound("Presupuesto no encontrado");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}