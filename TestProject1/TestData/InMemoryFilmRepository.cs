using FilmMate.Data;
using FilmMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1.TestData
{
    public class InMemoryFilmRepository : IFilmRepository
    {
        private List<Film> filmovi;

 
        public InMemoryFilmRepository(List<Film> initialFilms = null)
        {
       
            filmovi = initialFilms?.Select(f => f).ToList() ?? new List<Film>();
        }

        public List<Film> GetAll() => filmovi;

        
        public void Sacuvaj()
        {
            // Nema fajl I/O. Podaci ostaju u 'filmovi' listi u memoriji.
        }
    }
}
