using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP_Sala_de_escape.Models;


namespace TP_Sala_de_escape.Controllers;

public class HomeController : Controller
{
    private const string SalaUnlockedKey = "SalaUnlocked";
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

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

    public IActionResult Registro(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return RedirectToAction("Index");
        }

        HttpContext.Session.SetString("usuario", nombre);
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
        return RedirectToAction("Sala2");
    }

    public IActionResult ResultadoSala2(string respuesta)
    {
        return View("Sala2part2");
    }
    public IActionResult CompletarSala2()
    {
        GuardarSalaDesbloqueada(3);
        return RedirectToAction("Sala3");
    }

    public IActionResult CompletarSala3()
    {
        GuardarSalaDesbloqueada(4);
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

    public IActionResult Sala4()
    {
        var bloqueo = ValidarSala(4);
        if (bloqueo != null)
        {
            return bloqueo;
        }

        return View("Sala4");
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
