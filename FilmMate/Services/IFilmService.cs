using FilmMate.Models;
using System.Collections.Generic;

namespace FilmMate.Services
{
    public interface IFilmService
    {
        // Metode za Administratora
        void dodajFilm();
        void obrisiFilm();
        void azurirajFilm();

        // Metode za Gledaoca i opći prikaz/pretragu
        void prikaziFilmove();
        void FiltrirajPretraziFilmove();
        void OcijeniFilmGledaoca();
        void SortirajPoOcjeni(bool smjerRastuci);
        void SortirajPoGodini(bool smjerRastuci);
        void SortirajPoNazivu(bool smjerRastuci);
    }
}