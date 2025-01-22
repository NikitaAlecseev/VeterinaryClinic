using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeterinaryClinic.Core.Interface;

namespace VeterinaryClinic.Core.Entity
{
    public class Record : IRecord
    {
        public string ID { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string DateReception { get; set; }
        public string TimeReception { get; set; }
        public string Claim { get; set; }

        public string FullDateReception
        {
            get
            {
                return $"{DateReception} {TimeReception}";
            }
        }

        public string FullName
        {
            get
            {
                return $"{Surname} {Name} {Patronymic}";
            }
        }
        public Employee Veterinar { get; set; }
        public Animal AnimalClient { get; set; }
        public string IDStatusRecord { get; set; }
        public string IDClient { get; set; }

        public Record(string iD, string surname, string name, string patronymic, string phone, string dateReception, string timeReception, string claim,string idStatusRecord, string idClient, Animal animal, Employee veterinar)
        {
            ID = iD;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Phone = phone;
            DateReception =  dateReception.Replace(" 0:00:00","");
            TimeReception = timeReception;
            Claim = claim;
            Veterinar = veterinar;
            AnimalClient = animal;
            IDStatusRecord = idStatusRecord;
            IDClient = idClient;
        }
    }
}
