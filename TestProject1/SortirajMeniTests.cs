using FilmMate.Data;
using FilmMate.Models;
using FilmMate.Services;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        }

        [TestMethod]
        public void PrikaziSortirajMeni_Option0_ExitsMenu()
        {
            using var input = new StringReader("0\n");
            using var output = new StringWriter();
            SortirajMeni.PrikaziSortirajMeni(filmService, input, output);
            Assert.IsTrue(output.ToString().Contains("Sortiranje"));
        }

        [TestMethod]
        public void PrikaziSortirajMeni_Option1Ascending_Works()
        {
            using var input = new StringReader("1\nA\n0\n");
            using var output = new StringWriter();
           SortirajMeni.PrikaziSortirajMeni(filmService, input, output);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        [TestMethod]
        public void PrikaziSortirajMeni_Option1Descending_Works()
        {
            using var input = new StringReader("1\nD\n0\n");
            using var output = new StringWriter();
            SortirajMeni.PrikaziSortirajMeni(filmService, input, output);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        [TestMethod]
        public void PrikaziSortirajMeni_Option2Ascending_Works()
        {
            using var input = new StringReader("2\nA\n0\n");
            using var output = new StringWriter();
           SortirajMeni.PrikaziSortirajMeni(filmService, input, output);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        [TestMethod]
        public void PrikaziSortirajMeni_Option2Descending_Works()
        {
            using var input = new StringReader("2\nD\n0\n");
            using var output = new StringWriter();
           SortirajMeni.PrikaziSortirajMeni(filmService, input, output);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        [TestMethod]
        public void PrikaziSortirajMeni_Option3Ascending_Works()
        {
            using var input = new StringReader("3\nA\n0\n");
            using var output = new StringWriter();
           SortirajMeni.PrikaziSortirajMeni(filmService, input, output);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        [TestMethod]
        public void PrikaziSortirajMeni_Option3Descending_Works()
        {
            using var input = new StringReader("3\nD\n0\n");
            using var output = new StringWriter();
          SortirajMeni.PrikaziSortirajMeni(filmService, input, output);
            mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce());
        }

        [TestMethod]
        public void PrikaziSortirajMeni_InvalidDirection_ShowsError()
        {
            using var input = new StringReader("1\nX\n0\n");
            using var output = new StringWriter();
            SortirajMeni.PrikaziSortirajMeni(filmService, input, output);
            Assert.IsTrue(output.ToString().Contains("Neispravan odabir smjera"));
        }

        [TestMethod]
        public void PrikaziSortirajMeni_InvalidOption_ShowsError()
        {
            using var input = new StringReader("5\n0\n");
            using var output = new StringWriter();
           SortirajMeni.PrikaziSortirajMeni(filmService, input, output);
            Assert.IsTrue(output.ToString().Contains("Pogrešan unos!"));
        }
    }
}
