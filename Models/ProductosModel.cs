namespace TP7.ProductosModel;

public class Productos{
    int _idProducto;
    string _descripcion;
    int _precio;

    public int IdProducto { get => _idProducto; set => _idProducto = value; }
    public string Descripcion { get => _descripcion; set => _descripcion = value; }
    public int Precio { get => _precio; set => _precio = value; }
}