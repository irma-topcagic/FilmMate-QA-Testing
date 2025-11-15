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
      "Repo.Sacuvaj() mora biti pozvan nakon uspješnog ocenjivanja.");

        // 2. Verifikacija logike: da li je ocena filma ažurirana
        var avatarFilm = lazniFilmovi.FirstOrDefault(f => f.getNazivFilma() == "Avatar");

        // Očekivano: (8 + 10) / 2 = 9.0
        Assert.AreEqual(9.0, avatarFilm.getOcjena(), 0.001,
      "Prosječna ocjena se nije ispravno izračunala.");

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
      "Sacuvaj() se ne smije pozvati ako je ocjena izvan raspona.");

       
        StringAssert.Contains(
      consoleOutput.ToString(),
      "Ocjena mora biti u rasponu od 1 do 10."
    );
    }

    [Test]
    public void DodajFilm_ValidInput_AddsFilmAndCallsSave()
    {
        // Simulacija unosa za novi film
        string unos =
            "Novi Film" + Environment.NewLine + 
            "Akcija" + Environment.NewLine +     
            "9" + Environment.NewLine +         
            "2023";                              

        Console.SetIn(new StringReader(unos));

        service.dodajFilm();

        // 1. Provjera: Da li je film dodan u listu (repo.GetAll())
        Assert.AreEqual(3, lazniFilmovi.Count, "Film treba biti dodat u listu.");

        var noviFilm = lazniFilmovi.Last();
        Assert.AreEqual("Novi Film", noviFilm.getNazivFilma(), "Naziv filma se ne podudara.");

        // 2. Verifikacija Mocka: Sacuvaj() mora biti pozvana
        mockRepo.Verify(repo => repo.Sacuvaj(), Times.Once,
            "Sacuvaj() mora biti pozvana nakon uspješnog dodavanja filma.");

        StringAssert.Contains(consoleOutput.ToString(), "Film uspješno dodat!");
    }

    [Test]
    public void DodajFilm_ExistingName_DoesNotAddAndReturnsError()
    {
        // Simulacija unosa za film koji već postoji ("Avatar")
        string unos = "Avatar" + Environment.NewLine;
        Console.SetIn(new StringReader(unos));

        service.dodajFilm();

        // ASSERT
        // 1. Broj filmova NE smije se promijeniti
        Assert.AreEqual(2, lazniFilmovi.Count, "Novi film ne smije biti dodat.");

        // 2. Verifikacija Mocka: Sacuvaj() NE smije biti pozvan
        mockRepo.Verify(repo => repo.Sacuvaj(), Times.Never,
            "Sacuvaj() se ne smije pozvati ako film već postoji.");

        StringAssert.Contains(consoleOutput.ToString(), "Film već postoji!");
    }


    [Test]
    public void ObrisiFilm_ValidNameWithConfirmation_RemovesFilmAndCallsSave()
    {
        string unos ="Titanik\n" +"d\n" +"OBRISI\n";


        Console.SetIn(new StringReader(unos));


        service.obrisiFilm();

        // 1. Broj filmova mora biti smanjen
        Assert.AreEqual(1, lazniFilmovi.Count, "Film 'Titanik' treba biti obrisan.");
        Assert.IsNull(lazniFilmovi.FirstOrDefault(f => f.getNazivFilma() == "Titanik"), "Titanik ne smije biti u listi.");

        // 2. Verifikacija Mocka: Sacuvaj() mora biti pozvan
        mockRepo.Verify(repo => repo.Sacuvaj(), Times.Once,
            "Sacuvaj() mora biti pozvana nakon uspješnog brisanja.");

        StringAssert.Contains(consoleOutput.ToString(), "Film obrisan!");
    }

    [Test]
    public void ObrisiFilm_FilmNotFound_DoesNotCallSaveAndReturnsError()
    {
        // ARRANGE: Simulacija unosa za nepostojeći film
        string unos = "Nepostojeći Film" + Environment.NewLine;
        Console.SetIn(new StringReader(unos));

       
        service.obrisiFilm();

        // ASSERT
        // 1. Broj filmova ostaje isti
        Assert.AreEqual(2, lazniFilmovi.Count, "Nijedan film ne sijme biti obrisan.");

        // 2. Verifikacija Mocka: Sacuvaj() metoda NE smije biti pozvana
        mockRepo.Verify(repo => repo.Sacuvaj(), Times.Never,
            "Sacuvaj() se ne smije pozvati ako film nije pronađen.");

        StringAssert.Contains(consoleOutput.ToString(), "Film nije pronađen!");
    }



    [Test]
    public void AzurirajFilm_UpdateNaziv_UpdatesFilmAndCallsSave()
    {
       
        string unos =
            "Avatar" + Environment.NewLine +
            "1" + Environment.NewLine +
            "Novi Avatar Naziv" + Environment.NewLine;
        Console.SetIn(new StringReader(unos));

       
        service.azurirajFilm();

       //provjera da li se desilo azuriranje
        var film = lazniFilmovi.FirstOrDefault(f => f.getNazivFilma() == "Novi Avatar Naziv");
        Assert.IsNotNull(film, "Film mora biti pronađen pod novim nazivom.");

        // 2. Verifikacija Mocka: Sacuvaj() mora biti pozvana
        mockRepo.Verify(repo => repo.Sacuvaj(), Times.Once,
            "Sacuvaj() mora biti pozvan nakon uspješnog ažuriranja.");

        StringAssert.Contains(consoleOutput.ToString(), "Film ažuriran!");
    }

    [Test]
    public void AzurirajFilm_FilmNotFound_DoesNotCallSaveAndReturnsError()
    {
        //  Simulacija unos - nepostojeći film
        string unos = "Nepostojeći Film" + Environment.NewLine;
        Console.SetIn(new StringReader(unos));

        
        service.azurirajFilm();

        // ASSERT
        // 1. Verifikacija Mocka: Sacuvaj() NE smije biti pozvan
        mockRepo.Verify(repo => repo.Sacuvaj(), Times.Never,
            "Sacuvaj() se ne smije pozvati ako film nije pronađen.");

        // 2. Verifikacija izlaza
        StringAssert.Contains(consoleOutput.ToString(), "Film nije pronađen!");
    }




}