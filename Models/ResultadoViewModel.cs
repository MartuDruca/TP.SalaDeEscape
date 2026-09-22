using System;
using System.Collections.Generic;

public class ResultadoViewModel
{
    public int PartidaId { get; set; }
    public string Nombre { get; set; }
    public TimeSpan TiempoTotal { get; set; }
    public List<int> SalasCompletadas { get; set; } = new List<int>();
    public List<LeaderboardItem> Leaderboard { get; set; } = new List<LeaderboardItem>();
}

public class LeaderboardItem
{
    public string Nombre { get; set; }
    public int SalasCompletadas { get; set; }
    public TimeSpan Tiempo { get; set; }
}
