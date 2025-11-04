using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FilmMate.Models;
using FilmMate.Data;

namespace FilmMate.Services
{
    public class FilmService
    {
        private FilmRepository repo;

        public FilmService(FilmRepository filmRepo)
        {
            repo = filmRepo;
        }

        public void dodajFilm()
        {
            Console.Write("Naziv: ");
            string naziv = Console.ReadLine() ?? "";
            bool postoji = false;

            foreach (var f in repo.GetAll())
            {
                if (f.getNazivFilma().ToLower() == naziv.ToLower())
                {
                    postoji = true;
                    break;
                }
            }

            if (postoji)
            {
                Console.WriteLine("Film već postoji!");
                return;
            }

            Console.Write("Kategorija: ");
            string kat = Console.ReadLine() ?? "";
            Console.Write("Ocjena: ");
            double ocjena = double.TryParse(Console.ReadLine(), out ocjena) ? ocjena : 0;
            Console.Write("Godina: ");
            int godina = int.TryParse(Console.ReadLine(), out godina) ? godina : 0;

            repo.GetAll().Add(new Film(naziv, kat, ocjena, godina));
            repo.Sacuvaj();
            Console.WriteLine("Film uspješno dodat!");
        }

        public void obrisiFilm()
        {
            Console.Write("Unesite naziv filma: ");
            string naziv = Console.ReadLine() ?? "";

            bool obrisan = false;
            for (int i = 0; i < repo.GetAll().Count; i++)
            {
                if (repo.GetAll()[i].getNazivFilma().ToLower() == naziv.ToLower())
                {
                    repo.GetAll().RemoveAt(i);
                    obrisan = true;
                    break;
                }
            }

            if (obrisan)
            {
                repo.Sacuvaj();
                Console.WriteLine("Film obrisan!");
            }
            else Console.WriteLine("Film nije pronađen!");
        }

        public void azurirajFilm()
        {
            Console.Write("Unesite naziv filma: ");
            string naziv = Console.ReadLine() ?? "";

            Film filmZaAzuriranje = null;
            foreach (var f in repo.GetAll())
            {
                if (f.getNazivFilma().ToLower() == naziv.ToLower())
                {
                    filmZaAzuriranje = f;
                    break;
                }
            }

            if (filmZaAzuriranje == null)
            {
                Console.WriteLine("Film nije pronađen!");
                return;
            }

            izvrsiAzuriranje(filmZaAzuriranje); 

            repo.Sacuvaj();
            Console.WriteLine("Film ažuriran!");
        }

        private void izvrsiAzuriranje(Film film)
        {
            Console.WriteLine("1. Naziv 2. Kategorija 3. Ocjena 4. Godina");
            string izbor = Console.ReadLine() ?? "";

            switch (izbor)
            {
                case "1":
                    Console.Write("Novi naziv: ");
                    film.setNaziv(Console.ReadLine() ?? "");
                    break;
                case "2":
                    Console.Write("Nova kategorija: ");
                    film.setKategorija(Console.ReadLine() ?? "");
                    break;
                case "3":
                    Console.Write("Nova ocjena: ");
                    if (double.TryParse(Console.ReadLine(), out double ocj))
                        film.setOcjena(ocj);
                    break;
                case "4":
                    Console.Write("Nova godina: ");
                    if (int.TryParse(Console.ReadLine(), out int god))
                        film.setGodina(god);
                    break;
                default:
                    Console.WriteLine("Pogrešan unos!");
                    break;
            }
        }

        public void FiltrirajPretraziFilmove()
        {
            if (repo.GetAll().Count == 0)
            {
                Console.WriteLine("Nema filmova za pretragu!");
                return;
            }

            Console.WriteLine("\n--- Filtriranje i Pretraga Filmova ---");
            Console.WriteLine("1. Pretraga po Nazivu");
            Console.WriteLine("2. Filtriranje po Kategoriji");
            Console.WriteLine("3. Filtriranje po Minimalnoj Ocjeni");
            Console.WriteLine("0. Povratak na Meni");
            Console.Write("Odabir: ");
            string izbor = Console.ReadLine() ?? "";

            List<Film> rezultati = new List<Film>();

            switch (izbor)
            {
                case "1": rezultati = filtrirajPoNazivu(); break;
                case "2": rezultati = filtrirajPoKategoriji(); break;
                case "3": rezultati = filtrirajPoMinimalnojOcjeni(); break;
                case "0": return; 
                default: Console.WriteLine("Pogrešan odabir."); return;
            }

            PrikaziListuFilmova(rezultati, "Pronađeno");
        }

        private List<Film> filtrirajPoNazivu()
        {
            Console.Write("Unesite dio naziva filma za pretragu: ");
            string dioNaziva = Console.ReadLine()?.ToLower() ?? "";
            return repo.GetAll()
                .Where(f => f.getNazivFilma().ToLower().Contains(dioNaziva))
                .ToList();
        }

        private List<Film> filtrirajPoKategoriji()
        {
            Console.Write("Unesite kategoriju za filtriranje: ");
            string kategorija = Console.ReadLine()?.ToLower() ?? "";
            return repo.GetAll()
                .Where(f => f.getKategorija().ToLower() == kategorija)
                .ToList();
        }

        private List<Film> filtrirajPoMinimalnojOcjeni()
        {
            Console.Write("Unesite minimalnu ocjenu: ");
            if (double.TryParse(Console.ReadLine(), out double minOcjena))
            {
                return repo.GetAll()
                    .Where(f => f.getOcjena() >= minOcjena)
                    .ToList();
            }
            else
            {
                Console.WriteLine("Neispravan format ocjene.");
                return new List<Film>();
            }
        }

        public void PrikaziJedinstveneKategorije()
        {
            if (repo.GetAll().Count == 0)
            {
                Console.WriteLine("Nema filmova, nema ni kategorija.");
                return;
            }

            var kategorije = repo.GetAll()
                                  .Select(f => f.getKategorija())
                                  .Distinct()
                                  .ToList();

            Console.WriteLine("\n--- Postojeće Kategorije ---");
            foreach (var kat in kategorije)
            {
                Console.WriteLine($"- {kat}");
            }
            Console.WriteLine("----------------------------");
        }


        private List<Film> Merge(List<Film> left, List<Film> right, Func<Film, Film, bool> isLess)
        {
            var result = new List<Film>();
            int i = 0, j = 0;

            while (i < left.Count && j < right.Count)
            {
                if (isLess(left[i], right[j]))
                {
                    result.Add(left[i]);
                    i++;
                }
                else
                {
                    result.Add(right[j]);
                    j++;
                }
            }

            result.AddRange(left.Skip(i));
            result.AddRange(right.Skip(j));
            return result;
        }

        private List<Film> MergeSort(List<Film> list, Func<Film, Film, bool> isLess)
        {
            if (list.Count <= 1)
                return list;

            int mid = list.Count / 2;
            var left = new List<Film>(list.GetRange(0, mid));
            var right = new List<Film>(list.GetRange(mid, list.Count - mid));

            left = MergeSort(left, isLess);
            right = MergeSort(right, isLess);

            return Merge(left, right, isLess);
        }

        public void SortirajPoOcjeni()
        {
            var sortirano = MergeSort(repo.GetAll(), (f1, f2) => f1.getOcjena() < f2.getOcjena());
            PrikaziListuFilmova(sortirano, "Sortirana Lista (Ocjena)");
        }

        public void SortirajPoGodini()
        {
            var sortirano = MergeSort(repo.GetAll(), (f1, f2) => f1.getGodina() < f2.getGodina());
            PrikaziListuFilmova(sortirano, "Sortirana Lista (Godina)");
        }

        public void SortirajPoNazivu()
        {
            var sortirano = MergeSort(repo.GetAll(), (f1, f2) => string.Compare(f1.getNazivFilma(), f2.getNazivFilma()) < 0);
            PrikaziListuFilmova(sortirano, "Sortirana Lista (Naziv)");
        }
        
        private void PrikaziListuFilmova(List<Film> filmovi, string naslov)
        {
            if (!filmovi.Any())
            {
                Console.WriteLine("Lista filmova je prazna ili nije pronađena.");
            }
            else
            {
                Console.WriteLine($"\n--- {naslov} ({filmovi.Count} filmova) ---");
                foreach (var f in filmovi)
                {
                    Console.WriteLine(f);
                }
            }
        }
        
        public void prikaziFilmove()
        {
            PrikaziListuFilmova(repo.GetAll(), "Svi Filmovi u Bazi");
        }
    }
}
