using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeterinaryClinic.Core.Entity;

namespace VeterinaryClinic.Core.Interface
{
    interface IRecord
    {
        string ID { get; set; }
        string IDClient { get; set; }
        string Surname { get; set; }
        string Name { get; set; }
        string Patronymic { get; set; }
        string Phone { get; set; }
        string DateReception { get; set; } // дата приема
        string TimeReception { get; set; } // время приема
        string Claim { get; set; } // жалоба
        string FullName { get;}
        string IDStatusRecord { get; set; }

        Employee Veterinar { get; set; }
        Animal AnimalClient { get; set; }
    }
}
