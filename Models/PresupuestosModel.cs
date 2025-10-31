using TP7.PresupuestosDetalleModel;

namespace TP7.PresupuestoModel;

public class Presupuesto
{
    int _idPresupuesto;
    string? _nombreDestinatario;
    DateTime _fechaCreacion;
    List<PresupuestosDetalle> _detalle;

    public int MontoPresupuesto()
    {
        int monto = 1;
        return monto;
    }

    public int MontoPresupuestoConIVA()
    {
        int monto = 1;
        return monto;
    }

    public int CantidadProductos()
    {
        int monto = 1;
        return monto;
    }
}