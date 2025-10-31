using System;
using Microsoft.AspNetCore.Mvc;
using TP7.ProductoRepositorySpace;
using TP7.ProductosModel;

namespace TP7.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private ProductoRepository _productoRepository;
    public ProductosController(){
        _productoRepository = new ProductoRepository();
    }

    [HttpPost]
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

    [HttpPut("{id}")]
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

    [HttpGet("{id}")]
    public ActionResult<Productos> GetById(int id)
    {
        var producto = _productoRepository.GetById(id);
        if (producto == null)
        {
            return NotFound("Producto no encontrado");
        }
        return Ok(producto);
    }

    [HttpDelete("{id}")]
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

    [HttpGet]
    public IActionResult GetAll(){
        var listado = _productoRepository.GetProducts();
        return Ok(listado);
    }
}
