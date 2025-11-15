using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

using FilmMate.Models;
using FilmMate.Data;
using FilmMate.Services;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Microsoft.VisualStudio.TestTools.UnitTesting;


[TestFixture]
public class FilmServiceRateTests
{
    private Mock<IFilmRepository> mockRepo;
    private FilmService service;
    private List<Film> lazniFilmovi;
    private StringWriter consoleOutput; // Za hvatanje Console.WriteLine

    [SetUp]
    public void Setup()
    {
        // Inicijalizacija lažnih podataka
        lazniFilmovi = new List<Film>
    {
            
            new Film("Avatar", "Sci-Fi", 8.0, 2009),
      new Film("Titanik", "Romansa", 7.8, 1997)
    };

        mockRepo = new Mock<IFilmRepository>();

       
        mockRepo.Setup(repo => repo.GetAll())
        .Returns(lazniFilmovi);

        service = new FilmService(mockRepo.Object);

        //StringWriter sluzii za hvatanje izlaza
        consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
    }

    // ciscenje nakon svakog testa -- kao clear nesto
    [TearDown]
    public void TearDown()
    {
        // Vraćamo Console.Out i Console.In na originalni tok
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        Console.SetIn(new StreamReader(Console.OpenStandardInput()));
    }

    // Test: Uspješnost ocjenjivanja filma
    [Test]
    public void OcijeniFilmGledaoca_ValidRating_CallsSaveAndUpdatesRating()
    {
        // Simulacija unosa sa konzole
        string unos =
      "Avatar" + Environment.NewLine + 
            "10";

        var stringReader = new StringReader(unos);
        Console.SetIn(stringReader);

     
        service.OcijeniFilmGledaoca();

   

        // 1. Verifikacija Mocka: da li je Save() pozvan uopste
        mockRepo.Verify(repo => repo.Sacuvaj(), Times.Once,
      "Repo.Sacuvaj() mora biti pozvan nakon uspešnog ocenjivanja.");

        // 2. Verifikacija logike: da li je ocena filma ažurirana
        var avatarFilm = lazniFilmovi.FirstOrDefault(f => f.getNazivFilma() == "Avatar");

        // Očekivano: (8 + 10) / 2 = 9.0
        Assert.AreEqual(9.0, avatarFilm.getOcjena(), 0.001,
      "Prosječna ocena se nije ispravno izračunala.");

        StringAssert.Contains(
      consoleOutput.ToString(),
      "Uspješno ste ocijenili film."
    );
    }

    // Test: Sacuvaj() metoda se NE poziva za neispravnu ocenu
    [Test]
    public void OcijeniFilmGledaoca_InvalidRating_DoesNotCallSave()
    {
        string unos =
      "Titanik" + Environment.NewLine +
            "12";                            

        var stringReader = new StringReader(unos);
        Console.SetIn(stringReader);

       
        service.OcijeniFilmGledaoca();

      

       
        mockRepo.Verify(repo => repo.Sacuvaj(), Times.Never,
      "Sacuvaj() se ne sme pozvati ako je ocena izvan raspona.");

       
        StringAssert.Contains(
      consoleOutput.ToString(),
      "Ocjena mora biti u rasponu od 1 do 10."
    );
    }
}