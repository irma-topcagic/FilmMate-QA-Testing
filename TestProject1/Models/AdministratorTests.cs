using FilmMate.Models;
using FilmMate.Services;
using Moq;
using System.Text;
using System.IO;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FilmMate.Services;



[TestClass]
public class AdministratorTests
{
    private Mock<IFilmService> mockFilmService;
    private Administrator admin;
    private StringBuilder sb;

    [TestInitialize]
    public void SetUp()
    {
        mockFilmService = new Mock<IFilmService>();
        admin = new Administrator();

        sb = new StringBuilder();
        Console.SetOut(new StringWriter(sb));
    }

    [TestCleanup]
    public void TearDown()
    {
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
    }

    [TestMethod]
    public void DodajFilm_PozivaMetoduNaServisu()
    {
        string input = "NoviFilm" + Environment.NewLine + "Akcija" + Environment.NewLine + "8" + Environment.NewLine + "2020" + Environment.NewLine;
        Console.SetIn(new StringReader(input));

        admin.dodajFilm(mockFilmService.Object);

        mockFilmService.Verify(s => s.dodajFilm(), Times.Once);
    }

    [TestMethod]
    public void ObrisiFilm_PozivaMetoduNaServisu()
    {
        // Akcija
        admin.obrisiFilm(mockFilmService.Object);

        // Verifikacija
        mockFilmService.Verify(s => s.obrisiFilm(), Times.Once);
    }

    [TestMethod]
    public void AzurirajFilm_PozivaMetoduNaServisu()
    {
        // Akcija
        admin.azurirajFilm(mockFilmService.Object);

        // Verifikacija
        mockFilmService.Verify(s => s.azurirajFilm(), Times.Once);
    }
}