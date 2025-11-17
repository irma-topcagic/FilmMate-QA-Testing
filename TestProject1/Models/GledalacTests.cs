using FilmMate.Data;
using FilmMate.Models;
using FilmMate.Services;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq; // Dodano za Any()

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

            filmService = new FilmService(mockRepo.Object);
            gledalac = new Gledalac();

            // Čuvanje originalnih I/O streamova
            originalConsoleOut = Console.Out;
            originalConsoleIn = Console.In;

            // Postavljanje privremenog izlaza konzole
            sb = new StringBuilder();
            Console.SetOut(new StringWriter(sb));

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
            // ARRANGE
            var mockFilmovi = new List<Film> {
                new Film("Avatar", "Sci-Fi", 8.0, 2009)
            };
            mockRepo.Setup(r => r.GetAll()).Returns(mockFilmovi);

            Console.SetIn(new StringReader(""));

            // Act
            gledalac.pregledajFilmove(filmService);

            // Assert
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce(),
                "Očekuje se poziv GetAll() iz Repozitorija za prikaz filmova.");

            string consoleOutput = sb.ToString();
            Assert.IsTrue(consoleOutput.Contains("--- Svi Filmovi u Bazi ---"),
                "Greška u formatiranju naslova 'Svi Filmovi u Bazi'.");

            Assert.IsTrue(consoleOutput.Contains("Avatar"),
                "Očekivani film 'Avatar' nije pronađen u ispisu.");
        }

        [TestMethod]
        public void PretragaFilmovi_Poziva_FiltrirajPretraziFilmove_I_Vraca_Pravilan_Rezultat()
        {
            // ARRANGE
            var mockFilmovi = new List<Film>
            {
                new Film("Avatar: Put Vode", "Sci-Fi", 9.0, 2022),
                new Film("Avatar", "Sci-Fi", 8.0, 2009),
                new Film("Titanik", "Romansa", 7.5, 1997)
            };
            mockRepo.Setup(r => r.GetAll()).Returns(mockFilmovi);

            // Simulirani unos:
            // 1. Odabir opcije "1" (Pretraga po Nazivu)
            // 2. Unos dijela naziva ("Avatar")
            // 3. Unos "0" (Izlazak iz petlje menija u FilmService)
            string simulatedInput = "1\n" + "Avatar\n" + "0\n";

            // Postavljanje privremenog ulaza konzole
            Console.SetIn(new StringReader(simulatedInput));

            // Act
            gledalac.pretragaFilmova(filmService);

            // Assert
            string consoleOutput = sb.ToString();

            Assert.IsTrue(consoleOutput.Contains("--- Filtriranje i Pretraga Filmova ---"),
                "Naslov menija za pretragu nije pronađen ili je pogrešno formatiran.");

            Assert.IsTrue(consoleOutput.Contains("Avatar: Put Vode"));
            Assert.IsTrue(consoleOutput.Contains("Avatar"));
            Assert.IsFalse(consoleOutput.Contains("Titanik"));

            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }
    }
}