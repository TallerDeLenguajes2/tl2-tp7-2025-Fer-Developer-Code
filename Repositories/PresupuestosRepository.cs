using Microsoft.Data.Sqlite;
using TP7.PresupuestoModel;
using TP7.PresupuestosDetalleModel;
using TP7.ProductosModel;
using System;
using System.Collections.Generic;

namespace TP7.PresupuestosRepositorySpace;

public class PresupuestosRepository
{
    // Cadena de conexión readonly para prevenir modificaciones accidentales
    private readonly string cadenaConexion;

    public PresupuestosRepository()
    {
        cadenaConexion = "Data Source=DB/Tienda.db";
    }

    /// <summary>
    /// Crea un nuevo presupuesto con sus detalles en la base de datos usando una transacción
    /// </summary>
    public Presupuesto Create(Presupuesto presupuesto)
    {
        // Query principal para insertar el presupuesto
        // El @ permite escribir strings multilínea para mejor legibilidad
        const string query = @"
            INSERT INTO Presupuestos (
                NombreDestinatario,  -- Nombre de quien recibe el presupuesto
                FechaCreacion       -- Fecha en que se crea el presupuesto
            ) VALUES (
                @nombre,            -- Parámetro para evitar SQL injection
                @fecha             -- Parámetro para evitar SQL injection
            );
            SELECT last_insert_rowid();"; //Obtiene el ID del nuevo presupuesto

        try
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            
            // Iniciamos una transacción para garantizar la integridad de los datos
            using var transaction = conexion.BeginTransaction();

            try
            {
                using var command = new SqliteCommand(query, conexion, transaction);
                command.Parameters.AddWithValue("@nombre", presupuesto.NombreDestinatario);
                command.Parameters.AddWithValue("@fecha", presupuesto.FechaCreacion);

                // Guardamos el ID generado en el objeto presupuesto
                presupuesto.IdPresupuesto = Convert.ToInt32(command.ExecuteScalar());

                // Si hay detalles, los insertamos uno por uno
                if (presupuesto.Detalle != null)
                {
                    foreach (var detalle in presupuesto.Detalle)
                    {
                        // Usamos un método auxiliar para mantener el código organizado
                        AgregarProductoAPresupuestoConTransaction(
                            presupuesto.IdPresupuesto,
                            detalle.Producto.IdProducto,
                            detalle.Cantidad,
                            conexion,
                            transaction
                        );
                    }
                }

                // Commit confirma todos los cambios de la transacción
                transaction.Commit();
                return presupuesto;
            }
            catch
            {
                // Si algo falla, deshacemos todos los cambios
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear presupuesto: " + ex.Message);
        }
    }

    private void AgregarProductoAPresupuestoConTransaction(
        int presupuestoId, 
        int productoId, 
        int cantidad,
        SqliteConnection conexion,
        SqliteTransaction transaction)
    {
        const string query = @"
            INSERT INTO PresupuestosDetalle (
                IdPresupuesto, 
                IdProducto, 
                Cantidad
            ) VALUES (
                @presupuestoId, 
                @productoId, 
                @cantidad
            )";

        using var command = new SqliteCommand(query, conexion, transaction);
        command.Parameters.AddWithValue("@presupuestoId", presupuestoId);
        command.Parameters.AddWithValue("@productoId", productoId);
        command.Parameters.AddWithValue("@cantidad", cantidad);
        command.ExecuteNonQuery();
    }

    public List<Presupuesto> GetAll()
    {
        var presupuestos = new List<Presupuesto>();
        // CORREGIDO: Nombres de columnas
        var query = "SELECT IdPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos";
        
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var presupuesto = new Presupuesto
                {
                    // CORREGIDO: Nombres de columnas
                    IdPresupuesto = Convert.ToInt32(reader["IdPresupuesto"]),
                    NombreDestinatario = reader["NombreDestinatario"].ToString(),
                    FechaCreacion = DateTime.Parse(reader["FechaCreacion"].ToString()),
                    Detalle = ObtenerDetallesPresupuesto(Convert.ToInt32(reader["IdPresupuesto"]))
                };
                presupuestos.Add(presupuesto);
            }
        }
        return presupuestos;
    }

    public Presupuesto GetById(int id)
    {
        // CORREGIDO: Nombres de columnas
        var query = "SELECT IdPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos WHERE IdPresupuesto = @id";
        
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        command.Parameters.Add(new SqliteParameter("@id", id));
        
        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                return new Presupuesto
                {
                    // CORREGIDO: Nombres de columnas
                    IdPresupuesto = Convert.ToInt32(reader["IdPresupuesto"]),
                    NombreDestinatario = reader["NombreDestinatario"].ToString(),
                    FechaCreacion = DateTime.Parse(reader["FechaCreacion"].ToString()),
                    Detalle = ObtenerDetallesPresupuesto(id)
                };
            }
        }
        return null;
    }

    public void AgregarProductoAPresupuesto(int presupuestoId, int productoId, int cantidad)
    {
        // CORREGIDO: Nombres de columnas
        var query = @"INSERT INTO PresupuestosDetalle (IdPresupuesto, IdProducto, Cantidad) 
                     VALUES (@presupuestoId, @productoId, @cantidad)";
        
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        
        command.Parameters.Add(new SqliteParameter("@presupuestoId", presupuestoId));
        command.Parameters.Add(new SqliteParameter("@productoId", productoId));
        command.Parameters.Add(new SqliteParameter("@cantidad", cantidad));
        
        command.ExecuteNonQuery();
    }

    public bool Delete(int id)
    {
        // CORREGIDO: Nombres de columnas
        var queryDetalle = "DELETE FROM PresupuestosDetalle WHERE IdPresupuesto = @id";
        var queryPresupuesto = "DELETE FROM Presupuestos WHERE IdPresupuesto = @id";
        
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        
        var command = new SqliteCommand(queryDetalle, conexion);
        command.Parameters.Add(new SqliteParameter("@id", id));
        command.ExecuteNonQuery();
        
        command.CommandText = queryPresupuesto;
        var rowsAffected = command.ExecuteNonQuery();
        
        return rowsAffected > 0;
    }

    /// <summary>
    /// Obtiene los detalles de un presupuesto usando un JOIN con la tabla Productos
    /// </summary>
    private List<PresupuestosDetalle> ObtenerDetallesPresupuesto(int presupuestoId)
    {
        // Esta consulta usa JOIN para obtener los datos del producto relacionado
        const string query = @"
            SELECT 
                pd.Cantidad,           -- Cantidad del producto en el presupuesto
                p.IdProducto,          -- ID del producto
                p.Descripcion,         -- Descripción del producto
                p.Precio              -- Precio del producto
            FROM PresupuestosDetalle pd 
            INNER JOIN Productos p ON pd.IdProducto = p.IdProducto 
            WHERE pd.IdPresupuesto = @id";

        var detalles = new List<PresupuestosDetalle>();
        
        try
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            using var command = new SqliteCommand(query, conexion);
            
            // Uso seguro de parámetros para evitar SQL injection
            command.Parameters.AddWithValue("@id", presupuestoId);
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Creamos objetos PresupuestosDetalle con los datos de la BD
                var detalle = new PresupuestosDetalle
                {
                    Cantidad = Convert.ToInt32(reader["Cantidad"]),
                    Producto = new Productos
                    {
                        IdProducto = Convert.ToInt32(reader["IdProducto"]),
                        Descripcion = reader["Descripcion"].ToString(),
                        Precio = Convert.ToDecimal(reader["Precio"])
                    }
                };
                detalles.Add(detalle);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener detalles del presupuesto {presupuestoId}: " + ex.Message);
        }

        return detalles;
    }
}