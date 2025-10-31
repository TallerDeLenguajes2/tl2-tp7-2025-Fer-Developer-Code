using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;
using TP7.ProductosModel;

namespace TP7.ProductoRepositorySpace;

public class ProductoRepository
{
    private string cadenaConexion = "Data Source = DB/Tienda.db";

    public List<Productos> GetProducts()
    {
        string query = "Select * FROM Productos";
        List<Productos> listadoProductos = new List<Productos>();
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var product = new Productos(){
                    IdProducto = Convert.ToInt32(reader["id"]),
                    Descripcion = reader["Descripcion"].ToString(),
                    Precio = Convert.ToInt32(reader["precio"])
                };
                listadoProductos.Add(product);
            }
        }
        conexion.Close();
        return listadoProductos;
    }
}