using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;
using TP7.ProductosModel;
using System;

using System.Collections.Generic;

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

    public Productos Create(Productos producto)
    {
        var query = @"INSERT INTO Productos (Descripcion, precio) VALUES (@descripcion, @precio);
                     SELECT last_insert_rowid();"; // Obtiene el id del último registro insertado
        
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        
        command.Parameters.Add(new SqliteParameter("@descripcion", producto.Descripcion));
        command.Parameters.Add(new SqliteParameter("@precio", producto.Precio));
        
        producto.IdProducto = Convert.ToInt32(command.ExecuteScalar());
        conexion.Close();
        
        return producto;
    }

    public bool Update(int id, Productos producto)
    {
        var query = "UPDATE Productos SET Descripcion = @descripcion, precio = @precio WHERE id = @id";
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        
        command.Parameters.Add(new SqliteParameter("@descripcion", producto.Descripcion));
        command.Parameters.Add(new SqliteParameter("@precio", producto.Precio));
        command.Parameters.Add(new SqliteParameter("@id", id));
        
        int rowsAffected = command.ExecuteNonQuery();
        conexion.Close();
        
        return rowsAffected > 0;
    }

    public Productos GetById(int id)
    {
        var query = "SELECT * FROM Productos WHERE id = @id";
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        command.Parameters.Add(new SqliteParameter("@id", id));

        using (SqliteDataReader reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                var producto = new Productos()
                {
                    IdProducto = Convert.ToInt32(reader["id"]),
                    Descripcion = reader["Descripcion"].ToString(),
                    Precio = Convert.ToInt32(reader["precio"])
                };
                return producto;
            }
        }
        conexion.Close();
        return null;
    }

    public bool Delete(int id)
    {
        var query = "DELETE FROM Productos WHERE id = @id";
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        command.Parameters.Add(new SqliteParameter("@id", id));
        
        int rowsAffected = command.ExecuteNonQuery();
        conexion.Close();
        
        return rowsAffected > 0;
    }
}