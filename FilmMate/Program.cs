using FilmMate.Data;
using FilmMate.Services;
using FilmMate.Models;

class Program
{
    static void Main()
    {
        var userRepo = new UserRepository();
        var filmRepo = new FilmRepository();
        var userService = new UserService(userRepo);
        var filmService = new FilmService(filmRepo);

        while (true)
        {
            Console.WriteLine("\n1. Registracija\n2. Prijava\n3. Izlaz");
            Console.Write("Odabir: ");
            string izbor = Console.ReadLine() ?? "";

            if (izbor == "1") userService.Registracija();
            else if (izbor == "2")
            {
                var user = userService.Prijava();
                if (user is Administrator admin)
                    AdminMeni(admin, filmService);
                else if (user is Gledalac gledalac)
                    GledalacMeni(gledalac, filmService);
            }
            else if (izbor == "3") break;
        }
    }

    static void AdminMeni(Administrator admin, FilmService fs)
    {
        bool nazad = false;
        while (!nazad)
        {
            Console.WriteLine("\n1. Dodaj film\n2. Ažuriraj film\n3. Obriši film\n4. Prikaz filmova\n5. Pretraga/Filtriranje filmova\n6. Prikaz svih kategorija\n7. Sortiraj Filmove\n0. Nazad");
            string odabir = Console.ReadLine() ?? "";

            switch (odabir)
            {
                case "1": admin.dodajFilm(fs); break;
                case "2": admin.azurirajFilm(fs); break;
                case "3": admin.obrisiFilm(fs); break;
                case "4": fs.prikaziFilmove(); break;
                case "5": fs.FiltrirajPretraziFilmove(); break;
                case "6": fs.PrikaziJedinstveneKategorije(); break;
                case "7": SortirajMeni(fs); break;
                case "0": nazad = true; break;
                default: Console.WriteLine("Pogrešan unos!"); break;
            }
        }
    }

    static void GledalacMeni(Gledalac gledalac, FilmService fs)
    {
        bool nazad = false;
        while (!nazad)
        {
            Console.WriteLine("\n--- Meni Gledaoca ---");
            Console.WriteLine("1. Prikaz svih filmova");
            Console.WriteLine("2. Pretraga/Filtriranje filmova");
            Console.WriteLine("3. Sortiraj Filmove");
            Console.WriteLine("0. Nazad (Odjava)");
            Console.Write("Odabir: ");
            string odabir = Console.ReadLine() ?? "";

            switch (odabir)
            {
                case "1": fs.prikaziFilmove(); break;
                case "2": gledalac.pretragaFilmova(fs); break;
                case "3": SortirajMeni(fs); break;
                case "0": nazad = true; break;
                default: Console.WriteLine("Pogrešan unos!"); break;
            }
        }
    }

    static void SortirajMeni(FilmService fs)
    {
        bool nazad = false;
        while (!nazad)
        {
            Console.WriteLine("\n--- Sortiranje Filmova ---");
            Console.WriteLine("1. Po Ocjeni (Najmanja -> Najveća)");
            Console.WriteLine("2. Po Godini Izlaska (Najstariji -> Najnoviji)");
            Console.WriteLine("3. Po Nazivu (A-Z)");
            Console.WriteLine("0. Nazad");
            string odabir = Console.ReadLine() ?? "";

            switch (odabir)
            {
                case "1": fs.SortirajPoOcjeni(); break;
                case "2": fs.SortirajPoGodini(); break;
                case "3": fs.SortirajPoNazivu(); break;
                case "0": nazad = true; break;
                default: Console.WriteLine("Pogrešan unos!"); break;
            }
        }
    }
}
