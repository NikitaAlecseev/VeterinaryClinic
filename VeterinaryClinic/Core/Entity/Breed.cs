using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeterinaryClinic.Core.Interface;

namespace VeterinaryClinic.Core.Entity
{
    public class Breed : IBreed
    {
        public string IDBreed { get; set; }
        public string TitleBreed { get; set; }
        public string IdTypeAnimal { get; set; }

        public Breed(string iDBreed, string titleBreed, string idTypeAnimal)
        {
            IDBreed = iDBreed;
            TitleBreed = titleBreed;
            IdTypeAnimal = idTypeAnimal;
        }
    }
}
