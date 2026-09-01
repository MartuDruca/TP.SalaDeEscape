using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP_Sala_de_escape.Models;

namespace TP_Sala_de_escape.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if(HttpContext.Session.GetString("usuario") != null){
            return Salas();
        }
        string nombreUsuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            return RedirectToAction("Index");
        }
        return View();
    }

    Jugador jugador = new Jugador(HttpContext.Session.GetString("usuario"));

    public IActionResult Salas()
    {
        switch (jugador.SalaActual)
        {
            case 1:
                return RedirectToAction("Sala1", "Juego");
            case 2:
                return RedirectToAction("Sala2", "Juego");
            case 3:
                return RedirectToAction("Sala3", "Juego");
            
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Cierre(){
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
