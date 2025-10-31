using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using TP7.ProductosModel;
using System;

namespace TP7.ProductoRepositorySpace;

public class ProductoRepository
{
    private readonly string cadenaConexion;

    public ProductoRepository()
    {
        // Conexión a la base de datos SQLite
        cadenaConexion = "Data Source=DB/Tienda.db";
    }

    public List<Productos> GetProducts()
    {
        var listadoProductos = new List<Productos>();
        
        // Consulta parametrizada para evitar SQL injection
        const string query = @"
            SELECT *
            FROM Productos";

        try
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            using var command = new SqliteCommand(query, conexion);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                listadoProductos.Add(new Productos
                {
                    IdProducto = Convert.ToInt32(reader["IdProducto"]),
                    Descripcion = reader["Descripcion"].ToString(),
                    Precio = Convert.ToDecimal(reader["Precio"])
                });
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener productos: " + ex.Message);
        }

        return listadoProductos;
    }

    public Productos GetById(int id)
    {
        const string query = @"
            SELECT IdProducto, 
                   Descripcion, 
                   Precio 
            FROM Productos 
            WHERE IdProducto = @id";

        try
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            using var command = new SqliteCommand(query, conexion);
            
            // Uso seguro de parámetros
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Productos
                {
                    IdProducto = Convert.ToInt32(reader["IdProducto"]),
                    Descripcion = reader["Descripcion"].ToString(),
                    Precio = Convert.ToDecimal(reader["Precio"])
                };
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener producto {id}: " + ex.Message);
        }
    }

    public Productos Create(Productos producto)
    {
        // CORREGIDO: Nombres de columnas
        var query = @"INSERT INTO Productos (Descripcion, Precio) VALUES (@descripcion, @precio);
                     SELECT last_insert_rowid();"; 
        
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        
        command.Parameters.Add(new SqliteParameter("@descripcion", producto.Descripcion));
        command.Parameters.Add(new SqliteParameter("@precio", producto.Precio));
        
        // ExecuteScalar devuelve un 'long' (Int64), es más seguro convertirlo así
        producto.IdProducto = Convert.ToInt32(command.ExecuteScalar());
        conexion.Close();
        
        return producto;
    }

    public bool Update(int id, Productos producto)
    {
        // CORREGIDO: Nombres de columnas
        var query = "UPDATE Productos SET Descripcion = @descripcion, Precio = @precio WHERE IdProducto = @id";
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

    public bool Delete(int id)
    {
        // CORREGIDO: Nombres de columnas
        var query = "DELETE FROM Productos WHERE IdProducto = @id";
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        command.Parameters.Add(new SqliteParameter("@id", id));
        
        int rowsAffected = command.ExecuteNonQuery();
        conexion.Close();
        
        return rowsAffected > 0;
    }
}