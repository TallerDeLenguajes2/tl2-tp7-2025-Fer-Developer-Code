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
    ActionResult<Productos> CrearProducto()
    {
        return Ok("Producto guardado exitosamente");
    }

    [HttpGet]
    public IActionResult GetAll(){
        var listado = _productoRepository.GetProducts();
        return Ok(listado);
    }
}
