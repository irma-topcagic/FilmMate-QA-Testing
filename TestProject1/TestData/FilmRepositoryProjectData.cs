using FilmMate.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1.TestData
{
    public static class FilmRepositoryTestExtensions
    {
        // Pomoćna metoda za postavljanje privatnog polja 'filePath' pomoću refleksije
        public static void SetFilePath(this FilmRepository repo, string path)
        {
            // Koristimo GetField za pristup privatnom polju 'filePath'
            typeof(FilmRepository)
                .GetField("filePath", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(repo, path);
        }

        // Pomoćna metoda za pozivanje privatne metode 'UcitajFilmove'
        public static void InvokeLoad(this FilmRepository repo)
        {
            // Koristimo GetMethod za pristup privatnoj metodi 'UcitajFilmove'
            typeof(FilmRepository)
                .GetMethod("UcitajFilmove", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(repo, null);
        }
    }
}
