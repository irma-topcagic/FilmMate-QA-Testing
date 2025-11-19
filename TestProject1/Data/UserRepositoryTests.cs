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
    // **UKLONJEN [DoNotParallelize]** - Dozvoljava paralelizam
    public class UserRepositoryTests
    {
        private string originalFilePath = "korisnici.txt";
        private string backupFilePath = "korisnici_backup.txt";

        // **DODANO**: Putanja za privremenu datoteku specifičnu za test
        private string _testFilePath;
        private static int _testCounter = 0;

        // ============================================
        // ROBUSTNA METODA ZA BRISANJE FAJLA
        // ============================================
        private void ForceDeleteFile(string filePath)
        {
            // ... (implementacija ostaje ista) ...
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
            // **IZMJENA**: Generiše jedinstvenu putanju za svaki test
            int currentTestId = Interlocked.Increment(ref _testCounter);
            _testFilePath = $"test_korisnici_{currentTestId}_{DateTime.Now.Ticks}.txt";

            // 1. Osiguraj da originalni fajl ne ostane zaključan.
            // Ostavljamo ovaj dio za slučaj da originalni fajl nekako utiče na testove, 
            // ali je primarni fokus na _testFilePath.
            // **NAPOMENA:** Ova linija je sada manje kritična za paralelizam, ali ostaje kao oprez.
            ForceDeleteFile(originalFilePath);

            // 2. Backup originalnog fajla (ako je postojao) - ostavljeno radi sigurnosti
            if (File.Exists(originalFilePath) && !File.Exists(backupFilePath))
            {
                try
                {
                    File.Copy(originalFilePath, backupFilePath, true);
                }
                catch { /* ignorisanje grešaka kopiranja */ }
            }

            // 3. Agresivno brisanje privremene datoteke ako je ostala od prethodnih neuspješnih pokretanja
            ForceDeleteFile(_testFilePath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // **IZMJENA**: Agresivno briše SAMO privremenu datoteku testa
            ForceDeleteFile(_testFilePath);

            // 2. Restore originalnog fajla iz backupa
            if (File.Exists(backupFilePath))
            {
                try
                {
                    // Prvo oslobodi originalni fajl, pa ga prepiši
                    ForceDeleteFile(originalFilePath);
                    File.Copy(backupFilePath, originalFilePath, true);
                    ForceDeleteFile(backupFilePath);
                }
                catch { /* ignorisanje grešaka vraćanja */ }
            }

            // 3. Očisti sve privremene fajlove (kao dodatna sigurnost, iako ih TestCleanup treba riješiti)
            // Uklonjeno je Directory.GetFiles jer je spor i otežava paralelizam. 
            // Svaki test sada briše svoju datoteku u ForceDeleteFile(_testFilePath);
        }

        private void SetPrivateFilePath(UserRepository repo, string newPath)
        {
            // ... (implementacija ostaje ista) ...
            var field = typeof(UserRepository).GetField("filePath",
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(repo, newPath);
        }

        private void InvokePrivateUcitaj(UserRepository repo)
        {
            // ... (implementacija ostaje ista) ...
            var method = typeof(UserRepository).GetMethod("Ucitaj",
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(repo, null);
        }

        // ============================================
        // TEST 1: Konstruktor - Fajl ne postoji
        // ============================================
        [TestMethod]
        public void Constructor_FileDoesNotExist_CreatesEmptyList()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath); // **IZMJENA**: Proslijeđivanje jedinstvene putanje

            Assert.IsNotNull(repo);
            Assert.AreEqual(0, repo.GetAll().Count, "Lista korisnika treba biti prazna kad fajl ne postoji.");
        }

        // ============================================
        // TEST 2: Konstruktor - Prazan fajl
        // ============================================
        [TestMethod]
        public void Constructor_EmptyFile_CreatesEmptyList()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            File.WriteAllText(_testFilePath, "");

            var repo = new UserRepository(_testFilePath); // **IZMJENA**: Proslijeđivanje jedinstvene putanje

            Assert.AreEqual(0, repo.GetAll().Count, "Prazan fajl treba rezultovati praznom listom.");
        }

        // ============================================
        // TEST 4: Ucitaj - Linija sa manje od 3 polja (preskače)
        // ============================================
        [TestMethod]
        public void Ucitaj_InvalidLineWithLessThan3Fields_SkipsLine()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            var lines = new List<string>
            {
                "validuser;validpass;user",
                "invalidline;onlytwovalues",
                "admin2;adminpass2;admin"
            };
            File.WriteAllLines(_testFilePath, lines);

            var repo = new UserRepository(_testFilePath); // **IZMJENA**: Proslijeđivanje jedinstvene putanje

            Assert.AreEqual(2, repo.GetAll().Count, "Trebalo bi učitati samo 2 validna korisnika.");
            Assert.AreEqual("validuser", repo.GetAll()[0].getKorisnickoIme());
            Assert.AreEqual("admin2", repo.GetAll()[1].getKorisnickoIme());
        }

        // ============================================
        // TEST 5: Ucitaj - Linija sa tačno 3 polja
        // ============================================
        [TestMethod]
        public void Ucitaj_LineWithExactly3Fields_Loads()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            File.WriteAllText(_testFilePath, "user1;pass1;user");

            var repo = new UserRepository(_testFilePath); // **IZMJENA**: Proslijeđivanje jedinstvene putanje

            Assert.AreEqual(1, repo.GetAll().Count);
            Assert.AreEqual("user1", repo.GetAll()[0].getKorisnickoIme());
            Assert.AreEqual("pass1", repo.GetAll()[0].getLozinka());
        }

        // ============================================
        // TEST 11: GetAll - Prazan repozitorij
        // ============================================
        [TestMethod]
        public void GetAll_EmptyRepository_ReturnsEmptyList()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath); // **IZMJENA**: Proslijeđivanje jedinstvene putanje

            Assert.AreEqual(0, repo.GetAll().Count);
            Assert.IsNotNull(repo.GetAll());
        }

        // ============================================
        // TEST 12: Sacuvaj - Čuva Administrator kao "admin"
        // ============================================
        [TestMethod]
        public void Sacuvaj_Administrator_SavesAsAdmin()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath); // **IZMJENA**: Proslijeđivanje jedinstvene putanje

            var admin = new Administrator();
            admin.setKorisnickoIme("admintest");
            admin.setLozinka("adminpass");
            repo.GetAll().Add(admin);

            repo.Sacuvaj();

            var lines = File.ReadAllLines(_testFilePath);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("admintest;adminpass;admin", lines[0]);
        }

        // ============================================
        // TEST 13: Sacuvaj - Čuva Gledalac kao "user"
        // ============================================
        [TestMethod]
        public void Sacuvaj_Gledalac_SavesAsUser()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath); // **IZMJENA**: Proslijeđivanje jedinstvene putanje

            var gledalac = new Gledalac();
            gledalac.setKorisnickoIme("viewertest");
            gledalac.setLozinka("viewerpass");
            repo.GetAll().Add(gledalac);

            repo.Sacuvaj();

            var lines = File.ReadAllLines(_testFilePath);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("viewertest;viewerpass;user", lines[0]);
        }

        // ============================================
        // TEST 16: Sacuvaj pa Ucitaj - Persistence
        // ============================================
        [TestMethod]
        public void Sacuvaj_ThenUcitaj_DataPersists()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            ForceDeleteFile(_testFilePath);

            var repo1 = new UserRepository(_testFilePath); // **IZMJENA**: Proslijeđivanje jedinstvene putanje
            var admin = new Administrator();
            admin.setKorisnickoIme("persistadmin");
            admin.setLozinka("persistpass");
            repo1.GetAll().Add(admin);
            repo1.Sacuvaj();

            var repo2 = new UserRepository(_testFilePath); // **IZMJENA**: Kreiranje novog repozitorija iz iste datoteke

            Assert.AreEqual(1, repo2.GetAll().Count);
            Assert.IsInstanceOfType(repo2.GetAll()[0], typeof(Administrator));
            Assert.AreEqual("persistadmin", repo2.GetAll()[0].getKorisnickoIme());
            Assert.AreEqual("persistpass", repo2.GetAll()[0].getLozinka());
        }

        // ============================================
        // TEST 17: Sacuvaj - Prepisuje postojeći fajl
        // ============================================
        [TestMethod]
        public void Sacuvaj_ExistingFile_Overwrites()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            File.WriteAllText(_testFilePath, "olduser;oldpass;user");

            var repo = new UserRepository(_testFilePath); // **IZMJENA**: Proslijeđivanje jedinstvene putanje
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

        // ============================================
        // TEST 19: GetAll - Vraća istu referencu
        // ============================================
        [TestMethod]
        public void GetAll_ReturnsSameListReference()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath); // **IZMJENA**: Proslijeđivanje jedinstvene putanje

            var list1 = repo.GetAll();
            var list2 = repo.GetAll();

            Assert.AreSame(list1, list2);
        }

        // ============================================
        // TEST 22: Sacuvaj - Veliki broj korisnika
        // ============================================
        [TestMethod]
        public void Sacuvaj_MultipleUsers_SavesAll()
        {
            // **IZMJENA**: Radi sa jedinstvenom putanjom
            ForceDeleteFile(_testFilePath);

            var repo = new UserRepository(_testFilePath); // **IZMJENA**: Proslijeđivanje jedinstvene putanje

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