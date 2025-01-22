using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using VeterinaryClinic.Core.Helper;
using VeterinaryClinic.Core.Interface;

namespace VeterinaryClinic.Core.Entity
{
    public class Employee : IEmployee
    {
        public string ID { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Post { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string WorkOffice { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsVacation { get; set; }
        public string FullName
        {
            get
            {
                return $"{Surname} {Name} {Patronymic}";
            }
        }

        public Visibility getVisibility
        {
            get {
                if (IsVacation)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
                
            }
        }

        public string PathPhoto { get; set; }

        public BitmapImage GetFullPathPhoto
        {
            get
            {
                if (PathPhoto != "" && File.Exists(PhotoDB.GetPathPerson(PathPhoto)))
                {
                    var bytes = File.ReadAllBytes(PhotoDB.GetPathPerson(PathPhoto));
                    return PhotoDB.ToImage(bytes);
                }
                else
                {
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    logo.UriSource = new Uri("pack://application:,,,/Images/emptyUser.png");
                    logo.EndInit();
                    return logo;
                }

            }
        }


        public Employee(string iD, string surname, string name, string patronymic, string post, string phone,string pathPhoto, string address, string workOffice, bool isVacation, string login, string password)
        {
            ID = iD;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Post = post;
            Phone = phone;
            Address = address;
            WorkOffice = workOffice;
            IsVacation = isVacation;
            PathPhoto = pathPhoto;
            Login = login;
            Password = password;
        }
    }
}
