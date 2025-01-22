using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeterinaryClinic.Core.Interface
{
    interface IAnimals
    {
        string IDAnimal { get; set; }
        string Nickname { get; set; }
        string Gender { get; set; }
        string PathPhoto { get; set; }
        string TypeAnimal { get; set; }
        string Breed { get; set; }
        string IDTypeAnimal { get; set; }
        string IDBreed { get; set; }
    }
}
