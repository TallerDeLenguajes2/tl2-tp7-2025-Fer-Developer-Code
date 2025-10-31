using System;
using Microsoft.AspNetCore.Mvc;
using TP7.ProductoRepositorySpace;
using TP7.ProductosModel;

namespace TP7.Controllers;

/// <summary>
/// Controlador para gestionar operaciones con productos
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly ProductoRepository _productoRepository;

    /// <summary>
    /// Constructor del controlador de productos
    /// </summary>
    public ProductosController()
    {
        _productoRepository = new ProductoRepository();
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    /// <param name="producto">Datos del producto a crear</param>
    /// <returns>El producto creado con su ID asignado</returns>
    /// <response code="200">Producto creado exitosamente</response>
    /// <response code="400">Error al crear el producto</response>
    [HttpPost]
    [ProducesResponseType(typeof(Productos), 200)]
    [ProducesResponseType(400)]
    public ActionResult<Productos> CrearProducto(Productos producto)
    {
        try
        {
            var nuevoProducto = _productoRepository.Create(producto);
            return Ok(nuevoProducto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Modifica un producto existente
    /// </summary>
    /// <param name="id">ID del producto a modificar</param>
    /// <param name="producto">Nuevos datos del producto</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Producto modificado exitosamente</response>
    /// <response code="404">Producto no encontrado</response>
    /// <response code="400">Error al modificar el producto</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult ModificarProducto(int id, Productos producto)
    {
        try
        {
            var resultado = _productoRepository.Update(id, producto);
            if (resultado)
            {
                return Ok("Producto modificado exitosamente");
            }
            return NotFound("Producto no encontrado");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene un producto por su ID
    /// </summary>
    /// <param name="id">ID del producto a buscar</param>
    /// <returns>Producto encontrado</returns>
    /// <response code="200">Producto encontrado</response>
    /// <response code="404">Producto no encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Productos), 200)]
    [ProducesResponseType(404)]
    public ActionResult<Productos> GetById(int id)
    {
        var producto = _productoRepository.GetById(id);
        if (producto == null)
        {
            return NotFound("Producto no encontrado");
        }
        return Ok(producto);
    }

    /// <summary>
    /// Elimina un producto por su ID
    /// </summary>
    /// <param name="id">ID del producto a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Producto eliminado exitosamente</response>
    /// <response code="404">Producto no encontrado</response>
    /// <response code="400">Error al eliminar el producto</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult EliminarProducto(int id)
    {
        try
        {
            var resultado = _productoRepository.Delete(id);
            if (resultado)
            {
                return Ok("Producto eliminado exitosamente");
            }
            return NotFound("Producto no encontrado");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los productos
    /// </summary>
    /// <returns>Lista de todos los productos</returns>
    /// <response code="200">Lista de productos obtenida exitosamente</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Productos>), 200)]
    public IActionResult GetAll()
    {
        var listado = _productoRepository.GetProducts();
        return Ok(listado);
    }
}