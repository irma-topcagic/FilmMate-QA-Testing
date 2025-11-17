// ============================================
// SortirajMeniTests.cs - Minimalni testovi za 100% pokrivenost
// ============================================
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FilmMate.Services;
using FilmMate.Data;
using FilmMate.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;

namespace TestProject1.UI
{
    [TestClass]
    public class SortirajMeniTests
    {
        private Mock<IFilmRepository> mockRepo;
        private FilmService filmService;
        private StringWriter consoleOutput;

        [TestInitialize]
        public void Setup()
        {
            var testFilmovi = new List<Film>
            {
                new Film("Avatar", "Sci-Fi", 8.0, 2009),
                new Film("Titanic", "Romance", 7.8, 1997)
            };

            mockRepo = new Mock<IFilmRepository>();
            mockRepo.Setup(r => r.GetAll()).Returns(testFilmovi);

            filmService = new FilmService(mockRepo.Object);
            consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));
        }

        // TEST 1: Opcija 0 - Izlaz
        [TestMethod]
        public void PrikaziSortirajMeni_Option0_ExitsMenu()
        {
            Console.SetIn(new StringReader("0\n"));
            SortirajMeni.PrikaziSortirajMeni(filmService);
            Assert.IsTrue(consoleOutput.ToString().Contains("Sortiranje"));
        }

        // TEST 2: Opcija 1 + A - Sortira po ocjeni rastuće
        [TestMethod]
        public void PrikaziSortirajMeni_Option1Ascending_Works()
        {
            Console.SetIn(new StringReader("1\nA\n0\n"));
            SortirajMeni.PrikaziSortirajMeni(filmService);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        // TEST 3: Opcija 1 + D - Sortira po ocjeni opadajuće
        [TestMethod]
        public void PrikaziSortirajMeni_Option1Descending_Works()
        {
            Console.SetIn(new StringReader("1\nD\n0\n"));
            SortirajMeni.PrikaziSortirajMeni(filmService);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        // TEST 4: Opcija 2 + A - Sortira po godini rastuće
        [TestMethod]
        public void PrikaziSortirajMeni_Option2Ascending_Works()
        {
            Console.SetIn(new StringReader("2\nA\n0\n"));
            SortirajMeni.PrikaziSortirajMeni(filmService);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        // TEST 5: Opcija 2 + D - Sortira po godini opadajuće
        [TestMethod]
        public void PrikaziSortirajMeni_Option2Descending_Works()
        {
            Console.SetIn(new StringReader("2\nD\n0\n"));
            SortirajMeni.PrikaziSortirajMeni(filmService);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        // TEST 6: Opcija 3 + A - Sortira po nazivu rastuće
        [TestMethod]
        public void PrikaziSortirajMeni_Option3Ascending_Works()
        {
            Console.SetIn(new StringReader("3\nA\n0\n"));
            SortirajMeni.PrikaziSortirajMeni(filmService);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        // TEST 7: Opcija 3 + D - Sortira po nazivu opadajuće
        [TestMethod]
        public void PrikaziSortirajMeni_Option3Descending_Works()
        {
            Console.SetIn(new StringReader("3\nD\n0\n"));
            SortirajMeni.PrikaziSortirajMeni(filmService);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        // TEST 8: Neispravan smjer - Prikazuje grešku
        [TestMethod]
        public void PrikaziSortirajMeni_InvalidDirection_ShowsError()
        {
            Console.SetIn(new StringReader("1\nX\n0\n"));
            SortirajMeni.PrikaziSortirajMeni(filmService);
            Assert.IsTrue(consoleOutput.ToString().Contains("Neispravan odabir smjera"));
        }

        // TEST 9: Nevalidna opcija - Prikazuje grešku
        [TestMethod]
        public void PrikaziSortirajMeni_InvalidOption_ShowsError()
        {
            Console.SetIn(new StringReader("5\n0\n"));
            SortirajMeni.PrikaziSortirajMeni(filmService);
            Assert.IsTrue(consoleOutput.ToString().Contains("Pogrešan unos!"));
        }
    }
}