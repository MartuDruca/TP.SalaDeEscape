using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Data.SqlClient;

public static class BD
{

    private static string _connectionString = @"Server=localhost;Database=SalaDeEscape;Integrated Security=True;TrustServerCertificate=True;";

    public static void CrearPartida(Partida partida)
    {
            string query = @"INSERT INTO Partidas (JugadorId, FechaInicio, SalaActual, FechaFin) VALUES (@jugadorId, @inicio, @sala, @fin);";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Execute(query, new {JugadorId = partida.JugadorId, inicio = partida.FechaInicio, sala = partida.SalaActual, fin = null});
            }

    }

    private static void CreateJugadorId(string nombre)
    {
        string query = "INSERT INTO Jugadores (Nombre) VALUES (@nombre)";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
                cmd.Parameters.AddWithValue("@nombre", nombre);
                conn.Open();
                var id = cmd.ExecuteScalar();
            
        }
    }

    private static int GetJugadorId(string nombre)
    {
        using (var cmd = new SqlCommand("SELECT Id FROM Jugadores WHERE Nombre = @nombre", conn))
        {
            cmd.Parameters.AddWithValue("@nombre", nombre);
            var r = cmd.ExecuteScalar();
            if (r != null) return Convert.ToInt32(r);
        }

    }

    public static void RegistrarSalaCompletada(int partidaId, int sala)
    {
        if (!HasConnection) return;
        try
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var sql = "INSERT INTO PartidaSalas (PartidaId, Sala) VALUES (@pid, @sala);";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", partidaId);
                    cmd.Parameters.AddWithValue("@sala", sala);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch { /* swallow - caller may log */ }
    }

    public static void ActualizarSalaActual(int partidaId, int sala)
    {
        if (!HasConnection) return;
        try
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var sql = "UPDATE Partidas SET SalaActual = @sala WHERE Id = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@sala", sala);
                    cmd.Parameters.AddWithValue("@id", partidaId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch { }
    }

    public static void FinalizarPartida(int partidaId)
    {
        if (!HasConnection) return;
        try
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var sql = "UPDATE Partidas SET FechaFin = @fin WHERE Id = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@fin", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@id", partidaId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch { }
    }

    public static ResultadoViewModel? ObtenerResultados(int partidaId)
    {
        if (!HasConnection) return null;
        var model = new ResultadoViewModel();
        model.PartidaId = partidaId;

        try
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"SELECT j.Nombre, p.FechaInicio, p.FechaFin FROM Partidas p JOIN Jugadores j ON p.JugadorId = j.Id WHERE p.Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", partidaId);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            model.Nombre = r.GetString(0);
                            var fInicio = r.GetDateTime(1);
                            DateTime fFin = r.IsDBNull(2) ? DateTime.UtcNow : r.GetDateTime(2);
                            model.TiempoTotal = fFin - fInicio;
                        }
                    }
                }

                using (var cmd = new SqlCommand(@"SELECT Sala FROM PartidaSalas WHERE PartidaId = @id ORDER BY Sala", conn))
                {
                    cmd.Parameters.AddWithValue("@id", partidaId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read()) model.SalasCompletadas.Add(r.GetInt32(0));
                    }
                }

                using (var cmd = new SqlCommand(@"
                    SELECT j.Nombre, COUNT(ps.Sala) as SalasCompletadas, MIN(DATEDIFF(SECOND, p.FechaInicio, ISNULL(p.FechaFin, SYSUTCDATETIME()))) as TiempoSeg
                    FROM Partidas p
                    JOIN Jugadores j ON p.JugadorId = j.Id
                    LEFT JOIN PartidaSalas ps ON ps.PartidaId = p.Id
                    GROUP BY j.Nombre, p.Id, p.FechaInicio, p.FechaFin
                    ORDER BY SalasCompletadas DESC, TiempoSeg ASC", conn))
                {
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var item = new LeaderboardItem();
                            item.Nombre = r.GetString(0);
                            item.SalasCompletadas = r.IsDBNull(1) ? 0 : r.GetInt32(1);
                            var seg = r.IsDBNull(2) ? 0 : r.GetInt32(2);
                            item.Tiempo = TimeSpan.FromSeconds(seg);
                            model.Leaderboard.Add(item);
                        }
                    }
                }
            }
        }
        catch
        {
            return null;
        }

        return model;
    }
}
