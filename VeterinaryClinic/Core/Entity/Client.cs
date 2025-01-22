using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeterinaryClinic.Core.Interface;

namespace VeterinaryClinic.Core.Entity
{
    public class Client : IClient
    {
        public string IDClient { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }

        public string FullName
        {
            get
            {
                return $"{Surname} {Name} {Patronymic}";
            }
        }
        public List<Animal> Animals { get; set; }


        public Client(string iDClient, string surname, string name, string patronymic, string phone, List<Animal> animals)
        {
            IDClient = iDClient;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Phone = phone;
            Animals = animals;
        }
    }

}
