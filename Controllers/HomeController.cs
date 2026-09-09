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

    public IActionResult Introduccion(){
        return View("Sala1");
    }

    public IActionResult Sala1part2(){
        return View("Sala1part2");
    }

    public IActionResult Sala2()
    {
        return View("Sala2");
    }

    public IActionResult Sala3(){
        return View("Sala3");
    }

    public IActionResult Sala4()
    {
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
