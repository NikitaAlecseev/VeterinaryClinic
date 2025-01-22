using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeterinaryClinic.Core.Interface;

namespace VeterinaryClinic.Core.Entity
{
    public class Vacation : IVacation
    {
        public string IDVacation { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }

        public Vacation(string iDVacation, string dateStart, string dateEnd)
        {
            IDVacation = iDVacation;
            DateStart = dateStart;
            DateEnd = dateEnd;
        }
    }
}
