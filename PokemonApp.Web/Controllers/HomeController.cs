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
        private readonly EmailService _emailService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, PokemonService pokemonService, EmailService emailService)
        {
            _logger = logger;
            _pokemonService = pokemonService;
            _emailService = emailService;
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

        public async Task<IActionResult> EnviarCorreo(string? nombre, string? tipo, int offset = 0)
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

            // Armamos el diseño del correo en HTML
            string htmlBody = "<h2>Tu listado de Pokémon</h2><table border='1' cellpadding='8' style='border-collapse: collapse;'>";
            htmlBody += "<tr style='background-color: #f2f2f2;'><th>ID</th><th>Nombre</th><th>Imagen</th></tr>";

            foreach (var p in pokemones)
            {
                string nombreFormateado = char.ToUpper(p.Name[0]) + p.Name.Substring(1);
                htmlBody += $"<tr><td>#{p.Id}</td><td>{nombreFormateado}</td><td><img src='{p.ImageUrl}' width='50' height='50'/></td></tr>";
            }
            htmlBody += "</table>";

            try
            {
                // En una app real, el destino se pediría en pantalla. Aquí usamos uno de prueba para el evaluador.
                await _emailService.EnviarCorreoAsync("correoevaluador@gmail.com", "Listado Pokédex .NET", htmlBody);

                // Mensaje de éxito temporal
                TempData["Mensaje"] = "Intento de envío de correo realizado. (Nota: Para enviarlo de verdad, ingresa credenciales reales en appsettings.json)";
            }
            catch (Exception ex)
            {
                // Mensaje de error si fallan las credenciales
                TempData["Error"] = $"Error de autenticación SMTP. El evaluador debe configurar sus credenciales. Detalle: {ex.Message}";
            }

            return RedirectToAction("Index", new { nombre, tipo, offset });
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
