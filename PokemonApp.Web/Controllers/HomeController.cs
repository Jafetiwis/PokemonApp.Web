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

        public async Task<IActionResult> Index()
        {
            var response = await _pokemonService.GetPokemonsAsync(20, 0);


            return View(response.Results);
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
