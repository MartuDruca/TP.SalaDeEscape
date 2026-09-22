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
        // inicializar helper de BD con la cadena de conexión
        try { BD.Initialize(configuration); } catch { }
    }
    
    [HttpPost]

    private int ObtenerSalaDesbloqueada()
    {
        return 3;
    }

    private void GuardarSalaDesbloqueada(int sala)
    {
        HttpContext.Session.SetInt32(SalaUnlockedKey, int.MaxValue);
    }

    private IActionResult? ValidarSala(int salaRequerida)
    {
        if (ObtenerSalaDesbloqueada() < salaRequerida)
        {
            return RedirectToAction("Salas");
        }

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

    public IActionResult Registro(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return RedirectToAction("Index");
        }

        HttpContext.Session.SetString("usuario", nombre);

        // iniciar y guardar partida en session + DB (si está configurada)
        HttpContext.Session.SetString("inicioPartida", DateTime.UtcNow.ToString("o"));
        HttpContext.Session.SetInt32("salaActual", 1);

        try
        {
            int partidaId = BD.CrearPartida(nombre);
            if (partidaId > 0)
            {
                HttpContext.Session.SetInt32("partidaId", partidaId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo crear la partida en la base (revisar cadena de conexión).");
        }

        return RedirectToAction("Index");
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
        GuardarSalaDesbloqueada(2);
        var pid = HttpContext.Session.GetInt32("partidaId");
        if (pid.HasValue)
        {
            BD.RegistrarSalaCompletada(pid.Value, 1);
            BD.ActualizarSalaActual(pid.Value, 2);
        }
        return RedirectToAction("Sala2");
    }

    public IActionResult ResultadoSala2(string respuesta)
    {
        return View("Sala2part2");
    }
    public IActionResult CompletarSala2()
    {
        GuardarSalaDesbloqueada(3);
        var pid = HttpContext.Session.GetInt32("partidaId");
        if (pid.HasValue)
        {
            BD.RegistrarSalaCompletada(pid.Value, 2);
            BD.ActualizarSalaActual(pid.Value, 3);
        }
        return RedirectToAction("Sala3");
    }

    public IActionResult CompletarSala3()
    {
        GuardarSalaDesbloqueada(5);
        var pid = HttpContext.Session.GetInt32("partidaId");
        if (pid.HasValue)
        {
            BD.RegistrarSalaCompletada(pid.Value, 3);
            BD.ActualizarSalaActual(pid.Value, 4);
        }
        return RedirectToAction("Sala4");
    }

    [HttpPost]
    public IActionResult ActualizarSala(int sala)
    {
        var pid = HttpContext.Session.GetInt32("partidaId");
        if (pid.HasValue) BD.ActualizarSalaActual(pid.Value, sala);
        return Ok();
    }

    [HttpPost]
    public IActionResult TerminarPartida()
    {
        // marcar fin de partida y mostrar resultados
        int? pid = HttpContext.Session.GetInt32("partidaId");
        if (!pid.HasValue)
        {
            return RedirectToAction("Index");
        }

        try { BD.FinalizarPartida(pid.Value); } catch { }

        var model = BD.ObtenerResultados(pid.Value);
        if (model == null)
        {
            model = new ResultadoViewModel();
            model.PartidaId = pid.Value;
            model.Nombre = HttpContext.Session.GetString("usuario") ?? "---";
            var inicio = DateTime.Parse(HttpContext.Session.GetString("inicioPartida") ?? DateTime.UtcNow.ToString("o"));
            model.TiempoTotal = DateTime.UtcNow - inicio;
            int? s = HttpContext.Session.GetInt32("salaActual");
            if (s.HasValue) for (int i = 1; i < s.Value; i++) model.SalasCompletadas.Add(i);
        }

        return View("Resultados", model);
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

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Cierre()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}



