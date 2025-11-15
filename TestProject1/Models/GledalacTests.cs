using FilmMate.Data;
using FilmMate.Models;
using FilmMate.Services;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TestProject1.Models
{
    [TestClass]
    public class GledalacTests
    {
        private Mock<IFilmRepository> mockRepo;
        private FilmService filmService;
        private Gledalac gledalac;
        private StringBuilder sb;
        private TextWriter originalConsoleOut;
        private TextReader originalConsoleIn;

        [TestInitialize]
        public void SetUp()
        {
            mockRepo = new Mock<IFilmRepository>();

            // FilmService zahtijeva IFilmRepository u konstruktoru
            filmService = new FilmService(mockRepo.Object);
            gledalac = new Gledalac();

            // Čuvanje originalnih I/O streamova
            originalConsoleOut = Console.Out;
            originalConsoleIn = Console.In;

            // Postavljanje privremenog izlaza konzole
            sb = new StringBuilder();
            Console.SetOut(new StringWriter(sb));

            // Postavljanje mock repozitorija
            var mockFilmovi = new List<Film> {
                new Film("Avatar", "Sci-Fi", 8.0, 2009)
            };
            mockRepo.Setup(r => r.GetAll()).Returns(mockFilmovi);
            mockRepo.Setup(r => r.Sacuvaj());
        }

        [TestCleanup]
        public void TearDown()
        {
            // Vraćanje originalnih I/O streamova
            Console.SetOut(originalConsoleOut);
            Console.SetIn(originalConsoleIn);
        }

        [TestMethod]
        public void PregledajFilmove_Poziva_prikaziFilmove_I_Ispisuje_Listu()
        {
            // Act
            gledalac.pregledajFilmove(filmService);

            // Assert
            // 1. Potvrđujemo da je FilmService pristupio Repozitoriju
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce(),
                "Očekuje se poziv GetAll() iz Repozitorija za prikaz filmova.");

            // 2. Potvrđujemo da je naslov (sa crticama) ispravno ispisan
            string consoleOutput = sb.ToString();
            Assert.IsTrue(consoleOutput.Contains("--- Svi Filmovi u Bazi ---"),
                "Greška u formatiranju naslova 'Svi Filmovi u Bazi'.");

            // 3. Potvrđujemo da je film ispravno ispisan
            Assert.IsTrue(consoleOutput.Contains("Avatar"),
                "Očekivani film 'Avatar' nije pronađen u ispisu.");
        }

      
    }
}