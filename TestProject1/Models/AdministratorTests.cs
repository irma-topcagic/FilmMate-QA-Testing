using FilmMate.Models;
using FilmMate.Services; // Dodajte using
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1.Models
{
    [TestClass]
    public class AdministratorTests
    {
        // Koristimo IFilmService, ne Mock<FilmService>!
        private Mock<IFilmService> mockFilmService;
        private Administrator admin;

        [TestInitialize]
        public void SetUp()
        {
            // Sada Mock-ujemo INTERFEJS, a ne klasu
            mockFilmService = new Mock<IFilmService>();
            admin = new Administrator();
        }

        // Nema potrebe za TearDown i Console.SetOut/In jer više ne testiramo I/O!

        [TestMethod]
        public void DodajFilm_Poziva_FilmService_dodajFilm()
        {
            // Act
            // Prosljeđujemo Mock objekat. Metode se ne izvršavaju, samo se prati poziv.
            admin.dodajFilm(mockFilmService.Object);

            // Assert
            // Verifikujemo da je metoda FilmService.dodajFilm() pozvana tačno jednom
            mockFilmService.Verify(fs => fs.dodajFilm(), Times.Once(),
                "Administrator.dodajFilm treba da pozove FilmService.dodajFilm.");
        }

        [TestMethod]
        public void ObrisiFilm_Poziva_FilmService_obrisiFilm()
        {
            // Act
            admin.obrisiFilm(mockFilmService.Object);

            // Assert
            mockFilmService.Verify(fs => fs.obrisiFilm(), Times.Once(),
                "Administrator.obrisiFilm treba da pozove FilmService.obrisiFilm.");
        }

        [TestMethod]
        public void AzurirajFilm_Poziva_FilmService_azurirajFilm()
        {
            // Act
            admin.azurirajFilm(mockFilmService.Object);

            // Assert
            mockFilmService.Verify(fs => fs.azurirajFilm(), Times.Once(),
                "Administrator.azurirajFilm treba da pozove FilmService.azurirajFilm.");
        }
    }
}