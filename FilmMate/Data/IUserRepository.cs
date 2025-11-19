using FilmMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmMate.Data
{
    public interface IUserRepository
    {
        List<Korisnik> GetAll();
        void Sacuvaj();
    }
}
