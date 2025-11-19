using System;
using System.Text;
using System.Security.Cryptography;
using FilmMate.Models;
using FilmMate.Data;

namespace FilmMate.Services
{
    public class UserService
    {
        private UserRepository userRepo;

        public UserService(UserRepository repo)
        {
            userRepo = repo;
        }

        // =========================================
        // Registracija
        // =========================================
        public void Registracija(TextReader input, TextWriter output)
        {
            output.Write("Korisničko ime: ");
            string ime = input.ReadLine() ?? "";

            bool postoji = userRepo.GetAll().Exists(k => k.getKorisnickoIme().Equals(ime, StringComparison.OrdinalIgnoreCase));

            if (postoji)
            {
                output.WriteLine("Korisnik već postoji!");
                return;
            }

            output.Write("Lozinka: ");
            string pass = input.ReadLine() ?? "";
            string hash = GenerisiHash(pass);

            Gledalac novi = new Gledalac();
            novi.setKorisnickoIme(ime);
            novi.setLozinka(hash);

            userRepo.GetAll().Add(novi);
            userRepo.Sacuvaj();
            output.WriteLine("Registracija uspješna!");
        }

        // =========================================
        // Prijava
        // =========================================
        public Korisnik? Prijava(TextReader input, TextWriter output)
        {
            output.Write("Korisničko ime: ");
            string ime = input.ReadLine() ?? "";
            output.Write("Lozinka: ");
            string pass = input.ReadLine() ?? "";

            string hash = GenerisiHash(pass);

            Korisnik? korisnik = userRepo.GetAll()
                .Find(k => k.getKorisnickoIme() == ime && k.getLozinka() == hash);

            if (korisnik == null)
                output.WriteLine("Pogrešno korisničko ime ili lozinka!");
            else
                output.WriteLine($"Dobrodošao {korisnik.getKorisnickoIme()}");

            return korisnik;
        }

        // =========================================
        // Generisanje SHA256 heša
        // =========================================
        private string GenerisiHash(string lozinka)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(lozinka));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
