using FilmMate.Services;
using FilmMate.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestProject1.Services
{
    [TestClass]
    public class RatingServiceTests
    {
        // TEST 1: Validan prosjek za film koji ima ocjene
        [TestMethod]
        public void CalculateAverageRating_ReturnsCorrectAverage()
        {
            // Arrange
            var film = new Film("Avatar", "Action", 8.0, 2009);
            film.DodajOcjenu(6);
            film.DodajOcjenu(10);

            var service = new RatingService();

            // Act
            double avg = service.CalculateAverageRating(film);

            // Assert
            Assert.AreEqual(8.0, avg);
        }

        // TEST 2: Prazna lista ocjena - rezultat je 0
        [TestMethod]
        public void CalculateAverageRating_NoRatings_ReturnsZero()
        {
            // Arrange
            var film = new Film("Test", "Drama", 0, 2000);
            // nije dodana nijedna validna ocjena

            var service = new RatingService();

            // Act
            double avg = service.CalculateAverageRating(film);

            // Assert
            Assert.AreEqual(0, avg);
        }

        // TEST 3: Nevalidna ocjena (van raspona 1-10) baca exception
        [TestMethod]
        public void CalculateAverageRating_InvalidRating_ThrowsException()
        {
            
            var film = new Film("Invalid", "Sci-Fi", 5.0, 2020);
            var service = new RatingService();

            
            Assert.ThrowsException<ArgumentException>(() =>
                film.DodajOcjenu(12)  
            );
        }

    }
}
