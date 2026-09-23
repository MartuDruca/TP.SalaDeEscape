using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Data.SqlClient;

using Microsoft.Data.SqlClient;

public static class BD
{
    private static string _connectionString = @"Server=localhost;Database=SalaDeEscape;Integrated Security=True;TrustServerCertificate=True;";


    public static int CrearJugador(string nombre)
    {
        string query = @"
            INSERT INTO Jugador (nombre, SalaActual)
            VALUES (@nombre, 1);

            SELECT SCOPE_IDENTITY();
        ";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@nombre", nombre);

                int id = Convert.ToInt32(cmd.ExecuteScalar());

                return id;
            }
        }
    }

    public static Partida? ObtenerPartida(int id)
    {
        string query = @"
            SELECT Id, FechaInicio, FechaFin, salaActual AS SalaActual, JugadorId
            FROM Partida
            WHERE Id = @id
        ";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Partida
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaInicio")),
                            FechaFin = reader.IsDBNull(reader.GetOrdinal("FechaFin")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaFin")),
                            SalaActual = reader.GetInt32(reader.GetOrdinal("SalaActual")),
                            JugadorId = reader.GetInt32(reader.GetOrdinal("JugadorId"))
                        };
                    }
                }
            }
        }

        return null;
    }


    public static int CrearPartida(int jugadorId)
    {
        string query = @"
            INSERT INTO Partida
            (FechaInicio, salaActual, JugadorId, FechaFin)
            VALUES
            (@fechaInicio, 1, @jugadorId, NULL);

            SELECT SCOPE_IDENTITY();
        ";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@fechaInicio", DateTime.Now);
                cmd.Parameters.AddWithValue("@jugadorId", jugadorId);

                int id = Convert.ToInt32(cmd.ExecuteScalar());

                return id;
            }
        }
    }


    public static void ActualizarSalaActual(int partidaId, int jugadorId, int sala)
    {
        string query = @"
            UPDATE Partida
            SET salaActual = @sala
            WHERE Id = @id;

            UPDATE Jugador
            SET SalaActual = @sala
            WHERE Id = @jugadorId;
        ";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@sala", sala);
                cmd.Parameters.AddWithValue("@id", partidaId);
                cmd.Parameters.AddWithValue("@jugadorId", jugadorId);

                cmd.ExecuteNonQuery();
            }
        }
    }


    public static void FinalizarPartida(int partidaId)
    {
        string query = @"
            UPDATE Partida
            SET FechaFin = @fechaFin
            WHERE Id = @id
        ";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@fechaFin", DateTime.Now);
                cmd.Parameters.AddWithValue("@id", partidaId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}