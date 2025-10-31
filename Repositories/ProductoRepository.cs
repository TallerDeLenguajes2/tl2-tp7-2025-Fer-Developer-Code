using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using TP7.ProductosModel;
using System;

namespace TP7.ProductoRepositorySpace;

public class ProductoRepository
{
    // El readonly garantiza que la cadena de conexión no pueda ser modificada después de su inicialización
    private readonly string cadenaConexion;

    public ProductoRepository()
    {
        // Data Source indica la ubicación de la base de datos SQLite
        cadenaConexion = "Data Source=DB/Tienda.db";
    }

    public List<Productos> GetProducts()
    {
        // La consulta SQL usa un formato multilínea para mejor legibilidad
        // El @ antes del string permite escribir en múltiples líneas sin usar \n
        const string query = @"
            SELECT IdProducto, 
                   Descripcion, 
                   Precio 
            FROM Productos";

        try
        {
            // using garantiza que los recursos se liberen correctamente
            // SqliteConnection maneja la conexión con la base de datos
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            // SqliteCommand representa la consulta SQL que vamos a ejecutar
            using var command = new SqliteCommand(query, conexion);

            // ExecuteReader() se usa para consultas que devuelven múltiples registros
            using var reader = command.ExecuteReader();
            var listadoProductos = new List<Productos>();

            // Read() avanza registro por registro hasta el final
            while (reader.Read())
            {
                // Convertimos cada registro a un objeto Productos
                var producto = new Productos
                {
                    // Convert.ToXXX asegura una conversión segura entre tipos de datos
                    IdProducto = Convert.ToInt32(reader["IdProducto"]),
                    Descripcion = reader["Descripcion"].ToString(),
                    Precio = Convert.ToDecimal(reader["Precio"])
                };
                listadoProductos.Add(producto);
            }
            return listadoProductos;
        }
        catch (Exception ex)
        {
            // Propagamos el error con contexto adicional
            throw new Exception("Error al obtener productos: " + ex.Message);
        }
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
        // Los parámetros @descripcion y @precio son marcadores que previenen SQL injection
        // SQL injection es un ataque donde el usuario intenta inyectar código SQL malicioso
        const string query = @"
            INSERT INTO Productos (
                Descripcion, 
                Precio
            ) VALUES (
                @descripcion, 
                @precio
            );
            SELECT last_insert_rowid();"; // Obtiene el ID del último registro insertado

        try
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            using var command = new SqliteCommand(query, conexion);

            // AddWithValue asocia un valor con un parámetro de manera segura
            command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
            command.Parameters.AddWithValue("@precio", producto.Precio);

            // ExecuteScalar() se usa cuando esperamos un único valor de retorno
            producto.IdProducto = Convert.ToInt32(command.ExecuteScalar());
            return producto;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear producto: " + ex.Message);
        }
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