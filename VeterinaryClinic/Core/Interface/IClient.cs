using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeterinaryClinic.Core.Entity;

namespace VeterinaryClinic.Core.Interface
{
    interface IClient
    {
        string IDClient { get; set; }
        string Surname { get; set; }
        string Name { get; set; }
        string Patronymic { get; set; }
        string Phone { get; set; }
        List<Animal> Animals { get; set; }
    }
}
