using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeterinaryClinic.Core.Interface
{
    public interface IBreed
    {
        string IDBreed { get; set; }
        string TitleBreed { get; set; }
        string IdTypeAnimal { get; set; }
    }
}
