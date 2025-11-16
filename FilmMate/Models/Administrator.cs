using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FilmMate.Services;

namespace FilmMate.Models
{
    public class Administrator : Korisnik
    {
        public void dodajFilm(IFilmService filmService) => filmService.dodajFilm();
        public void obrisiFilm(IFilmService filmService) => filmService.obrisiFilm();
        public void azurirajFilm(IFilmService filmService) => filmService.azurirajFilm();
    }
}

