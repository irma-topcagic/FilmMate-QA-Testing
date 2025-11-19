using FilmMate.Data;
using FilmMate.Models;
using FilmMate.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace TestProject1.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private string tempFile = "";
        private UserRepository userRepo;

        private string GenerisiHash(string lozinka)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(lozinka));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        [TestInitialize]
        public void Setup()
        {
            tempFile = Path.GetTempFileName();
            userRepo = new UserRepository(tempFile);
        }

        [TestMethod]
        public void Registracija_NewUser_SavesUserAndReturnsSuccess()
        {
            const string testUsername = "novikorisnik";
            const string testPassword = "lozinka123";
            string expectedHash = GenerisiHash(testPassword);

            var input = new StringReader($"{testUsername}{Environment.NewLine}{testPassword}");
            var output = new StringWriter();

            var service = new UserService(userRepo);
            service.Registracija(input, output);

            var allUsers = userRepo.GetAll();
            Assert.AreEqual(1, allUsers.Count);
            var savedUser = allUsers.First();
            Assert.AreEqual(testUsername, savedUser.getKorisnickoIme());
            Assert.AreEqual(expectedHash, savedUser.getLozinka());
            Assert.IsInstanceOfType(savedUser, typeof(Gledalac));
            Assert.IsTrue(output.ToString().Contains("Registracija uspješna!"));
        }

        [TestMethod]
        public void Registracija_ExistingUser_DoesNotSaveAndReturnsError()
        {
            const string existingUsername = "admin_test";

            var admin = new Administrator();
            admin.setKorisnickoIme(existingUsername);
            admin.setLozinka(GenerisiHash("pass"));
            userRepo.GetAll().Add(admin);
            userRepo.Sacuvaj();

            var input = new StringReader($"{existingUsername}{Environment.NewLine}bilo_koja_lozinka");
            var output = new StringWriter();

            var service = new UserService(userRepo);
            service.Registracija(input, output);

            Assert.AreEqual(1, userRepo.GetAll().Count);
            Assert.IsTrue(output.ToString().Contains("Korisnik već postoji!"));
        }

        [TestMethod]
        public void Prijava_ValidCredentials_ReturnsKorisnikObject()
        {
            const string validUsername = "ValidUser";
            const string validPassword = "Password1";
            string validHash = GenerisiHash(validPassword);

            var gledalac = new Gledalac();
            gledalac.setKorisnickoIme(validUsername);
            gledalac.setLozinka(validHash);
            userRepo.GetAll().Add(gledalac);
            userRepo.Sacuvaj();

            var input = new StringReader($"{validUsername}{Environment.NewLine}{validPassword}");
            var output = new StringWriter();

            var service = new UserService(userRepo);
            Korisnik? result = service.Prijava(input, output);

            Assert.IsNotNull(result);
            Assert.AreEqual(validUsername, result.getKorisnickoIme());
            Assert.IsInstanceOfType(result, typeof(Gledalac));
            Assert.IsTrue(output.ToString().Contains($"Dobrodošao {validUsername}"));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
