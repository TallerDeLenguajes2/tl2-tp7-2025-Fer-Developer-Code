using Microsoft.AspNetCore.Mvc;
using TP7.PresupuestoModel;
using TP7.PresupuestosRepositorySpace;
using TP7.ProductosModel;
using TP7.PresupuestosDetalleModel;
using System;
using System.Collections.Generic;

namespace TP7.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PresupuestoController : ControllerBase
{
    private readonly PresupuestosRepository _presupuestoRepository;

    public PresupuestoController()
    {
        _presupuestoRepository = new PresupuestosRepository();
    }

    [HttpPost]
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

    [HttpPost("{id}/ProductoDetalle")]
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

    [HttpGet("{id}")]
    public ActionResult<Presupuesto> GetById(int id)
    {
        var presupuesto = _presupuestoRepository.GetById(id);
        if (presupuesto == null)
        {
            return NotFound("Presupuesto no encontrado");
        }
        return Ok(presupuesto);
    }

    [HttpGet]
    public ActionResult<List<Presupuesto>> GetAll()
    {
        var presupuestos = _presupuestoRepository.GetAll();
        return Ok(presupuestos);
    }

    [HttpDelete("{id}")]
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