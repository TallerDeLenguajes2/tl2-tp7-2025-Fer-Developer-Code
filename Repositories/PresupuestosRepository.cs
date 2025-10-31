using Microsoft.Data.Sqlite;
using TP7.PresupuestoModel;
using TP7.PresupuestosDetalleModel;
using TP7.ProductosModel;
using System;
using System.Collections.Generic;

namespace TP7.PresupuestosRepositorySpace;

public class PresupuestosRepository
{
    private string cadenaConexion = "Data Source=DB/Tienda.db";

    public Presupuesto Create(Presupuesto presupuesto)
    {
        var query = @"INSERT INTO Presupuestos (nombreDestinatario, fechaCreacion) 
                     VALUES (@nombre, @fecha);
                     SELECT last_insert_rowid();";
        
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        
        command.Parameters.Add(new SqliteParameter("@nombre", presupuesto.NombreDestinatario));
        command.Parameters.Add(new SqliteParameter("@fecha", presupuesto.FechaCreacion));
        
        presupuesto.IdPresupuesto = Convert.ToInt32(command.ExecuteScalar());

        // Guardar los detalles
        if (presupuesto.Detalle != null)
        {
            foreach (var detalle in presupuesto.Detalle)
            {
                AgregarProductoAPresupuesto(presupuesto.IdPresupuesto, detalle.Producto.IdProducto, detalle.Cantidad);
            }
        }
        
        return presupuesto;
    }

    public List<Presupuesto> GetAll()
    {
        var presupuestos = new List<Presupuesto>();
        var query = "SELECT * FROM Presupuestos";
        
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var presupuesto = new Presupuesto
                {
                    IdPresupuesto = Convert.ToInt32(reader["id"]),
                    NombreDestinatario = reader["nombreDestinatario"].ToString(),
                    FechaCreacion = DateTime.Parse(reader["fechaCreacion"].ToString()),
                    Detalle = ObtenerDetallesPresupuesto(Convert.ToInt32(reader["id"]))
                };
                presupuestos.Add(presupuesto);
            }
        }
        return presupuestos;
    }

    public Presupuesto GetById(int id)
    {
        var query = "SELECT * FROM Presupuestos WHERE id = @id";
        
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
                    IdPresupuesto = Convert.ToInt32(reader["id"]),
                    NombreDestinatario = reader["nombreDestinatario"].ToString(),
                    FechaCreacion = DateTime.Parse(reader["fechaCreacion"].ToString()),
                    Detalle = ObtenerDetallesPresupuesto(id)
                };
            }
        }
        return null;
    }

    public void AgregarProductoAPresupuesto(int presupuestoId, int productoId, int cantidad)
    {
        var query = @"INSERT INTO PresupuestosDetalle (presupuestoId, productoId, cantidad) 
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
        // Primero eliminamos los detalles
        var queryDetalle = "DELETE FROM PresupuestosDetalle WHERE presupuestoId = @id";
        var queryPresupuesto = "DELETE FROM Presupuestos WHERE id = @id";
        
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        
        var command = new SqliteCommand(queryDetalle, conexion);
        command.Parameters.Add(new SqliteParameter("@id", id));
        command.ExecuteNonQuery();
        
        command.CommandText = queryPresupuesto;
        var rowsAffected = command.ExecuteNonQuery();
        
        return rowsAffected > 0;
    }

    private List<PresupuestosDetalle> ObtenerDetallesPresupuesto(int presupuestoId)
    {
        var detalles = new List<PresupuestosDetalle>();
        var query = @"SELECT pd.*, p.* 
                     FROM PresupuestosDetalle pd 
                     INNER JOIN Productos p ON pd.productoId = p.id 
                     WHERE pd.presupuestoId = @id";
        
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        var command = new SqliteCommand(query, conexion);
        command.Parameters.Add(new SqliteParameter("@id", presupuestoId));
        
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var detalle = new PresupuestosDetalle
                {
                    Cantidad = Convert.ToInt32(reader["cantidad"]),
                    Producto = new Productos
                    {
                        IdProducto = Convert.ToInt32(reader["productoId"]),
                        Descripcion = reader["Descripcion"].ToString(),
                        Precio = Convert.ToInt32(reader["precio"])
                    }
                };
                detalles.Add(detalle);
            }
        }
        return detalles;
    }
}
