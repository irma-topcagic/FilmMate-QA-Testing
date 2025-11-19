using FilmMate.Models;

namespace FilmMate.Services
{
    public class RatingService
    {
        public double CalculateAverageRating(Film film)
        {
            var ocjene = film.getOcjene();

            if (ocjene.Any(o => o < 1 || o > 10))
                throw new ArgumentException("Ocjena mora biti u rasponu 1–10.");

            if (ocjene.Count == 0)
                return 0;

            return ocjene.Average();
        }
    }
}
