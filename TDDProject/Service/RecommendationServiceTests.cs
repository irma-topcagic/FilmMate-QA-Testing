using FilmMate.Services;
using FilmMate.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestProject1.Services
{
    [TestClass]
    public class RecommendationServiceTests
    {
        // TEST 1: Vraća samo filmove traženog žanra
        [TestMethod]
        public void RecommendMoviesByGenre_ReturnsOnlyMatchingGenre()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film("A", "Action", 9.0, 2010),
                new Film("B", "Drama", 5.0, 2005)
            };

            var service = new RecommendationService();

            // Act
            var result = service.RecommendMoviesByGenre(films, "Action");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("A", result.First().getNazivFilma());
        }

        // TEST 2: Sortira po prosječnoj ocjeni opadajuće
        [TestMethod]
        public void RecommendMoviesByGenre_SortsByRatingDescending()
        {
            // Arrange
            var film1 = new Film("A", "Action", 5.0, 2010);
            var film2 = new Film("B", "Action", 10.0, 2012);

            var films = new List<Film> { film1, film2 };

            var service = new RecommendationService();

            // Act
            var result = service.RecommendMoviesByGenre(films, "Action");

            // Assert
            Assert.AreEqual("B", result.First().getNazivFilma());
        }

        // TEST 3: Nevalidan žanr baca izuzetak
        [TestMethod]
        public void RecommendMoviesByGenre_InvalidGenre_ThrowsException()
        {
            // Arrange
            var service = new RecommendationService();

            // Act + Assert
            Assert.ThrowsException<ArgumentException>(() =>
                service.RecommendMoviesByGenre(new List<Film>(), "")
            );
        }
    }
}
