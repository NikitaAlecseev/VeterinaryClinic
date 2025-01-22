using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeterinaryClinic.Core.Interface
{
    interface IVacation
    {
        string IDVacation { get; set; }
        string DateStart { get; set; }
        string DateEnd { get; set; }
    }
}
