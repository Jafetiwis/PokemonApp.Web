using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PokemonApp.Web.Models;
using PokemonApp.Web.Services;
using ClosedXML.Excel;

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

        public async Task<IActionResult> ExportarExcel(string? nombre, string? tipo, int offset = 0)
        {
            List<PokemonDto> pokemones = new List<PokemonDto>();

            if (!string.IsNullOrEmpty(nombre))
            {
                var response = await _pokemonService.GetPokemonsAsync(1500, 0);
                pokemones = response.Results.Where(p => p.Name.Contains(nombre.ToLower())).Skip(offset).Take(20).ToList();
            }
            else
            {
                var response = await _pokemonService.GetPokemonsAsync(20, offset);
                pokemones = response.Results;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Pokédex");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Número (ID)";
                worksheet.Cell(currentRow, 2).Value = "Nombre del Pokémon";
                worksheet.Cell(currentRow, 1).Value = "Enlace de Imagen";
                worksheet.Row(1).Style.Font.Bold = true;

                foreach (var pokemon in pokemones)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = pokemon.Id;
                    worksheet.Cell(currentRow, 2).Value = char.ToUpper(pokemon.Name[0]) + pokemon.Name.Substring(1);
                    worksheet.Cell(currentRow, 3).Value = pokemon.ImageUrl;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Pokemones_Filtrados.xlsx");
                }
            }
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
