using System.Text.Json;
using PokemonApp.Web.Models;

namespace PokemonApp.Web.Services
{
    public class PokemonService
    {
        private readonly HttpClient _httpClient;

        public PokemonService(HttpClient httpClient) {
            _httpClient = httpClient;
    }

        public async Task<PokemonListResponse> GetPokemonsAsync(int limit = 20, int offset =0)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon?limit={limit}&offset={offset}");
            if (!response.IsSuccessStatusCode) return new PokemonListResponse();

            var jsonString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            return JsonSerializer.Deserialize<PokemonListResponse>(jsonString, options) ?? new PokemonListResponse();
            
        }

        //Método para obtener los tipos de Pokémon desde la Api, para el dropdown
        public async Task<List<string>> GetPokemonTypesAsync()
        {
            var response = await _httpClient.GetAsync("https://pokeapi.co/api/v2/type\"");
            if (!response.IsSuccessStatusCode) return new List<string>();

            var jsonString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonString);
            var types = new List<string>();

            foreach (var element in doc.RootElement.GetProperty("results").EnumerateArray())
            {
                var typeName = element.GetProperty("name").GetString();
                if (!string.IsNullOrEmpty(typeName))
                {
                    types.Add(typeName);
                }    
            }
            return types;
        }
    }

}
