using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmMate.Models
{
    public class Film
    {
        private string nazivFilma;
        private string kategorija;
        private double ocjena;
        private int godinaIzlaska;

        public Film(string naziv, string kat, double ocj, int god)
        {
            nazivFilma = naziv;
            kategorija = kat;
            ocjena = ocj;
            godinaIzlaska = god;
        }

        public string getNazivFilma() => nazivFilma;
        public string getKategorija() => kategorija;
        public double getOcjena() => ocjena;
        public int getGodina() => godinaIzlaska;

        public void setNaziv(string n) => nazivFilma = n;
        public void setKategorija(string k) => kategorija = k;
        public void setOcjena(double o) => ocjena = o;
        public void setGodina(int g) => godinaIzlaska = g;

        public override string ToString()
        {
            return $"{nazivFilma} | {kategorija} | {godinaIzlaska} | {ocjena}";
        }
    }
}

