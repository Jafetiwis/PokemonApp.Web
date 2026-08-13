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

        public async Task<IActionResult> Index(string? nombre, string? tipo, int offset = 0)
        {
            ViewBag.Tipos = await _pokemonService.GetPokemonTypesAsync();

            ViewBag.Offset = offset;
            ViewBag.Nombre = nombre;
            ViewBag.Tipo = tipo;

            List<PokemonDto> pokemones = new List<PokemonDto>();

            if (!string.IsNullOrEmpty(nombre))
            {
                var response = await _pokemonService.GetPokemonsAsync(1500, 0);
                pokemones = response.Results;

                pokemones = pokemones.Where(p => p.Name.Contains(nombre.ToLower())).ToList();

                pokemones = pokemones.Skip(offset).Take(20).ToList();
            }
            else
            {
                var response = await _pokemonService.GetPokemonsAsync(20, offset);
                pokemones = response.Results;
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
