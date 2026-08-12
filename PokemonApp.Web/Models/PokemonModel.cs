namespace PokemonApp.Web.Models
{
    public class PokemonListResponse
    {
        public int Count { get; set; }
        public string Next {  get; set; }
        public string previous { get; set; }
        public List<PokemonDto> Results { get; set; }
    }

    public class PokemonDto
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public int Id
        {
            get
            {
                if (string.IsNullOrEmpty(Url)) return 0;
                var segments = Url.TrimEnd('/').Split('/');
                return int.TryParse(segments[^1], out var id) ? id : 0;
            }
        }
        
        public string ImageUrl => $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{Id}.png";
        public string Type {  get; set; }
    }
}
