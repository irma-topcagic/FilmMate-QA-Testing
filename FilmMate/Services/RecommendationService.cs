using FilmMate.Models;

namespace FilmMate.Services
{
    public class RecommendationService
    {
        public List<Film> RecommendMoviesByGenre(List<Film> films, string zanr)
        {
            if (string.IsNullOrWhiteSpace(zanr))
                throw new ArgumentException("Zanr ne može biti prazan.");

            return films
                .Where(f => f.getKategorija().Equals(zanr, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(f => f.getOcjena())
                .ToList();
        }
    }
}
