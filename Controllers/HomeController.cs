using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP_Sala_de_escape.Models;
using Microsoft.Extensions.Configuration;


namespace TP_Sala_de_escape.Controllers;

public class HomeController : Controller
{
    private const string SalaUnlockedKey = "SalaUnlocked";
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
    }

    private int ObtenerSalaDesbloqueada()
    {
        return 3;
    }

    private void GuardarSalaDesbloqueada(int sala)
    {
    }

    private IActionResult? ValidarSala(int salaRequerida)
    {
        return null;
    }

    public IActionResult Index()
    {
        string nombreUsuario = HttpContext.Session.GetString("usuario");

        if (!string.IsNullOrEmpty(nombreUsuario))
        {
            return RedirectToAction("Salas");
        }

        return View();
    }

    public IActionResult Integrantes()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registro(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return RedirectToAction("Index");
        }

        int jugadorId = BD.CrearJugador(nombre);

        int partidaId = BD.CrearPartida(jugadorId);

        HttpContext.Session.SetString("usuario", nombre);

        HttpContext.Session.SetInt32("jugadorId", jugadorId);

        HttpContext.Session.SetInt32("partidaId", partidaId);

        HttpContext.Session.SetInt32("salaActual", 1);

        HttpContext.Session.SetInt32("SalaUnlocked", 1);

        return RedirectToAction("Salas");
    }

    public IActionResult Salas()
    {
        string nombreUsuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            return RedirectToAction("Index");
        }

        return View("salas");
    }

    public IActionResult CompletarSala1()
    {
        HttpContext.Session.SetInt32("salaActual", 2);
        HttpContext.Session.SetInt32("SalaUnlocked", 2);

        int? partidaId = HttpContext.Session.GetInt32("partidaId");
        int? jugadorId = HttpContext.Session.GetInt32("jugadorId");

        if (partidaId.HasValue && jugadorId.HasValue)
        {
            BD.ActualizarSalaActual(partidaId.Value, jugadorId.Value, 2);
        }

        return RedirectToAction("Sala2");
    }

    public IActionResult ResultadoSala2(string respuesta)
    {
        return View("Sala2part2");
    }
    
    public IActionResult CompletarSala2()
    {
        HttpContext.Session.SetInt32("salaActual", 3);
        HttpContext.Session.SetInt32("SalaUnlocked", 3);

        int? partidaId = HttpContext.Session.GetInt32("partidaId");
        int? jugadorId = HttpContext.Session.GetInt32("jugadorId");

        if (partidaId.HasValue && jugadorId.HasValue)
        {
            BD.ActualizarSalaActual(partidaId.Value, jugadorId.Value, 3);
        }

        return RedirectToAction("Sala3");
    }

    public IActionResult CompletarSala3()
    {
        HttpContext.Session.SetInt32("salaActual", 4);
        HttpContext.Session.SetInt32("SalaUnlocked", 4);

        int? partidaId = HttpContext.Session.GetInt32("partidaId");
        int? jugadorId = HttpContext.Session.GetInt32("jugadorId");

        if (partidaId.HasValue && jugadorId.HasValue)
        {
            BD.ActualizarSalaActual(partidaId.Value, jugadorId.Value, 4);
         }

        return RedirectToAction("Sala4");
    }

    public IActionResult Introduccion(){
        return View("Sala1");
    }

    public IActionResult Sala1part2(){
        return View("Sala1part2");
    }

    public IActionResult Sala2()
    {
        var bloqueo = ValidarSala(2);
        if (bloqueo != null)
        {
            return bloqueo;
        }

        return View("Sala2");
    }

    public IActionResult Sala3(){
        var bloqueo = ValidarSala(3);
        if (bloqueo != null)
        {
            return bloqueo;
        }

        return View("Sala3");
    }

    public IActionResult Sala3part2(){
        return View("Sala3part2");
    }

    public IActionResult Sala4()
    {
        return View("Sala4");
    }

    public IActionResult Sala4part2()
    {
        return View("Sala4part2");
    }

    public IActionResult Resultados()
    {
        string nombreUsuario = HttpContext.Session.GetString("usuario");

        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            return RedirectToAction("Index");
        }

        var model = new ResultadoViewModel
        {
            Nombre = nombreUsuario,
            TiempoTotal = TimeSpan.Zero,
            SalasCompletadas = new List<int>(),
            Leaderboard = new List<LeaderboardItem>()
        };

        int? partidaId = HttpContext.Session.GetInt32("partidaId");

        if (partidaId.HasValue)
        {
            var partida = BD.ObtenerPartida(partidaId.Value);
            if (partida != null)
            {
                DateTime inicio = partida.FechaInicio;
                DateTime fin = partida.FechaFin ?? DateTime.Now;
                model.TiempoTotal = fin - inicio;
                model.PartidaId = partida.Id;

                for (int i = 1; i <= partida.SalaActual; i++)
                {
                    model.SalasCompletadas.Add(i);
                }
            }
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult FinalizarPartida()
    {
        int? partidaId = HttpContext.Session.GetInt32("partidaId");

        if (partidaId.HasValue)
        {
            BD.FinalizarPartida(partidaId.Value);
        }

        return RedirectToAction("Resultados");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Cierre()
    {
        int? partidaId = HttpContext.Session.GetInt32("partidaId");

        if (partidaId.HasValue)
        {
            BD.FinalizarPartida(partidaId.Value);
        }

        HttpContext.Session.Clear();

        return View("Cierre");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}



