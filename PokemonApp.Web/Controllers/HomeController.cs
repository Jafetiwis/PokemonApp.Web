using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PokemonApp.Web.Models;
using PokemonApp.Web.Services;

namespace PokemonApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly PokemonService _pokemonService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, PokemonService pokemonService)
        {
            _logger = logger;
            _pokemonService = pokemonService;
        }

        public async Task<IActionResult> Index(string? nombre)
        {
            ViewBag.Tipos = await _pokemonService.GetPokemonTypesAsync();

            var response = await _pokemonService.GetPokemonsAsync(20, 0);
            var pokemones = response.Results;

            if (!string.IsNullOrEmpty(nombre))
            {
                pokemones = pokemones.Where(p => p.Name.Contains(nombre.ToLower())).ToList();
            }    

            return View(pokemones);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
