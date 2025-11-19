using Microsoft.VisualStudio.TestTools.UnitTesting;
using FilmMate.Data;
using FilmMate.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace TestProject1.Data
{
    [TestClass]
 
    public class UserRepositoryTests
    {
        private string originalFilePath = "korisnici.txt";
        private string backupFilePath = "korisnici_backup.txt";

       
        private string _testFilePath;
        private static int _testCounter = 0;

        
        private void ForceDeleteFile(string filePath)
        {
           
            if (File.Exists(filePath))
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        File.Delete(filePath);
                        return;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(50);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return;
                    }
                }
            }
        }

        [TestInitialize]
        public void Setup()
        {
           
            int currentTestId = Interlocked.Increment(ref _testCounter);
            _testFilePath = $"test_korisnici_{currentTestId}_{DateTime.Now.Ticks}.txt";

            
            ForceDeleteFile(originalFilePath);

           
            if (File.Exists(originalFilePath) && !File.Exists(backupFilePath))
            {
                try
                {
                    File.Copy(originalFilePath, backupFilePath, true);
                }
                catch { /* ignorisanje grešaka kopiranja */ }
            }

            ForceDeleteFile(_testFilePath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            ForceDeleteFile(_testFilePath);

            if (File.Exists(backupFilePath))
            {
                try
                {
                    // Prvo oslobodi originalni fajl, pa ga prepiši
                    ForceDeleteFile(originalFilePath);
                    File.Copy(backupFilePath, originalFilePath, true);
                    ForceDeleteFile(backupFilePath);
                }
                catch { }
            }

        }

        private void SetPrivateFilePath(UserRepository repo, string newPath)
        {
            
            var field = typeof(UserRepository).GetField("filePath",
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(repo, newPath);
        }

        private void InvokePrivateUcitaj(UserRepository repo)
        {
            var method = typeof(UserRepository).GetMethod("Ucitaj",
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(repo, null);
        }

        
        [TestMethod]
        public void Constructor_FileDoesNotExist_CreatesEmptyList()
        {
           
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath); 

            Assert.IsNotNull(repo);
            Assert.AreEqual(0, repo.GetAll().Count, "Lista korisnika treba biti prazna kad fajl ne postoji.");
        }

       
        [TestMethod]
        public void Constructor_EmptyFile_CreatesEmptyList()
        {
            
            File.WriteAllText(_testFilePath, "");

            var repo = new UserRepository(_testFilePath); 

            Assert.AreEqual(0, repo.GetAll().Count, "Prazan fajl treba rezultovati praznom listom.");
        }

        
        [TestMethod]
        public void Ucitaj_InvalidLineWithLessThan3Fields_SkipsLine()
        {
          
            var lines = new List<string>
            {
                "validuser;validpass;user",
                "invalidline;onlytwovalues",
                "admin2;adminpass2;admin"
            };
            File.WriteAllLines(_testFilePath, lines);

            var repo = new UserRepository(_testFilePath);

            Assert.AreEqual(2, repo.GetAll().Count, "Trebalo bi učitati samo 2 validna korisnika.");
            Assert.AreEqual("validuser", repo.GetAll()[0].getKorisnickoIme());
            Assert.AreEqual("admin2", repo.GetAll()[1].getKorisnickoIme());
        }

        
        [TestMethod]
        public void Ucitaj_LineWithExactly3Fields_Loads()
        {
            
            File.WriteAllText(_testFilePath, "user1;pass1;user");

            var repo = new UserRepository(_testFilePath); 

            Assert.AreEqual(1, repo.GetAll().Count);
            Assert.AreEqual("user1", repo.GetAll()[0].getKorisnickoIme());
            Assert.AreEqual("pass1", repo.GetAll()[0].getLozinka());
        }

      
        [TestMethod]
        public void GetAll_EmptyRepository_ReturnsEmptyList()
        {
            
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath);

            Assert.AreEqual(0, repo.GetAll().Count);
            Assert.IsNotNull(repo.GetAll());
        }

        
        [TestMethod]
        public void Sacuvaj_Administrator_SavesAsAdmin()
        {
            
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath); 

            var admin = new Administrator();
            admin.setKorisnickoIme("admintest");
            admin.setLozinka("adminpass");
            repo.GetAll().Add(admin);

            repo.Sacuvaj();

            var lines = File.ReadAllLines(_testFilePath);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("admintest;adminpass;admin", lines[0]);
        }

      
        [TestMethod]
        public void Sacuvaj_Gledalac_SavesAsUser()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath);

            var gledalac = new Gledalac();
            gledalac.setKorisnickoIme("viewertest");
            gledalac.setLozinka("viewerpass");
            repo.GetAll().Add(gledalac);

            repo.Sacuvaj();

            var lines = File.ReadAllLines(_testFilePath);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("viewertest;viewerpass;user", lines[0]);
        }

        
        [TestMethod]
        public void Sacuvaj_ThenUcitaj_DataPersists()
        {
           
            ForceDeleteFile(_testFilePath);

            var repo1 = new UserRepository(_testFilePath); 
            var admin = new Administrator();
            admin.setKorisnickoIme("persistadmin");
            admin.setLozinka("persistpass");
            repo1.GetAll().Add(admin);
            repo1.Sacuvaj();

            var repo2 = new UserRepository(_testFilePath); 

            Assert.AreEqual(1, repo2.GetAll().Count);
            Assert.IsInstanceOfType(repo2.GetAll()[0], typeof(Administrator));
            Assert.AreEqual("persistadmin", repo2.GetAll()[0].getKorisnickoIme());
            Assert.AreEqual("persistpass", repo2.GetAll()[0].getLozinka());
        }

        
        [TestMethod]
        public void Sacuvaj_ExistingFile_Overwrites()
        {
           
            File.WriteAllText(_testFilePath, "olduser;oldpass;user");

            var repo = new UserRepository(_testFilePath); 
            repo.GetAll().Clear();

            var newUser = new Gledalac();
            newUser.setKorisnickoIme("newuser");
            newUser.setLozinka("newpass");
            repo.GetAll().Add(newUser);

            repo.Sacuvaj();

            var lines = File.ReadAllLines(_testFilePath);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("newuser;newpass;user", lines[0]);
        }

       
        [TestMethod]
        public void GetAll_ReturnsSameListReference()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath); 

            var list1 = repo.GetAll();
            var list2 = repo.GetAll();

            Assert.AreSame(list1, list2);
        }

       
        [TestMethod]
        public void Sacuvaj_MultipleUsers_SavesAll()
        {
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath); 

            for (int i = 1; i <= 10; i++)
            {
                var user = new Gledalac();
                user.setKorisnickoIme($"user{i}");
                user.setLozinka($"pass{i}");
                repo.GetAll().Add(user);
            }

            repo.Sacuvaj();

            var lines = File.ReadAllLines(_testFilePath);
            Assert.AreEqual(10, lines.Length);
        }
    }
}