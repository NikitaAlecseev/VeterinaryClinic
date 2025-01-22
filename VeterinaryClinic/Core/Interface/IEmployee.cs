using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeterinaryClinic.Core.Interface
{
    interface IEmployee
    {
        string ID { get; set; }
        string Surname { get; set; }
        string Name { get; set; }
        string Patronymic { get; set; }
        string Post { get; set; }
        string Phone { get; set; }
        string PathPhoto { get; set; }
        string Address { get; set; }
        string WorkOffice { get; set; } // рабочий кабинет
        bool IsVacation { get; set; } // человек в отпуске?

        string FullName { get; }
        string Login { get; set; }
        string Password { get; set; }
    }
}
